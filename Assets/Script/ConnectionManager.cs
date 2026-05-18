using UnityEngine;
using UnityEngine.InputSystem;

public class ConnectionManager : MonoBehaviour
{
    public static ConnectionManager Instance;

    private PlayerInputActions input;
    private Node startNode;
    private LineDragController line;
    private Camera cam;

    [Header("Prefabs")]
    public LineRenderer linePrefab;

    [Header("Connection Rules")]
    public float maxConnectionDistance = 2.5f;   // adjust in Inspector

    [System.Obsolete]
    private void Awake()
    {
        Instance = this;
        cam = Camera.main;
        line = FindObjectOfType<LineDragController>();

        input = new PlayerInputActions();
        input.UI.Click.started += ctx => OnClickStart();
        input.UI.Click.canceled += ctx => OnClickEnd();
    }

    private void OnEnable() => input.Enable();
    private void OnDisable() => input.Disable();

    private void OnClickStart()
    {
        Vector2 mousePos = input.UI.Position.ReadValue<Vector2>();
        Vector3 worldPos = cam.ScreenToWorldPoint(mousePos);
        worldPos.z = 0;

        Collider2D hit = Physics2D.OverlapPoint(worldPos);
        if (hit != null)
        {
            Node node = hit.GetComponent<Node>();
            if (node != null)
            {
                // ✅ Only StartNode OR already connected nodes can start a line
                if (node.isStartNode || node.isConnected)
                {
                    startNode = node;
                    line.BeginLine(node.transform.position);
                }
            }
        }
    }

    private void OnClickEnd()
    {
        if (startNode == null) return;

        Vector2 mousePos = input.UI.Position.ReadValue<Vector2>();
        Vector3 worldPos = cam.ScreenToWorldPoint(mousePos);
        worldPos.z = 0;

        Collider2D hit = Physics2D.OverlapPoint(worldPos);
        if (hit != null)
        {
            Node targetNode = hit.GetComponent<Node>();
            if (targetNode != null && targetNode != startNode)
            {
                // ⭐ RULE: Cannot connect if too far away
                float distance = Vector3.Distance(startNode.transform.position, targetNode.transform.position);
                if (distance > maxConnectionDistance)
                {
                    Debug.Log("Connection too far — must be within " + maxConnectionDistance);
                    line.EndLine();
                    startNode = null;
                    return;
                }

                // ⭐ RULE: StartNode cannot connect directly to EndNode
                if (startNode.isStartNode && targetNode.isEndNode)
                {
                    Debug.Log("StartNode cannot connect directly to EndNode.");
                    line.EndLine();
                    startNode = null;
                    return;
                }

                // ⭐ If this is the EndNode, load next scene
                if (targetNode.isEndNode)
                {
                    if (!string.IsNullOrEmpty(targetNode.nextSceneName))
                    {
                        UnityEngine.SceneManagement.SceneManager.LoadScene(targetNode.nextSceneName);
                    }
                    else
                    {
                        Debug.LogWarning("EndNode has no scene assigned!");
                    }
                    return;
                }

                // ⭐ Only connect if target is not already connected
                if (!targetNode.isConnected)
                {
                    LineRenderer newLine = Instantiate(linePrefab);
                    newLine.positionCount = 2;
                    newLine.SetPosition(0, startNode.transform.position);
                    newLine.SetPosition(1, targetNode.transform.position);

                    targetNode.OnConnected();
                }
            }
        }

        line.EndLine();
        startNode = null;
    }

    private void Update()
    {
        if (startNode != null)
        {
            Vector2 mousePos = input.UI.Position.ReadValue<Vector2>();
            Vector3 worldPos = cam.ScreenToWorldPoint(mousePos);
            worldPos.z = 0;
            line.UpdateLine(worldPos);
        }
    }
}
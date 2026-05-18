using UnityEngine;

public class LineDragController : MonoBehaviour
{
    public LineRenderer dragLinePrefab; // assign in Inspector
    private LineRenderer activeLine;

    public void BeginLine(Vector3 startPos)
    {
        if (activeLine != null) Destroy(activeLine.gameObject);

        activeLine = Instantiate(dragLinePrefab);
        activeLine.positionCount = 2;
        activeLine.useWorldSpace = true;
        activeLine.SetPosition(0, startPos);
        activeLine.SetPosition(1, startPos);
    }

    public void UpdateLine(Vector3 endPos)
    {
        if (activeLine != null)
            activeLine.SetPosition(1, endPos);
    }

    public void EndLine()
    {
        if (activeLine != null)
        {
            Destroy(activeLine.gameObject);
            activeLine = null;
        }
    }
}
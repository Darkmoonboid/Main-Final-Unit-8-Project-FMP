using UnityEngine;

public class Node : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite defaultSprite;
    public Sprite connectedSprite;

    [Header("Connection Settings")]
    public bool isStartNode = false;
    public bool isConnected = false;
    public bool isEndNode = false;

    [Header("End Node Settings")]
    public string nextSceneName = "";   // ⭐ Assigned in Inspector

    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = defaultSprite;
    }

    public void OnConnected()
    {
        isConnected = true;
        sr.sprite = connectedSprite;
    }
}
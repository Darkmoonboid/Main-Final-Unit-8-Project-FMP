using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine.UI;

public class CustomCursor : MonoBehaviour
{
    public Texture2D cursorTexture;

    private void Start()
    {
        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
    }

}

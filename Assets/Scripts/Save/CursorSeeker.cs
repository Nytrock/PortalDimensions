using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorSeeker : MonoBehaviour
{
    public static CursorSeeker cursorSeeker;

    public int cursorId;

    public Texture2D defaultNormalCursor;
    public Texture2D defaultShortCursor;

    public Texture2D normalLeftCursor;
    public Texture2D normalRightCursor;

    public Texture2D shortLeftCursor;
    public Texture2D shortRightCursor;

    private void Awake()
    {
        cursorSeeker = this;
    }

    private void Update()
    {
        if (Input.GetMouseButton(0)) {
            if(cursorId == 4)
                Cursor.SetCursor(normalLeftCursor, Vector2.zero, CursorMode.Auto);
            if (cursorId == 5)
                Cursor.SetCursor(shortLeftCursor, Vector2.zero, CursorMode.Auto);
        } else if (Input.GetMouseButton(1)) {
            if (cursorId == 4)
                Cursor.SetCursor(normalRightCursor, Vector2.zero, CursorMode.Auto);
            if (cursorId == 5)
                Cursor.SetCursor(shortRightCursor, Vector2.zero, CursorMode.Auto);
        } else {
            if (cursorId == 4)
                Cursor.SetCursor(defaultNormalCursor, Vector2.zero, CursorMode.Auto);
            if (cursorId == 5)
                Cursor.SetCursor(defaultShortCursor, Vector2.zero, CursorMode.Auto);
        }
    }
}

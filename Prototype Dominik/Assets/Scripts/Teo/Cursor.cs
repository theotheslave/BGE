using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public Texture2D customCursor;
    public Vector2 hotSpot = Vector2.zero;
    public CursorMode cursorMode = CursorMode.Auto;

    void Start()
    {
        Cursor.SetCursor(customCursor, hotSpot, cursorMode);
    }
}
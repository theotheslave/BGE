using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance;

    public Texture2D defaultCursor;
    public Texture2D lockedCursor;
    public Texture2D doorCursor;
    public Vector2 hotSpot = Vector2.zero;

    void Awake()
    {
        Instance = this;
        SetDefaultCursor();
    }

    public void SetDefaultCursor()
    {
        Cursor.SetCursor(defaultCursor, hotSpot, CursorMode.Auto);
    }

    public void SetLockedCursor()
    {
        Cursor.SetCursor(lockedCursor, hotSpot, CursorMode.Auto);
    }

    public void SetDoorCursor()
    {
        Cursor.SetCursor(doorCursor, hotSpot, CursorMode.Auto);
    }
}

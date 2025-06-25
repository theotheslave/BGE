using UnityEngine.EventSystems;
using UnityEngine;

public enum DoorUnlockType { Manual, AutoOpen }

public class SceneManagerDoor1 : MonoBehaviour
{
    [SerializeField] private int targetSceneIndex = 0;
    [SerializeField] private DoorUnlockType unlockType = DoorUnlockType.Manual;

    [Header("Cursor")]
    [SerializeField] private Texture2D lockedCursor;
    [SerializeField] private Texture2D unlockedCursor;
    [SerializeField] private Vector2 hotspot = Vector2.zero;

    private Outline outline;
    private static Transform currentHighlight;

    private bool isUnlocked = false;

    void Awake()
    {
        outline = GetComponent<Outline>() ?? gameObject.AddComponent<Outline>();
        outline.OutlineColor = Color.cyan;
        outline.OutlineWidth = 6f;
        outline.enabled = false;

        if (unlockType == DoorUnlockType.AutoOpen)
        {
            isUnlocked = true;
        }
    }

    void Update()
    {
        bool hoveredThisFrame = false;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!EventSystem.current.IsPointerOverGameObject() && Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.transform == transform)
            {
                hoveredThisFrame = true;

                if (unlockType == DoorUnlockType.Manual)
                    Cursor.SetCursor(isUnlocked ? unlockedCursor : lockedCursor, hotspot, CursorMode.Auto);

                if (currentHighlight != transform)
                {
                    if (currentHighlight != null)
                    {
                        Outline prev = currentHighlight.GetComponent<Outline>();
                        if (prev != null) prev.enabled = false;
                    }
                    currentHighlight = transform;
                }

                if (outline != null) outline.enabled = true;
            }
        }

        if (!hoveredThisFrame && currentHighlight == transform)
        {
            outline.enabled = false;
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            currentHighlight = null;
        }
    }

    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (unlockType == DoorUnlockType.Manual && !isUnlocked) return;

        FadeToBlack.Instance.FadeToScene(targetSceneIndex);
    }

    public void UnlockDoor()
    {
        isUnlocked = true;
    }
}

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider))]
public class SceneManagerDoor1 : MonoBehaviour
{
    [SerializeField] private int targetSceneIndex = 0;
    
    [SerializeField] private Texture2D unlockedCursor;
    [SerializeField] private Vector2 hotspot = Vector2.zero;

    private Outline outline;
    private static Transform currentHighlight;

    private bool isUnlocked = false;

    void Awake()
    {

        outline = GetComponent<Outline>();
        if (outline == null)
        {
            outline = gameObject.AddComponent<Outline>();
            outline.OutlineColor = Color.cyan;
            outline.OutlineWidth = 6f;
        }

        if (outline != null)
            outline.enabled = false;

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

                

                if (currentHighlight != transform)
                {
                    if (currentHighlight != null)
                    {
                        Outline prevOutline = currentHighlight.GetComponent<Outline>();
                        if (prevOutline != null) prevOutline.enabled = false;
                    }

                    currentHighlight = transform;
                }

                if (outline != null) outline.enabled = true;
            }
        }

        if (!hoveredThisFrame && currentHighlight == transform)
        {
            if (outline != null) outline.enabled = false;
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            currentHighlight = null;
        }
    }

    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
     

        FadeToBlack.Instance.FadeToScene(targetSceneIndex);

    }

  
}

using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider))]
public class SelectableObject : MonoBehaviour
{
    [SerializeField] private float splinePosition = 0.5f;

    private Outline outline;

    // Track currently hovered and selected object
    private static SelectableObject currentHighlight;
    private static SelectableObject currentSelection;
    public static Transform CurrentSelectionTransform;
    void Awake()
    {
        outline = GetComponent<Outline>();
        if (outline == null)
        {
            outline = gameObject.AddComponent<Outline>();
            outline.OutlineColor = Color.magenta;
            outline.OutlineWidth = 7f;
        }

        outline.enabled = false;
    }

    void Update()
    {
        // Clear previous highlight if needed
        if (currentHighlight != null && currentHighlight != currentSelection)
        {
            currentHighlight.outline.enabled = false;
            currentHighlight = null;
        }

        // Raycast to detect hover
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        int mask = ~LayerMask.GetMask("fadeblack");

        if (!EventSystem.current.IsPointerOverGameObject() &&
            Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, mask))
        {
            if (hit.transform == transform && currentSelection != this)
            {
                currentHighlight = this;
                if (outline != null) outline.enabled = true;
            }
        }
    }

    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        // Disable outline on click
        if (outline != null) outline.enabled = false;

        currentSelection = this;

        // Focus the camera
        CameraMovement cam = FindFirstObjectByType<CameraMovement>();
        if (cam != null)
        {
            cam.Focus(splinePosition, transform);
        }
    }

    public static void ClearSelection()
    {
        if (currentSelection != null && currentSelection.outline != null)
        {
            currentSelection.outline.enabled = false;
        }

        currentSelection = null;
    }
}
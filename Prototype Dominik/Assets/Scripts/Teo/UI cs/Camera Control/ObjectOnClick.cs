using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider))]
public class SelectableObject : MonoBehaviour
{
    [SerializeField] private float splinePosition = 0.5f; // Assign in inspector
    private Outline outline;
    private static Transform currentHighlight;
    private static Transform currentSelection;

    void Awake()
    {
        outline = GetComponent<Outline>();
        if (outline == null)
        {
            outline = gameObject.AddComponent<Outline>();
            outline.OutlineColor = Color.magenta;
            outline.OutlineWidth = 7f;
        }

    }

    void Update()
    {
        
        if (currentHighlight != null && currentHighlight != currentSelection)
        {
            Outline prev = currentHighlight.GetComponent<Outline>();
            if (prev != null) prev.enabled = false;
            currentHighlight = null;
        }

        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        int mask = ~LayerMask.GetMask("fadeblack");
        if (!EventSystem.current.IsPointerOverGameObject() && Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, mask))
        {
            if (hit.transform == transform && transform != currentSelection)
            {
                currentHighlight = transform;
                if (outline != null) outline.enabled = true;
            }
        }
    }

    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        
        if (outline != null) outline.enabled = false;
        currentSelection = transform;

       
        CameraMovement cam = FindFirstObjectByType<CameraMovement>();
        if (cam != null)
        {
            cam.Focus(splinePosition, transform);
        }
    }

   
    public static void ClearSelection()
    {
        if (currentSelection != null)
        {
            Outline o = currentSelection.GetComponent<Outline>();
            if (o != null) o.enabled = false;
        }

        currentSelection = null;
    }
}

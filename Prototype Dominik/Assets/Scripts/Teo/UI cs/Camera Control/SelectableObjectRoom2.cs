using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider))]
public class SelectableObjectRoom2 : MonoBehaviour
{
    public enum PuzzleType { None, Isochoric, Isothermal }
    [SerializeField] private float splinePosition = 0.5f;
    [SerializeField] private PuzzleType puzzleType = PuzzleType.None;

    private Outline outline;
    private static SelectableObjectRoom2 currentHighlight;
    private static SelectableObjectRoom2 currentSelection;

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
        if (currentHighlight != null && currentHighlight != currentSelection)
        {
            currentHighlight.outline.enabled = false;
            currentHighlight = null;
        }

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
        if (outline != null) outline.enabled = false;

        currentSelection = this;
        CameraMovement.Instance?.FocusTo(splinePosition, transform);

        var controller = FindObjectOfType<PuzzleControlRoom2>();
        controller?.ActivatePuzzle(puzzleType);
    }

    public static void ClearSelection()
    {
        if (currentSelection != null && currentSelection.outline != null)
            currentSelection.outline.enabled = false;

        currentSelection = null;
    }
}
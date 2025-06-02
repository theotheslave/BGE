using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Attach this to any UI Image GameObject that you want to be draggable.
/// It implements IBeginDragHandler, IDragHandler, IEndDragHandler:
///  • OnBeginDrag: remembers its original parent & anchored position, then re‐parents to the root Canvas
///    (so it can move freely above everything else), and disables its own raycast target so drop targets
///    underneath can receive the drop event.
///  • OnDrag: moves the RectTransform to the mouse/finger position.
///  • OnEndDrag: restores its parent & original anchored position, and re‐enables raycast blocking.
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class DraggableImage : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Canvas canvas;            // the root Canvas this element lives under
    private RectTransform rectTransform;    // to move it around
    private CanvasGroup canvasGroup;      // to toggle blocksRaycasts

    private Transform originalParent;       // to return under
    private Vector2 originalAnchoredPos;   // to snap back

    private Image imageComponent;           // to fetch its sprite

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        // Ensure a CanvasGroup exists so we can turn off raycast blocking while dragging
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        imageComponent = GetComponent<Image>();
        if (imageComponent == null)
            Debug.LogError("DraggableImage: no Image component found on this GameObject.");

        // Find the root Canvas (walk up parents until you hit a Canvas)
        canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
            Debug.LogError("DraggableImage: must be placed under a Canvas.");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 1) Remember where we came from
        originalParent = transform.parent;
        originalAnchoredPos = rectTransform.anchoredPosition;

        // 2) Re-parent to root canvas so we render on top
        transform.SetParent(canvas.transform, true);

        // 3) Allow drop targets under this to receive raycasts
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Move the UI element to the pointer position
        rectTransform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 1) Re‐enable raycast blocking
        canvasGroup.blocksRaycasts = true;

        // 2) Restore parent & anchored position
        transform.SetParent(originalParent, true);
        rectTransform.anchoredPosition = originalAnchoredPos;
    }

    /// <summary>
    /// Public accessor so drop‐targets can read the sprite we carry.
    /// </summary>
    public Sprite GetSprite()
    {
        return imageComponent != null ? imageComponent.sprite : null;
    }
}
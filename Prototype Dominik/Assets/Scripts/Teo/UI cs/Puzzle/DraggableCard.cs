using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableCard : MonoBehaviour,
                              IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Assign the (invisible) Drag Layer here")]
    [SerializeField] private Transform dragLayer;     // set in Inspector!

    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Transform originalParent;
    private Vector2 originalAnchoredPos;
    private LayoutElement layoutElement;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
        layoutElement = GetComponent<LayoutElement>();

        // One-time fallback lookup
        if (dragLayer == null)
        {
            var obj = GameObject.Find("DragLayer");
            if (obj) dragLayer = obj.transform;
            else Debug.LogWarning("[DraggableCard] DragLayer missing - please assign!");
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        originalAnchoredPos = rectTransform.anchoredPosition;

        if (layoutElement) layoutElement.ignoreLayout = true;

        if (dragLayer) transform.SetParent(dragLayer, true);
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Move in Canvas-space, not world-space
        rectTransform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        // If we didn’t land on a FormulaSlot -> snap back
        bool droppedOnSlot = eventData.pointerEnter &&
                             eventData.pointerEnter.GetComponentInParent<FormulaSlot>();

        if (!droppedOnSlot)
            ReturnToOrigin();
    }

    /* ——— public so FormulaSlot can also reset when needed ——— */
    public void ReturnToOrigin()
    {
        transform.SetParent(originalParent, false);
        rectTransform.anchoredPosition = originalAnchoredPos;

        if (layoutElement) layoutElement.ignoreLayout = false;
    }
}
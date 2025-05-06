using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableCardRework : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Transform originalParent;
    public Vector3 originalLocalPosition;

    private Canvas rootCanvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        rootCanvas = GetComponentInParent<Canvas>().rootCanvas;
        canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        var layout = GetComponent<LayoutElement>();
        if (layout != null) layout.ignoreLayout = true;
        originalParent = transform.parent;
        originalLocalPosition = rectTransform.localPosition;
        transform.SetParent(rootCanvas.transform, true);
        transform.SetAsLastSibling();
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rootCanvas.transform as RectTransform,
            eventData.position,
            rootCanvas.worldCamera,
            out Vector2 localPoint);
        rectTransform.localPosition = localPoint;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        if (transform.parent == rootCanvas.transform) ReturnToOrigin();
        var layout = GetComponent<LayoutElement>();
        if (layout != null) layout.ignoreLayout = false;
    }

    public void ReturnToOrigin()
    {
        if (originalParent != null)
        {
            transform.SetParent(originalParent, false);
            rectTransform.localPosition = originalLocalPosition;
        }
        var layout = GetComponent<LayoutElement>();
        if (layout != null) layout.ignoreLayout = false;
    }
}

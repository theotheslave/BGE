using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class DraggableCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Transform originalParent;
    private Vector3 originalPosition;
    private LayoutElement layoutElement;
    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();

        layoutElement = GetComponent<LayoutElement>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        originalPosition = rectTransform.anchoredPosition;

        if (layoutElement != null)
            layoutElement.ignoreLayout = true;

        var dragLayer = GameObject.Find("DragLayer");
        if (dragLayer)
            transform.SetParent(dragLayer.transform, true);
        else
            Debug.LogWarning("DragLayer not found!");

        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        // If it’s not parented to a slot, return to original
        if (transform.parent == GameObject.Find("DragLayer").transform)
        {
            ReturnToOrigin();
        }
    }

    public void ReturnToOrigin()
    {
        transform.SetParent(originalParent, false);
        rectTransform.anchoredPosition = originalPosition;

        if (layoutElement != null)
            layoutElement.ignoreLayout = false;
    }
}
using UnityEngine.EventSystems;
using UnityEngine;

public class DraggableCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Transform originalParent;
    private Vector3 originalPosition;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        originalPosition = rectTransform.anchoredPosition;

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
    }
}
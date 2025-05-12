using UnityEngine.EventSystems;
using UnityEngine;

public class DraggableCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Transform originalParent;
    private Canvas dragCanvas;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;

        // 🔍 Find or assign the drag canvas (e.g., DragLayer)
        GameObject dragLayer = GameObject.Find("DragLayer");
        if (dragLayer != null)
        {
            transform.SetParent(dragLayer.transform, true);
        }
        else
        {
            Debug.LogWarning("DragLayer canvas not found!");
        }

        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        transform.SetParent(originalParent, false);
    }
}

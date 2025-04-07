using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [HideInInspector] public Transform originalParent;
    [HideInInspector] public Vector3 originalLocalPosition;

    public void OnBeginDrag(PointerEventData eventData)
    {
        var layout = GetComponent<LayoutElement>();
        if (layout != null)
        {
            layout.ignoreLayout = true;
        }

        originalParent = transform.parent;
        originalLocalPosition = GetComponent<RectTransform>().localPosition;

        // ✅ Fix: move to DragLayer instead of root
        Transform dragLayer = GameObject.Find("DragLayer")?.transform;
        if (dragLayer != null)
        {
            transform.SetParent(dragLayer, false);
            transform.SetAsLastSibling(); // Ensure it's on top
        }
        else
        {
            Debug.LogWarning("DragLayer not found in scene!");
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Drop logic handled in FormulaSlotView
    }
}
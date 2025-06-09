using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup), typeof(RectTransform))]
public class DraggableFormula : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    CanvasGroup _cg;
    RectTransform _rt;
    Transform _origParent;
    Vector2 _origPos;
    Vector2 _pointerOffset;

    void Awake()
    {
        _cg = GetComponent<CanvasGroup>();
        _rt = GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData e)
    {
        _origParent = transform.parent;
        _origPos = _rt.anchoredPosition;
        transform.SetParent(_rt.root, true);
        transform.SetAsLastSibling();
        _cg.blocksRaycasts = false;
        _pointerOffset = (Vector2)_rt.position - e.position;
    }

    public void OnDrag(PointerEventData e)
    {
        _rt.position = e.position + _pointerOffset;
    }

    public void OnEndDrag(PointerEventData e)
    {
        _cg.blocksRaycasts = true;
        transform.SetParent(_origParent, true);
        _rt.anchoredPosition = _origPos;
    }
}

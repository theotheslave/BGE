using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Text))]
public class DraggableText : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Canvas _canvas;
    private RectTransform _rectTransform;
    private CanvasGroup _canvasGroup;
    private Transform _originalParent;
    private Vector2 _originalAnchoredPos;
    private Text _textComponent;
    private Vector2 _pointerOffset;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();

        // Add (or get) a CanvasGroup so we can disable raycasts during drag
        _canvasGroup = GetComponent<CanvasGroup>();
        if (_canvasGroup == null)
            _canvasGroup = gameObject.AddComponent<CanvasGroup>();

        // Get the Text component (it must be on this same GameObject)
        _textComponent = GetComponent<Text>();
        if (_textComponent == null)
            Debug.LogError($"{name}: DraggableText requires a Text component.");

        // Find the parent Canvas
        _canvas = GetComponentInParent<Canvas>();
        if (_canvas == null)
            Debug.LogError($"{name}: DraggableText must be a child of a Canvas.");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_textComponent == null) return;

        // Remember original parent & anchored position so we can restore later
        _originalParent = transform.parent;
        _originalAnchoredPos = _rectTransform.anchoredPosition;

        // Reparent to root Canvas so it renders on top of everything else
        transform.SetParent(_canvas.transform, true);
        transform.SetAsLastSibling();

        // Disable raycast blocking so drop targets can receive OnDrop
        _canvasGroup.blocksRaycasts = false;

        // Compute offset between pointer and object to prevent “jumping”
        Vector2 pointerScreenPos = eventData.position;
        Vector2 objectScreenPos = _rectTransform.position;
        _pointerOffset = objectScreenPos - pointerScreenPos;

        Debug.Log($"{name}: OnBeginDrag, text = \"{_textComponent.text}\"");
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Move the object so that the pointer holds the same spot inside the text
        Vector2 newPos = eventData.position + _pointerOffset;
        _rectTransform.position = newPos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log($"{name}: OnEndDrag");

        // Re-enable raycast blocking
        _canvasGroup.blocksRaycasts = true;

        // Return to original parent & anchored position
        transform.SetParent(_originalParent, true);
        _rectTransform.anchoredPosition = _originalAnchoredPos;
    }

    // Provide the string so that the drop target can read it
    public string GetText()
    {
        return _textComponent != null ? _textComponent.text : null;
    }
}

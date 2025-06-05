using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class DroppableTextTarget : MonoBehaviour, IDropHandler
{
    public string variableName; // e.g. “P1”, “V1”, “P2”, or “V2”

    private Text _textComponent;

    private void Awake()
    {
        _textComponent = GetComponent<Text>();
        if (_textComponent == null)
            Debug.LogError($"{name}: DroppableTextTarget requires a Text component.");
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log($"[{name}] OnDrop called. pointerDrag = {eventData.pointerDrag?.name}");

        if (eventData.pointerDrag == null)
        {
            Debug.Log($"[{name}] OnDrop: pointerDrag is null → dropped outside a target");
            return;
        }

        // Look for DraggableText on the dragged object
        DraggableText draggable = eventData.pointerDrag.GetComponent<DraggableText>();
        if (draggable == null)
        {
            Debug.Log($"[{name}] OnDrop: {eventData.pointerDrag.name} does not have DraggableText.");
            return;
        }

        // Get the string from the dragged Text
        string newText = draggable.GetText();
        Debug.Log($"[{name}] OnDrop: retrieved text = \"{newText ?? "null"}\"");

        if (string.IsNullOrEmpty(newText))
        {
            Debug.Log($"[{name}] OnDrop: text is empty or null, leaving target unchanged.");
            return;
        }

        // Set our own Text to that string
        _textComponent.text = newText;
        Debug.Log($"[{name}] OnDrop: text successfully set → \"{newText}\"");

        // Notify the manager that “variableName” now holds newText
        FormulaManager.Instance.OnSlotUpdated(variableName, newText);
    }
}

using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;  // if you use TextMeshProUGUI
using UnityEngine.UI;

public class DroppableTextTarget : MonoBehaviour, IDropHandler
{
    [Tooltip("Must match P1, V1, P2, V2, etc.")]
    public string variableName;

    private Text uiText;
    private TMP_Text tmpText;

    private void Awake()
    {
        uiText = GetComponent<Text>();
        tmpText = GetComponent<TMP_Text>();

        if (uiText == null && tmpText == null)
            UnityEngine.Debug.LogError($"[{name}] needs a Text or TMP_Text!");
    }

    public void OnDrop(PointerEventData eventData)
    {
        var dragGO = eventData.pointerDrag;
        if (dragGO == null) return;

        // pull the string off the dragged object
        string dropped = null;
        if (dragGO.TryGetComponent<Text>(out var t1)) dropped = t1.text;
        else if (dragGO.TryGetComponent<TMP_Text>(out var t2)) dropped = t2.text;

        if (string.IsNullOrEmpty(dropped)) return;

        // **Overwrite** the slot's Text with *just* the dropped text
        if (uiText != null) uiText.text = dropped;
        if (tmpText != null) tmpText.text = dropped;

        // notify the manager of the raw dropped value
        FormulaManager.Instance.OnSlotUpdated(variableName, dropped);

        UnityEngine.Debug.Log($"Dropped '{dropped}' into {variableName}");
    }
}

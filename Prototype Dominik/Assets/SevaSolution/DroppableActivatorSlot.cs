using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class DroppableActivatorSlot : MonoBehaviour, IDropHandler
{
    [Tooltip("Which GameObjects should be activated when a formula is dropped here?")]
    public List<GameObject> objectsToActivate;

    public static readonly List<DroppableActivatorSlot> AllSlots = new List<DroppableActivatorSlot>();

    private void Awake()
    {
        AllSlots.Add(this);
    }

    private void OnDestroy()
    {
        AllSlots.Remove(this);
    }

    public void OnDrop(PointerEventData eventData)
    {
        // Only accept draggables tagged as "Formula"
        var formula = eventData.pointerDrag?.GetComponent<DraggableFormula>();
        if (formula == null)
        {
            UnityEngine.Debug.Log($"[{name}] OnDrop ignored: not a formula.");
            return;
        }

        UnityEngine.Debug.Log($"[{name}] Formula dropped: activating objects.");
        foreach (var go in objectsToActivate)
            if (go != null)
                go.SetActive(true);
    }

    public void ResetActivations()
    {
        foreach (var go in objectsToActivate)
            if (go != null)
                go.SetActive(false);
    }
}

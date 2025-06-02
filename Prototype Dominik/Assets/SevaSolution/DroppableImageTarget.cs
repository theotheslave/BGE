using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Attach this to any UI Image GameObject you want to act as a drop target.
/// Implements IDropHandler so that when a DraggableImage is dropped on it,
/// it grabs the dragged sprite and replaces its own Image.sprite with it.
/// </summary>
[RequireComponent(typeof(Image))]
public class DroppableImageTarget : MonoBehaviour, IDropHandler
{
    private Image imageComponent;

    private void Awake()
    {
        imageComponent = GetComponent<Image>();
        if (imageComponent == null)
            Debug.LogError("DroppableImageTarget: no Image component found on this GameObject.");
    }

    public void OnDrop(PointerEventData eventData)
    {
        // eventData.pointerDrag is the GameObject being dragged
        GameObject droppedObject = eventData.pointerDrag;
        if (droppedObject == null) return;

        // Check if it has a DraggableImage component
        DraggableImage draggable = droppedObject.GetComponent<DraggableImage>();
        if (draggable == null) return;

        // Get the sprite from the dragged image
        Sprite newSprite = draggable.GetSprite();
        if (newSprite == null) return;

        // Set our own Image to that sprite
        imageComponent.sprite = newSprite;

        // Optional: if you want the target to resize to the sprite’s native size:
        imageComponent.SetNativeSize();
    }
}


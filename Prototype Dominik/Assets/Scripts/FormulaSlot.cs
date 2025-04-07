using UnityEngine;

public class FormulaSlot : MonoBehaviour
{
    private Transform originalParent;
    private Vector3 originalLocalPosition;
    public string slotID;

    private FormulaCard placedCard;
    private GameObject placedCardObject;

    public void PlaceCard(FormulaCard card, GameObject cardObj)
    {
        var dragData = cardObj.GetComponent<DraggableCard>();
        if (dragData != null)
        {
            originalParent = dragData.originalParent;
            originalLocalPosition = dragData.originalLocalPosition;
        }

        placedCard = card;
        placedCardObject = cardObj;
    }

    public bool IsCorrect() => placedCard != null && placedCard.correctSlotID == slotID;
    public FormulaCard GetPlacedCard() => placedCard;

    public void ClearSlot()
    {
        
        placedCard = null;

        if (placedCardObject != null)
        {
            var dragData = placedCardObject.GetComponent<DraggableCard>();
            if (dragData != null && dragData.originalParent != null)
            {
                placedCardObject.transform.SetParent(dragData.originalParent, false);
                placedCardObject.GetComponent<RectTransform>().localPosition = dragData.originalLocalPosition;
                Debug.Log($"{gameObject.name} returned to  {placedCardObject.transform.parent.name}");
                placedCardObject.SetActive(true);

                // ✅ Update the drag reference AFTER returning to hand
                dragData.originalParent = placedCardObject.transform.parent;
                dragData.originalLocalPosition = placedCardObject.GetComponent<RectTransform>().localPosition;
            }
            else
            {
                placedCardObject.SetActive(false);
            }

            placedCardObject = null;
        }
    }
}
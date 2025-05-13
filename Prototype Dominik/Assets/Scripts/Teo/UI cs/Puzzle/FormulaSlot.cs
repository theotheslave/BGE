using UnityEngine;

public class FormulaSlot : MonoBehaviour
{
    public string slotID;
    private FormulaCard placedCard;

    public void PlaceCard(FormulaCard card)
    {
        placedCard = card;
    }

    public bool IsCorrect()
    {
        return placedCard != null && placedCard.correctSlotID == slotID;
    }

    public void ClearSlot()
    {
        if (placedCard != null)
        {
            var cardObj = GetComponentInChildren<FormulaCardView>()?.gameObject;
            var draggable = cardObj?.GetComponent<DraggableCard>();
            if (draggable != null)
            {
                draggable.ReturnToOrigin();
            }
        }

        placedCard = null;
    }
}
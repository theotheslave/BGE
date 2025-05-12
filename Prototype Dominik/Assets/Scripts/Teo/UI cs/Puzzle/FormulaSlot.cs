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
        placedCard = null;
    }
}
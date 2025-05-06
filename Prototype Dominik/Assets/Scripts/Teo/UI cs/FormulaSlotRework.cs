using UnityEngine;

public class FormulaSlotRework : MonoBehaviour
{
    public string slotID;
    private FormulaCardRework placedCardData;
    private GameObject placedCardObject;

    public void PlaceCard(FormulaCardRework cardData, GameObject cardObj)
    {
        placedCardData = cardData;
        placedCardObject = cardObj;
    }

    public FormulaCardRework GetPlacedCard() => placedCardData;
    public GameObject GetPlacedCardObject() => placedCardObject;

    public void ClearSlot()
    {
        placedCardObject?.GetComponent<DraggableCardRework>()?.ReturnToOrigin();
        placedCardData = null;
        placedCardObject = null;
    }
}
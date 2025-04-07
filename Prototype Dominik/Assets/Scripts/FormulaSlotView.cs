using UnityEngine;
using UnityEngine.EventSystems;

public class FormulaSlotView : MonoBehaviour, IDropHandler
{
    public FormulaSlot slotLogic;
    public Transform placedCardsContainer; 

    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedCard = eventData.pointerDrag;
        if (droppedCard == null) return;

        FormulaCardView cardView = droppedCard.GetComponent<FormulaCardView>();
        if (cardView == null || cardView.cardData == null) return;
        Debug.Log($"Dropped card {cardView.cardData.formulaID} into slot {slotLogic.slotID}");

        // Snap the card into the placedCards container instead
        droppedCard.transform.SetParent(placedCardsContainer, false);
        droppedCard.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        droppedCard.transform.SetAsLastSibling();

        var layout = droppedCard.GetComponent<UnityEngine.UI.LayoutElement>();
        if (layout != null)
        {
            layout.ignoreLayout = false;
        }

        
        slotLogic.PlaceCard(cardView.cardData, droppedCard);
    }
}

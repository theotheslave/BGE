using UnityEngine;
using UnityEngine.EventSystems;

public class FormulaSlotView : MonoBehaviour, IDropHandler
{
    public FormulaSlot logicSlot;

    public void OnDrop(PointerEventData eventData)
    {
        var cardView = eventData.pointerDrag?.GetComponent<FormulaCardView>();
        if (cardView != null && logicSlot != null)
        {
            logicSlot.PlaceCard(cardView.cardData);
            cardView.transform.SetParent(transform);
            cardView.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }
    }
}

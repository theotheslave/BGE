using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class FormulaSlotView : MonoBehaviour, IDropHandler
{
    public FormulaSlotRework formulaSlotLogic;

    void Awake() => formulaSlotLogic ??= GetComponent<FormulaSlotRework>();

    public void OnDrop(PointerEventData eventData)
    {
        var droppedCard = eventData.pointerDrag?.GetComponent<DraggableCardRework>();
        var cardView = eventData.pointerDrag?.GetComponent<FormulaCardViewRerwork>();

        if (droppedCard != null && cardView != null && formulaSlotLogic != null)
        {
            var existing = formulaSlotLogic.GetPlacedCardObject();
            formulaSlotLogic.ClearSlot();
            formulaSlotLogic.PlaceCard(cardView.cardData, droppedCard.gameObject);
            droppedCard.transform.SetParent(transform);
            droppedCard.GetComponent<RectTransform>().localPosition = Vector3.zero;
            existing?.GetComponent<DraggableCardRework>()?.ReturnToOrigin();
        }
        else
        {
            droppedCard?.ReturnToOrigin();
        }
    }
}
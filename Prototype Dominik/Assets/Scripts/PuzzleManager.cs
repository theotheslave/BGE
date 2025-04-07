using System.Linq;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public FormulaSlot[] slots;

    public int CalculateTotalValue(ActionType actionType)
    {
        int total = 0;
        foreach (var slot in slots)
        {
            var card = slot.GetPlacedCard();
            if (card != null && slot.IsCorrect())
            {
                Debug.Log($"Checking card {card.formulaID} in slot {slot.slotID}. Expected slot: {card.correctSlotID}");
                if ((actionType == ActionType.Attack && card.cardType == CardType.Attack) ||
                    (actionType == ActionType.Defend && card.cardType == CardType.Defense))
                {
                    total += card.value;
                }
            }
        }
        return total;
    }

    public int CalculateUniversalValue()
    {
        int total = 0;
        foreach (var slot in slots)
        {
            var card = slot.GetPlacedCard();
            if (card != null && slot.IsCorrect() && card.cardType == CardType.Universal)
            {
                total += card.value;
            }
        }
        return total;
    }

    public bool IsPuzzleSolvedCorrectly()
    {
        return slots.All(slot => slot.IsCorrect());
    }

    public void ResetPuzzle()
    {
        foreach (var slot in slots)
        {
            slot.ClearSlot();
        }
    }
}
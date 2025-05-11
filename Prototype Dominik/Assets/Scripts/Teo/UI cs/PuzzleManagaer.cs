using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.Events;

public class FormulaPuzzleController : MonoBehaviour
{
    public FormulaSlotRework[] slots;
    public UIManager uiManager;
    public UnityEvent onPuzzleSolved;
    public UnityEvent onPuzzleFailed;
    public Transform cardHandArea;
    public GameObject cardPrefab;

    private Dictionary<string, FormulaCardRework> correctAssignments = new();

    public void LoadPuzzle(FormulaPuzzleDefinition puzzle)
    {
        ResetPuzzle();
        correctAssignments = puzzle.cardAssignments.ToDictionary(x => x.targetSlotID, x => x.card);

        foreach (var assignment in puzzle.cardAssignments)
        {
            GameObject newCard = Instantiate(cardPrefab, cardHandArea);
            var view = newCard.GetComponent<FormulaCardViewRerwork>();
            view.cardData = assignment.card;
            view.ApplyCardData();
        }
    }

    public bool IsPuzzleSolvedCorrectly() =>
        slots.All(slot => slot.GetPlacedCard() != null && correctAssignments.TryGetValue(slot.slotID, out var expected) && slot.GetPlacedCard() == expected);

    public void CheckPuzzleState()
    {
        if (IsPuzzleSolvedCorrectly())
        {
            uiManager?.ShowPuzzleFeedback("Formula Correct!");
            onPuzzleSolved?.Invoke();
            UIManager.Instance?.TogglePanel(UIManager.Instance.formulaWindowPanel);
        }
        else
        {
            uiManager?.ShowPuzzleFeedback("Incorrect placement. Try again.");
            onPuzzleFailed?.Invoke();
        }
    }

    public void ResetPuzzle()
    {
        foreach (var slot in slots) slot?.ClearSlot();
        foreach (Transform child in cardHandArea) Destroy(child.gameObject);
    }
}

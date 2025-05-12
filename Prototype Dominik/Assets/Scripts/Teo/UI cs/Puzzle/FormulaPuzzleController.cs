using UnityEngine;
using System.Linq;

public class FormulaPuzzleController : MonoBehaviour
{
    public FormulaSlot[] slots;
    public string puzzleID;

    public bool IsPuzzleSolvedCorrectly()
    {
        return slots.All(slot => slot != null && slot.IsCorrect());
    }

    public void ResetPuzzle()
    {
        foreach (var slot in slots)
        {
            slot?.ClearSlot();
        }
    }

    public string GetPuzzleID() => puzzleID;
}

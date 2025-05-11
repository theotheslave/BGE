using UnityEngine;
using UnityEngine.UI;

public class PuzzleSceneManager : MonoBehaviour
{
    [Header("Puzzle Scene UI")]
    public Button checkButton;
    public Button resetButton;
    public Button showLearnedButton;
    public GameObject puzzlePanel;

    [Header("Puzzle Info")]
    public string formulaIDToUnlock = "Puzzle_Formula_001";

    void Start()
    {
        checkButton?.onClick.AddListener(ValidatePuzzle);
        resetButton?.onClick.AddListener(ResetPuzzle); // placeholder for real logic
        showLearnedButton?.onClick.AddListener(() => UIManager.Instance?.TogglePanel(UIManager.Instance.learnedFormulasPanel));
    }

    void ValidatePuzzle()
    {
        Debug.Log("Puzzle validated (simulated).");

        // ✅ Assume puzzle is correct
        UIManager.Instance?.HandlePuzzleSolved(formulaIDToUnlock);

        // Hide puzzle if needed
        puzzlePanel?.SetActive(false);
    }

    void ResetPuzzle()
    {
        Debug.Log("Puzzle reset (not implemented yet).");
        // Add real puzzle reset logic here
    }
}

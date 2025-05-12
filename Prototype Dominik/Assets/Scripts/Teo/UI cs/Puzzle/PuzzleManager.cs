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
    public FormulaPuzzleController puzzleController;

    void Start()
    {
        checkButton?.onClick.AddListener(ValidatePuzzle);
        resetButton?.onClick.AddListener(() => puzzleController?.ResetPuzzle());
        showLearnedButton?.onClick.AddListener(() => UIManager.Instance?.TogglePanel(UIManager.Instance.learnedFormulasPanel));
    }

    void ValidatePuzzle()
    {
        if (puzzleController == null)
        {
            Debug.LogError("PuzzleController not assigned.");
            return;
        }

        if (puzzleController.IsPuzzleSolvedCorrectly())
        {
            UIManager.Instance?.HandlePuzzleSolved(puzzleController.GetPuzzleID());
            puzzlePanel?.SetActive(false);
        }
        else
        {
            UIManager.Instance?.ShowPuzzleFeedback("Incorrect formula.");
        }
    }
}
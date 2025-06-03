using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("General UI")]
    public GameObject learnedFormulasPanel;
    public TextMeshProUGUI feedbackText;
    [SerializeField] private GameObject puzzleMachine;
    [SerializeField] private TextMeshProUGUI levelNameText;

    void Awake()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        levelNameText.text = $"Lvl#: {sceneName}";
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        learnedFormulasPanel?.SetActive(false);
    }

    public void TogglePanel(GameObject panel, bool? forceState = null)
    {
        if (panel != null)
        {
            bool newState = forceState.HasValue ? forceState.Value : !panel.activeSelf;
            panel.SetActive(newState);
        }
    }

    public void ShowPuzzleFeedback(string message)
    {
        if (feedbackText != null)
        {
            feedbackText.text = message;
            feedbackText.gameObject.SetActive(true);
        }
    }

    public void HandlePuzzleSolved(string formulaID)
    {
        FormulaUnlockManager.Instance.UnlockFormula(formulaID);
        ShowPuzzleFeedback($"Unlocked: {formulaID}");

        var ui = learnedFormulasPanel?.GetComponent<LearnedFormulasUI>();
        ui?.RefreshList();
    }

    public void EnableMachine()
    {
        if (puzzleMachine != null)
        {
            puzzleMachine.SetActive(!puzzleMachine.activeSelf);
        }
    }
}

using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("General UI")]
    public GameObject formulaWindowPanel;
    public GameObject learnedFormulasPanel;
    public TextMeshProUGUI feedbackText;

    void Awake()
    {
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
        formulaWindowPanel?.SetActive(false);
        learnedFormulasPanel?.SetActive(false);
    }

    public void TogglePanel(GameObject panel)
    {
        if (panel != null)
        {
            bool isActive = panel.activeSelf;
            panel.SetActive(!isActive);
        }
    }

    public void ShowPuzzleFeedback(string message)
    {
        Debug.Log("UI Feedback: " + message);
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

        if (learnedFormulasPanel != null && learnedFormulasPanel.activeSelf)
        {
            var ui = learnedFormulasPanel.GetComponent<LearnedFormulasUI>();
            ui?.RefreshList();
        }
    }
}

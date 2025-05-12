using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Formula Detail View")]
    public Image formulaImage;
    public TextMeshProUGUI formulaNameText;
    public TextMeshProUGUI formulaDescriptionText;

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

    public void ShowFormulaDetails(string formulaID)
    {
        FormulaCard card = Resources.Load<FormulaCard>($"FormulaCards/{formulaID}");
        if (card != null)
        {
            if (formulaWindowPanel != null) formulaWindowPanel.SetActive(true);
            if (formulaImage != null) formulaImage.sprite = card.formulaSprite;
            if (formulaNameText != null) formulaNameText.text = card.formulaID;
            if (formulaDescriptionText != null) formulaDescriptionText.text = card.description;
        }
        else
        {
            Debug.LogWarning($"FormulaCard with ID '{formulaID}' not found in Resources/FormulaCards/");
        }
    }
        
}
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public GameObject formulaWindowPanel;
    public GameObject learnedFormulasPanel;
    public Text feedbackText;

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

    public void ToggleFormulaWindow() => formulaWindowPanel?.SetActive(!formulaWindowPanel.activeSelf);
    public void CloseFormulaWindow() => formulaWindowPanel?.SetActive(false);
    public void ToggleLearnedFormulasPanel() => learnedFormulasPanel?.SetActive(!learnedFormulasPanel.activeSelf);

    public void ShowPuzzleFeedback(string message)
    {
        Debug.Log("UI Feedback: " + message);
        if (feedbackText != null)
        {
            feedbackText.text = message;
            feedbackText.gameObject.SetActive(true);
        }
    }
}

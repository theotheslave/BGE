using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; set; }

    [Header("General UI")]
    public TextMeshProUGUI feedbackText;
    [SerializeField] private GameObject puzzleMachine;
    [SerializeField] private TextMeshProUGUI levelNameText;
    [SerializeField] private GameObject UiForMachine;
    [SerializeField] private GameObject ProblemText;


    public bool IsPuzzleCompleted { get; private set; } = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        if (gameObject.scene.name == "DontDestroyOnLoad") // It's been carried across
        {
            Debug.LogWarning("Destroying UIManager that was preserved by mistake.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        RebindIfNeeded();
    }

    public void RebindIfNeeded()
    {
        if (UiForMachine != null) UiForMachine.SetActive(false);

        if (levelNameText != null)
        {
            string sceneName = SceneManager.GetActiveScene().name;
            levelNameText.text = $"Lvl#: {sceneName}";
        }

        // Optionally: add null checks and fallbacks for dynamic assignment
        // if (feedbackText == null) feedbackText = GameObject.Find("FeedbackText")?.GetComponent<TextMeshProUGUI>();
    }

    public void TogglePanel(GameObject panel, bool? forceState = null)
    {
        if (panel != null)
        {
            bool newState = forceState.HasValue ? forceState.Value : !panel.activeSelf;
            panel.SetActive(newState);
            Debug.Log("TogglePanel called by " + this.name + " at " + Time.time);
        }
    }

    public void MarkPuzzleComplete()
    {
        IsPuzzleCompleted = true;
    }

    public void ShowPuzzleFeedback(string message)
    {
        if (feedbackText != null)
        {
            feedbackText.text = message;
            feedbackText.gameObject.SetActive(true);
        }
    }

    public void EnableMachine()
    {
        if (puzzleMachine != null)
        {
            puzzleMachine.SetActive(!puzzleMachine.activeSelf);
        }
        if (UiForMachine != null)
        {
            UiForMachine.SetActive(!UiForMachine.activeSelf);
        }
        if (ProblemText != null)
        {
            ProblemText.SetActive(!ProblemText.activeSelf);
        }
    }
}

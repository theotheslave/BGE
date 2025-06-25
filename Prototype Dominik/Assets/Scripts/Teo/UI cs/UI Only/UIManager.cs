using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; set; }

    [Header("General UI")]
    public TextMeshProUGUI feedbackText;
    [SerializeField] private GameObject puzzleMachine;
    [SerializeField] private TextMeshProUGUI levelNameText;
    [SerializeField] private GameObject UiForMachine;

    public bool IsPuzzleCompleted { get; private set; } = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        // Optional: persist if you want UIManager to live across scenes
        // DontDestroyOnLoad(gameObject); // <- ONLY if you want that

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
    }
}

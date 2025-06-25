using UnityEngine;

public class GogglesManagerRoom2 : MonoBehaviour
{
    public static GogglesManagerRoom2 Instance;

    [Header("Puzzle Groups")]
    public GameObject isochoricPuzzleGroup;
    public GameObject isothermalPuzzleGroup;

    [Header("Puzzle Panels")]
    public GameObject isochoricPanel;
    public GameObject isothermalPanel;

    private GameObject activeGroup;
    private GameObject activePanel;
    private bool showingUI = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void SetActivePuzzle(SelectableObjectRoom2.PuzzleType type)
    {
        // Reset current
        isochoricPuzzleGroup?.SetActive(true);
        isothermalPuzzleGroup?.SetActive(true);
        isochoricPanel?.SetActive(false);
        isothermalPanel?.SetActive(false);

        showingUI = false;

        switch (type)
        {
            case SelectableObjectRoom2.PuzzleType.Isochoric:
                activeGroup = isochoricPuzzleGroup;
                activePanel = isochoricPanel;
                break;
            case SelectableObjectRoom2.PuzzleType.Isothermal:
                activeGroup = isothermalPuzzleGroup;
                activePanel = isothermalPanel;
                break;
            default:
                activeGroup = null;
                activePanel = null;
                break;
        }
    }

    public void TogglePuzzleUI()
    {
        if (activeGroup == null || activePanel == null) return;

        showingUI = !showingUI;

        activeGroup.SetActive(!showingUI); // Hide machine body
        activePanel.SetActive(showingUI);  // Show panel
    }

    public void ForceClose()
    {
        if (activeGroup != null) activeGroup.SetActive(true); // Re-enable the puzzle shell
        if (activePanel != null) activePanel.SetActive(false); // Hide the UI
        showingUI = false;
    }
}

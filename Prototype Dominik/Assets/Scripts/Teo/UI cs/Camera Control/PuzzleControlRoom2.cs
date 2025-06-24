using UnityEngine;

public class PuzzleControlRoom2 : MonoBehaviour
{
    [Header("Puzzle Logic")]
    public IsochoricMinigame isochoricPuzzle;
    public IsothermalMinigameFinal isothermalPuzzle;
    public SceneManagerDoor finalDoor;
    [SerializeField] private GameObject isothermalPuzzleGroup;
    [Header("Puzzle Panels")]
    [SerializeField] private GameObject isochoricPanel;
    [SerializeField] private GameObject isothermalPanel;

    private bool isochoricDone = false;
    private bool isothermalDone = false;

    void Start()
    {
        if (isochoricPuzzle != null)
            isochoricPuzzle.OnPuzzleComplete += HandleIsochoricComplete;

        if (isothermalPuzzle != null)
            isothermalPuzzle.OnPuzzleComplete += HandleIsothermalComplete;

        isochoricPanel?.SetActive(false);
        isothermalPanel?.SetActive(false);

        // Lock second puzzle at the start
        if (isothermalPuzzleGroup != null && isothermalPuzzleGroup.TryGetComponent<Collider>(out var col))
            col.enabled = false;
    }

    void HandleIsochoricComplete()
    {
        isochoricDone = true;
        Debug.Log("Isochoric complete");

        CameraMovement.Instance?.ReturnToStart();
        CameraMovement.Instance?.FocusToGogglesView();

        // Unlock second puzzle interaction
        if (isothermalPuzzleGroup != null && isothermalPuzzleGroup.TryGetComponent<Collider>(out var col))
            col.enabled = true;
    }

    void HandleIsothermalComplete()
    {
        isothermalDone = true;
        Debug.Log("Isothermal complete");
        CameraMovement.Instance?.ReturnToStart();
        finalDoor?.UnlockDoor();
    }

    public void ActivatePuzzle(SelectableObjectRoom2.PuzzleType type)
    {
        if (type == SelectableObjectRoom2.PuzzleType.Isochoric)
        {
            isochoricPanel?.SetActive(true);
            isothermalPanel?.SetActive(false);
            GogglesManagerRoom2.Instance?.SetActivePuzzle(type);
        }
        else if (type == SelectableObjectRoom2.PuzzleType.Isothermal)
        {
            isochoricPanel?.SetActive(false);
            isothermalPanel?.SetActive(true);
            GogglesManagerRoom2.Instance?.SetActivePuzzle(type);
        }
    }

    public void ReturnToHub()
    {
        Debug.Log("ReturnToHub called");
        GogglesManagerRoom2.Instance?.ForceClose();
        isochoricPanel?.SetActive(false);
        isothermalPanel?.SetActive(false);
        CameraMovement.Instance?.ReturnToStart();
    }
}

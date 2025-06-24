using UnityEngine;

public class PuzzleControlRoom2 : MonoBehaviour
{
    [Header("Puzzle Logic")]
    public IsochoricMinigame isochoricPuzzle;
    public IsothermalMinigameFinal isothermalPuzzle;
    public SceneManagerDoor finalDoor;

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
    }

    void HandleIsochoricComplete()
    {
        isochoricDone = true;
        Debug.Log("Isochoric complete");
        CameraMovement.Instance?.ReturnToStart();
        CameraMovement.Instance?.FocusGoggles();
    }

    void HandleIsothermalComplete()
    {
        isothermalDone = true;
        Debug.Log("Isothermal complete");
        CameraMovement.Instance?.ReturnToStart();
        if (finalDoor != null)
            finalDoor.UnlockDoor();
    }

    public void ActivatePuzzle(SelectableObject.PuzzleType type)
    {
        if (type == SelectableObject.PuzzleType.Isochoric)
        {
            isochoricPanel?.SetActive(true);
            isothermalPanel?.SetActive(false);
        }
        else if (type == SelectableObject.PuzzleType.Isothermal)
        {
            isothermalPanel?.SetActive(true);
            isochoricPanel?.SetActive(false);
        }
    }

    public void ReturnToHub()
    {
        isochoricPanel?.SetActive(false);
        isothermalPanel?.SetActive(false);
        CameraMovement.Instance?.ReturnToStart();
    }
}

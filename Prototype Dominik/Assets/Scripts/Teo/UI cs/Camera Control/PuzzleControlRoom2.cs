using UnityEngine;

public class PuzzleControlRoom2 : MonoBehaviour
{
    public IsochoricMinigame isochoricPuzzle;
    public IsothermalMinigameFinal isothermalPuzzle;
    public SceneManagerDoor finalDoor; // Optional

    private bool isochoricDone = false;
    private bool isothermalDone = false;

    void Start()
    {
        isochoricPuzzle.OnPuzzleComplete += HandleIsochoricComplete;
        isothermalPuzzle.OnPuzzleComplete += HandleIsothermalComplete;
    }

    void HandleIsochoricComplete()
    {
        isochoricDone = true;
        Debug.Log("Isochoric complete");

        CameraMovement.Instance?.ReturnToStart(); 
        CameraMovement.Instance?.FocusGoggles(); // focus on next puzzle
    }

    void HandleIsothermalComplete()
    {
        isothermalDone = true;
        Debug.Log("Isothermal complete");

        CameraMovement.Instance?.ReturnToStart(); // or focus on a door, ending area, etc.

        if (finalDoor != null)
            finalDoor.UnlockDoor(); // Allow scene transition
    }
}

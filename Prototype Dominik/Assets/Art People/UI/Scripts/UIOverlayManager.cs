using UnityEngine;

public class UIOverlayManager : MonoBehaviour
{
    [Header("Overlay Canvases")]
    public GameObject overlaySystem;
    public GameObject overlayData;

    [SerializeField] private GameObject notebookUI;


    private bool overlayActive = false;
    private bool notebookOpen = false;

    public void ToggleOverlay()
    {
        overlayActive = !overlayActive;
        overlaySystem.SetActive(overlayActive);
        overlayData.SetActive(overlayActive);
    }


    public void OpenNotebook()
    {
        notebookOpen = !notebookOpen;
        notebookUI.SetActive(notebookOpen);
    }

}
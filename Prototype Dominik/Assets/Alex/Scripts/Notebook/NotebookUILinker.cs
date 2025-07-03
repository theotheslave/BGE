using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NotebookUILinker : MonoBehaviour
{
    public NotebookManager notebookManager;
    public GameObject notebookUI;
    public Button toggleButton;
    public Transform titleContainer;
    public TextMeshProUGUI contentDisplay;
    public GameObject noteTitlePrefab;

    private void Start()
    {
        if (notebookManager != null)
        {
            notebookManager.notebookUI = notebookUI;
            notebookManager.toggleButton = toggleButton;
            notebookManager.titleContainer = titleContainer;
            notebookManager.contentDisplay = contentDisplay;
            notebookManager.noteTitlePrefab = noteTitlePrefab;

            notebookManager.TryConnectUI();
        }
    }
}
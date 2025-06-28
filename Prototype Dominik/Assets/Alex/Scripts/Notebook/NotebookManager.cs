using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotebookManager : MonoBehaviour
{
    public static NotebookManager Instance { get; private set; }

    public List<Note> allNotes = new List<Note>();
    public GameObject noteTitlePrefab;
    public Transform titleContainer;
    public TMPro.TextMeshProUGUI contentDisplay;

    public GameObject notebookUI;
    public Button toggleButton; 

    private bool isOpen = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        if (toggleButton != null)
        {
            toggleButton.onClick.AddListener(ToggleNotebook);
        }
        else
        {
            Debug.LogWarning("Toggle Button is not assigned in NotebookManager.");
        }
    }
    public void ReconnectUI(GameObject newNotebookUI)
    {
        this.notebookUI = newNotebookUI;

        titleContainer = notebookUI.transform.Find("LeftPanel/TitleContainer").transform;
        contentDisplay = notebookUI.transform.Find("RightPanel/NoteContentText").GetComponent<TMPro.TextMeshProUGUI>();
        toggleButton = notebookUI.transform.Find("ToggleButton").GetComponent<UnityEngine.UI.Button>();

        toggleButton.onClick.RemoveAllListeners();
        toggleButton.onClick.AddListener(ToggleNotebook);

        foreach (Transform child in titleContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (Note note in allNotes)
        {
            CreateNoteButton(note);
        }

        notebookUI.SetActive(false);
    }

    public void ToggleNotebook()
    {
        isOpen = !isOpen;
        notebookUI.SetActive(isOpen);
    }

    public void AddNote(Note note)
    {
        if (allNotes.Exists(n => n.title == note.title))
            return;

        allNotes.Add(note);
        CreateNoteButton(note);
    }

    private void CreateNoteButton(Note note)
    {
        GameObject btn = Instantiate(noteTitlePrefab, titleContainer);
        btn.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = note.title;
        btn.GetComponent<Button>().onClick.AddListener(() =>
        {
            contentDisplay.text = note.content;
        });
    }
}
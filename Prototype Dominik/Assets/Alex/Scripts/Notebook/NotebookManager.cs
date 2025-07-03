using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NotebookManager : MonoBehaviour
{
    public static List<Note> GlobalNotes = new List<Note>();

    [Header("UI References")]
    public GameObject notebookUI;
    public Button toggleButton;
    public Transform titleContainer;
    public TextMeshProUGUI contentDisplay;
    public GameObject noteTitlePrefab;

    private bool isOpen = false;

    private void Start()
    {
        TryConnectUI();
    }

    public void TryConnectUI()
    {
        if (notebookUI == null || toggleButton == null || titleContainer == null || contentDisplay == null || noteTitlePrefab == null)
        {
            Debug.LogWarning("Notebook UI references not assigned.");
            return;
        }

        toggleButton.onClick.RemoveAllListeners();
        toggleButton.onClick.AddListener(ToggleNotebook);

        notebookUI.SetActive(false);
        contentDisplay.text = "";

        RefreshNoteButtons();
    }

    public void ToggleNotebook()
    {
        isOpen = !isOpen;
        notebookUI.SetActive(isOpen);
        if (!isOpen) contentDisplay.text = "";
    }

    public void AddNote(Note note)
    {
        if (GlobalNotes.Exists(n => n.title == note.title))
            return;

        GlobalNotes.Add(note);
        CreateNoteButton(note);
    }

    private void RefreshNoteButtons()
    {
        foreach (Transform child in titleContainer)
            Destroy(child.gameObject);

        foreach (Note note in GlobalNotes)
            CreateNoteButton(note);
    }

    private void CreateNoteButton(Note note)
    {
        GameObject btn = Instantiate(noteTitlePrefab, titleContainer);
        btn.GetComponentInChildren<TextMeshProUGUI>().text = note.title;
        btn.GetComponent<Button>().onClick.AddListener(() =>
        {
            contentDisplay.text = note.content;
        });
    }
}
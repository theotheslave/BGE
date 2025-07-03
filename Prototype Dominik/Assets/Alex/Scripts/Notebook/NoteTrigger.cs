using UnityEngine;

public class NoteTrigger : MonoBehaviour
{
    public string noteTitle;
    [TextArea] public string noteContent;
    private bool noteGiven = false;

    private void OnMouseDown()
    {
        if (noteGiven) return;

        Note newNote = new Note { title = noteTitle, content = noteContent };

        NotebookManager manager = FindObjectOfType<NotebookManager>();
        if (manager != null)
        {
            manager.AddNote(newNote);
            noteGiven = true;
        }
    }
}
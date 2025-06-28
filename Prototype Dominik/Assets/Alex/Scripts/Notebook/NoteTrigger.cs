using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class NoteTrigger : MonoBehaviour
{
    public string noteTitle;
    [TextArea]
    public string noteContent;
    private bool noteGiven = false;

    private void OnMouseDown()
    {
        if (!noteGiven)
        {
            Note newNote = new Note { title = noteTitle, content = noteContent };
            NotebookManager.Instance.AddNote(newNote);
            noteGiven = true;
        }
    }
}
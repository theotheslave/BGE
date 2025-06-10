using UnityEngine;

public class NoteTrigger : MonoBehaviour
{
    [TextArea] public string noteContent;
    public string noteTitle;
    public string category = "General";

    private bool triggered = false;

    public void TriggerNote()
    {
        if (!triggered)
        {
            NotebookManager.Instance.UnlockNote(category, noteTitle, noteContent);
            triggered = true;
        }
    }
}

using System.Collections.Generic;

[System.Serializable]
public class NotebookEntry
{
    public string title;
    [UnityEngine.TextArea(4, 20)]
    public string content;

    [System.Serializable]
    public class NotebookCategory
    {
        public string categoryName;
        public List<NotebookEntry> entries = new List<NotebookEntry>();
    }
}
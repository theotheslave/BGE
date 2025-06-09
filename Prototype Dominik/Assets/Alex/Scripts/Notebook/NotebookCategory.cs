using System.Collections.Generic;

[System.Serializable]
public class NotebookCategory
{
    public string categoryName;
    public List<NotebookEntry> entries = new List<NotebookEntry>();
}
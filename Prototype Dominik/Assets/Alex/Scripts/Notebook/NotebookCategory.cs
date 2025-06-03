using System;
using System.Collections.Generic;

[Serializable]
public class NotebookCategory
{
    public string id;
    public string title;
    public List<NotebookEntry> entries = new List<NotebookEntry>();
}

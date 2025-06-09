using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class NotebookManager : MonoBehaviour
{
    public static NotebookManager Instance;

    private Dictionary<string, NotebookCategory> unlockedCategories = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else Destroy(gameObject);
    }

    public void UnlockNote(string category, string title, string content)
    {
        if (!unlockedCategories.ContainsKey(category))
        {
            var newCat = new NotebookCategory { categoryName = category };
            unlockedCategories[category] = newCat;
        }

        var cat = unlockedCategories[category];
        if (!cat.entries.Any(e => e.title == title))
        {
            cat.entries.Add(new NotebookEntry { title = title, content = content });
        }

        NotebookUI.Instance?.RefreshNotebook(unlockedCategories.Values.ToList());
    }
}

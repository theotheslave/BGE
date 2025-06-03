using System.Collections.Generic;
using UnityEngine;

public class NotebookManager : MonoBehaviour
{
    public static NotebookManager Instance;

    [SerializeField] private NotebookUI notebookUI;

    private Dictionary<string, NotebookCategory> categories = new();

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void AddEntry(string categoryId, string categoryTitle, NotebookEntry entry)
    {
        if (!categories.ContainsKey(categoryId))
        {
            categories[categoryId] = new NotebookCategory
            {
                id = categoryId,
                title = categoryTitle
            };
        }

        var category = categories[categoryId];

        if (!category.entries.Exists(e => e.id == entry.id))
        {
            category.entries.Add(entry);
            Debug.Log($"New notebook entry unlocked: {entry.title}");
            notebookUI?.Refresh(categories);
        }
    }

    public Dictionary<string, NotebookCategory> GetAllData()
    {
        return categories;
    }
}

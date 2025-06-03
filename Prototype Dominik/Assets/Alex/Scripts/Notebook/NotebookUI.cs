using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class NotebookUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform categoryContainer;     
    [SerializeField] private Transform entryContainer;     
    [SerializeField] private GameObject categoryButtonPrefab; 
    [SerializeField] private GameObject entryButtonPrefab;
    [SerializeField] private TextMeshProUGUI entryViewerText;
    [SerializeField] private GameObject notebookPanel;

    private Dictionary<string, NotebookCategory> currentData;

    private void Start()
    {
        notebookPanel.SetActive(false);
    }

    public void ToggleNotebook()
    {
        notebookPanel.SetActive(!notebookPanel.activeSelf);
        if (notebookPanel.activeSelf && currentData != null)
            Refresh(currentData);
    }

    public void Refresh(Dictionary<string, NotebookCategory> categories)
    {
        currentData = categories;
        ClearContainer(categoryContainer);
        ClearContainer(entryContainer);
        entryViewerText.text = "";

        foreach (var cat in categories.Values)
        {
            GameObject catButtonObj = Instantiate(categoryButtonPrefab, categoryContainer);
            var text = catButtonObj.GetComponentInChildren<TextMeshProUGUI>();
            text.text = cat.title;

            var button = catButtonObj.GetComponent<Button>();
            button.onClick.AddListener(() => ShowEntries(cat));
        }
    }

    void ShowEntries(NotebookCategory category)
    {
        ClearContainer(entryContainer);
        entryViewerText.text = "";

        foreach (var entry in category.entries)
        {
            GameObject entryButtonObj = Instantiate(entryButtonPrefab, entryContainer);
            var text = entryButtonObj.GetComponentInChildren<TextMeshProUGUI>();
            text.text = entry.title;

            var button = entryButtonObj.GetComponent<Button>();
            button.onClick.AddListener(() => ShowEntry(entry));
        }
    }

    void ShowEntry(NotebookEntry entry)
    {
        entryViewerText.text = entry.content;
    }

    void ClearContainer(Transform container)
    {
        foreach (Transform child in container)
            Destroy(child.gameObject);
    }
}

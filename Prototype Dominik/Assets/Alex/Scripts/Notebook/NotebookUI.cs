using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;

public class NotebookUI : MonoBehaviour
{
    public static NotebookUI Instance;

    [SerializeField] private GameObject categoryButtonPrefab;
    [SerializeField] private Transform categoryListParent;
    [SerializeField] private TextMeshProUGUI noteContentText;
    [SerializeField] private GameObject notebookRoot;

    private void Awake()
    {
        Instance = this;
        notebookRoot.SetActive(false);
    }

    public void ToggleNotebook()
    {
        notebookRoot.SetActive(!notebookRoot.activeSelf);
    }

    public void RefreshNotebook(List<NotebookCategory> categories)
    {
        foreach (Transform child in categoryListParent)
        {
            Destroy(child.gameObject);
        }

        foreach (var cat in categories)
        {
            foreach (var entry in cat.entries)
            {
                GameObject buttonGO = Instantiate(categoryButtonPrefab, categoryListParent);
                buttonGO.GetComponentInChildren<TextMeshProUGUI>().text = "• " + entry.title;
                buttonGO.GetComponent<Button>().onClick.AddListener(() =>
                {
                    noteContentText.text = entry.content;
                });
            }
        }
    }
}





using UnityEngine;

public class NotebookUILinker : MonoBehaviour
{
    public GameObject notebookUIPrefab;

    void Start()
    {
        if (NotebookManager.Instance != null)
        {
            NotebookManager.Instance.ReconnectUI(notebookUIPrefab);
        }
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

public class SevasSceneChange : MonoBehaviour
{
    [Tooltip("Name of the scene to load when this object is clicked.")]
    public string sceneToLoad = "SceneNameHere"; // Set this in the Inspector

    void Start()
    {
        // Make sure the mouse cursor is visible and not locked
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void OnMouseDown()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            UnityEngine.Debug.Log("Object clicked, loading scene: " + sceneToLoad);
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            UnityEngine.Debug.LogWarning("Scene name is empty. Please assign a scene name in the Inspector.");
        }
    }
}

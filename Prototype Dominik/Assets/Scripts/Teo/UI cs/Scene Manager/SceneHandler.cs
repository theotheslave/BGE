using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadHandler : MonoBehaviour
{
    void Awake()
    {
        if (FindObjectsByType<SceneLoadHandler>(FindObjectsSortMode.None).Length > 1)
        {
            Destroy(gameObject); 
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"SceneLoadHandler: Scene loaded -> {scene.name}");

        var ui = FindFirstObjectByType<UIManager>();
        if (ui != null)
        {
            UIManager.Instance = ui;
            ui.RebindIfNeeded(); // must exist
        }

        var cam = FindFirstObjectByType<CameraMovement>();
        if (cam != null)
        {
            CameraMovement.Instance = cam;
            cam.InitializeCamera(); // optional, but useful
        }
    }
}

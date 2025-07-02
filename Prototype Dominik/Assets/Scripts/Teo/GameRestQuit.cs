using UnityEngine;
using System.Diagnostics;
using System.IO;
public class GameRestQuit : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitGame();
        }
    }

    void RestartGame()
    {
#if UNITY_EDITOR
       
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
#else
        // In build: restart the .exe
        string exePath = Process.GetCurrentProcess().MainModule.FileName;
        Process.Start(exePath);
        Application.Quit();
#endif
    }

    void QuitGame()
    {
#if UNITY_EDITOR
       
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class FadeToBlack : MonoBehaviour
{
    public static FadeToBlack Instance;

    [SerializeField] private CanvasGroup fadeCanvasGroup;
    [SerializeField] private float fadeDuration = 1f;

    void Awake()
    {
        fadeCanvasGroup.alpha = 0f; // Force full black
        fadeCanvasGroup.blocksRaycasts = false;

        if (TryGetComponent<Image>(out var img))
            img.raycastTarget = true;

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
       
    }
    void Start()
    {
        StartCoroutine(FadeInFromBlack());
    }
    private IEnumerator FadeInFromBlack()
    {
        fadeCanvasGroup.alpha = 1f; 
        fadeCanvasGroup.blocksRaycasts = true; 

        yield return new WaitForSeconds(0.5f); 

        yield return StartCoroutine(Fade(0f)); 
    }

    public void FadeToScene(int sceneIndex)
    {
        StartCoroutine(FadeAndSwitch(sceneIndex));
    }

    private IEnumerator FadeAndSwitch(int sceneIndex)
    {
        yield return StartCoroutine(Fade(1f)); // Fade to black
        SceneManager.LoadScene(sceneIndex);
        yield return null; // Give scene a frame to load
        yield return StartCoroutine(Fade(0f)); // Fade back in
    }

    private IEnumerator Fade(float targetAlpha)
    {

        float startAlpha = fadeCanvasGroup.alpha;
        float time = 0f;
        bool goingToBlack = targetAlpha > 0.5f;
        bool raycastSet = false;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float t = time / fadeDuration;
            float currentAlpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            fadeCanvasGroup.alpha = currentAlpha;
            Debug.Log("Current alpha: " + currentAlpha);

            // Block raycasts only once it's mostly black
            if (goingToBlack && !raycastSet && currentAlpha >= 0.5f)
            {
                fadeCanvasGroup.blocksRaycasts = true;
                raycastSet = true;
            }

            yield return null;
        }

        fadeCanvasGroup.alpha = targetAlpha;

        // When fade is complete, unblock raycasts if fading in
        if (targetAlpha <= 0.01f)
        {
            fadeCanvasGroup.blocksRaycasts = false;
        }
       
    }
}
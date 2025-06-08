using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class SceneManagerDoor : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private int targetSceneIndex = -1;

    [Header("Fade Settings")]
    [SerializeField] private CanvasGroup fadeCanvasGroup;
    [SerializeField] private float fadeDuration = 1f;

    private Outline outline;
    private bool isHovered = false;
    private static bool isFading = false;

    void Awake()
    {
        outline = GetComponent<Outline>();
        if (outline == null)
        {
            outline = gameObject.AddComponent<Outline>();
            outline.OutlineColor = Color.cyan;
            outline.OutlineWidth = 6f;
        }

        

        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.alpha = 0f;
            fadeCanvasGroup.blocksRaycasts = false;

            if (fadeCanvasGroup.TryGetComponent<Image>(out var img))
                img.raycastTarget = true;
        }
    }

    void Update()
    {
        if (isFading) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!EventSystem.current.IsPointerOverGameObject() && Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.transform == transform)
            {
                if (!isHovered)
                {
                    
                    isHovered = true;
                }
                return;
            }
        }

        // If not hovering anymore
        if (isHovered)
        {
            
            isHovered = false;
        }
    }

    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        if (!isFading && targetSceneIndex >= 0 && targetSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            StartCoroutine(FadeAndSwitch(targetSceneIndex));
        }
    }

    private IEnumerator FadeAndSwitch(int sceneIndex)
    {
        isFading = true;

        if (fadeCanvasGroup == null)
        {
            Debug.LogError("Missing Fade Canvas Group!");
            yield break;
        }

        yield return StartCoroutine(Fade(1f)); // Fade to black

        SceneManager.LoadScene(sceneIndex);
        yield return null;

        yield return StartCoroutine(Fade(0f)); // Optional fade in again

        isFading = false;
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

            if (goingToBlack && !raycastSet && currentAlpha >= 0.5f)
            {
                fadeCanvasGroup.blocksRaycasts = true;
                raycastSet = true;
            }

            yield return null;
        }

        fadeCanvasGroup.alpha = targetAlpha;

        if (targetAlpha <= 0.01f)
            fadeCanvasGroup.blocksRaycasts = false;
    }
}

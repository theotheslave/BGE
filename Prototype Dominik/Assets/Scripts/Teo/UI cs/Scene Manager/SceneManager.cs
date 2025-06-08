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
    private static Transform currentHighlight;
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

        outline.enabled = false;

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

        bool hoveredThisFrame = false;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!EventSystem.current.IsPointerOverGameObject() && Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.transform == transform)
            {
                hoveredThisFrame = true;

                if (currentHighlight != transform)
                {
                    if (currentHighlight != null)
                    {
                        Outline prevOutline = currentHighlight.GetComponent<Outline>();
                        if (prevOutline != null) prevOutline.enabled = false;
                    }

                    currentHighlight = transform;
                }

                if (outline != null) outline.enabled = true;
            }
        }

        if (!hoveredThisFrame && currentHighlight == transform)
        {
            if (outline != null) outline.enabled = false;
            currentHighlight = null;
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

        // Fade to black
        yield return StartCoroutine(Fade(1f));

        SceneManager.LoadScene(sceneIndex);
        yield return null;

        // Optional: fade in again (or remove this if you start new scenes already black)
        yield return StartCoroutine(Fade(0f));

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
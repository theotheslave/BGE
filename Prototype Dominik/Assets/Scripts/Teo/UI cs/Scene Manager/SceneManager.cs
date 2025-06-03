using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
[RequireComponent(typeof(Collider))]
public class SceneManagerDoor : MonoBehaviour
{
    [SerializeField] private int targetSceneIndex = -1;

    private Outline outline;
    private static Transform currentHighlight;

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
    }

    void Update()
    {
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

        if (targetSceneIndex >= 0 && targetSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(targetSceneIndex);
        }
    }
}
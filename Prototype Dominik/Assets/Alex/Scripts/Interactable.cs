using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider))]
public class Interactable : MonoBehaviour
{
    [SerializeField] private bool isCharacter = false;
    [SerializeField] private string[] dialogueLines;

    private Outline outline;

    private static Interactable currentHighlight;
    private static Interactable currentSelection;

    void Awake()
    {
        outline = GetComponent<Outline>();
        if (outline == null)
        {
            outline = gameObject.AddComponent<Outline>();
            outline.OutlineColor = Color.magenta;
            outline.OutlineWidth = 7f;
        }

        outline.enabled = false;
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.transform == transform)
            {
                if (!outline.enabled) outline.enabled = true;
            }
            else
            {
                if (outline.enabled) outline.enabled = false;
            }
        }
        else
        {
            if (outline.enabled) outline.enabled = false;
        }
    }

    void OnMouseEnter()
    {
        if (isCharacter || EventSystem.current.IsPointerOverGameObject())
            return;

        // Disable outline of previously highlighted object
        if (currentHighlight != null && currentHighlight != currentSelection)
        {
            currentHighlight.outline.enabled = false;
        }

        // Highlight this object only if not already selected
        if (this != currentSelection)
        {
            outline.enabled = true;
            currentHighlight = this;
        }
    }

    void OnMouseExit()
    {
        if (isCharacter || this == currentSelection)
            return;

        if (outline != null)
        {
            outline.enabled = false;
        }

        if (currentHighlight == this)
        {
            currentHighlight = null;
        }
    }

    void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        // Deselect previous one
        if (currentSelection != null && currentSelection != this)
        {
            currentSelection.outline.enabled = false;
        }

        currentSelection = this;
        outline.enabled = true;

        DialogueManager manager = DialogueManager.Instance;
        if (!manager.IsDialoguePlaying() || !manager.CurrentSpeakerIs(this))
        {
            manager.StartDialogue(dialogueLines, this);
        }
    }

    public static void ClearSelection()
    {
        if (currentSelection != null && currentSelection.outline != null)
        {
            currentSelection.outline.enabled = false;
        }

        currentSelection = null;
    }
}

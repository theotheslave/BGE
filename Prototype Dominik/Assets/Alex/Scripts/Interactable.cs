using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Interactable : MonoBehaviour
{
    [SerializeField]
    private bool isCharacter = false;
    [SerializeField]
    private string[] dialogueLines;

    private Outline outline;

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

    void OnMouseEnter()
    {
        if (!isCharacter && outline != null)
            outline.enabled = true;
    }

    void OnMouseExit()
    {
        if (!isCharacter && outline != null)
            outline.enabled = false;
    }

    private void OnMouseDown()
    {
        DialogueManager manager = DialogueManager.Instance;

        if (manager.IsDialoguePlaying() && manager.CurrentSpeakerIs(this))
            return;

        manager.StartDialogue(dialogueLines, this);
    }
}
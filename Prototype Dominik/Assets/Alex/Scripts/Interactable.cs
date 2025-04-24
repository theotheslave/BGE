using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Interactable : MonoBehaviour
{
    public bool isCharacter = false;
    public string[] dialogueLines;

    private Outline outline;

    void Start()
    {
        if (!isCharacter)
        {
            outline = GetComponent<Outline>();
            if (outline != null) outline.enabled = false;
        }
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
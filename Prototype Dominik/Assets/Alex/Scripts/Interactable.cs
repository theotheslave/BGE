using UnityEngine;
using UnityEngine.UI;

public class Interactable : MonoBehaviour
{
    public bool isCharacter = false;
    private Outline outline;

    void Start()
    {
        if (!isCharacter)
        {
            outline = gameObject.AddComponent<Outline>();
            outline.enabled = false;
        }
    }

    void OnMouseEnter()
    {
        if (!isCharacter) outline.enabled = true;
    }

    void OnMouseExit()
    {
        if (!isCharacter) outline.enabled = false;
    }

    void OnMouseDown()
    {
        UnityEngine.Object.FindFirstObjectByType<DialogueManager>().StartDialogue(dialogueLines);
    }

    [TextArea(2, 10)]
    public string[] dialogueLines;
}
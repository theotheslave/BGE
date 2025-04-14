using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public float typingSpeed = 0.02f;
    private string[] lines;
    private int currentLine;
    private Coroutine typingCoroutine;
    private bool isTyping;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && dialoguePanel.activeSelf)
        {
            if (isTyping)
            {
                StopCoroutine(typingCoroutine);
                dialogueText.text = lines[currentLine];
                isTyping = false;
            }
            else
            {
                NextLine();
            }
        }
    }

    public void StartDialogue(string[] dialogueLines)
    {
        lines = BreakLines(dialogueLines);
        currentLine = 0;
        dialoguePanel.SetActive(true);
        typingCoroutine = StartCoroutine(TypeLine(lines[currentLine]));
    }

    void NextLine()
    {
        currentLine++;
        if (currentLine < lines.Length)
        {
            typingCoroutine = StartCoroutine(TypeLine(lines[currentLine]));
        }
        else
        {
            dialoguePanel.SetActive(false);
        }
    }

    IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogueText.text = "";
        foreach (char c in line)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;
    }

    string[] BreakLines(string[] inputLines)
    {
        var result = new System.Collections.Generic.List<string>();
        foreach (string original in inputLines)
        {
            string line = original;
            while (line.Length > 50)
            {
                int splitAt = line.LastIndexOf(' ', 50);
                if (splitAt <= 0) splitAt = 50;
                result.Add(line.Substring(0, splitAt).Trim());
                line = line.Substring(splitAt).Trim();
            }
            result.Add(line);
        }
        return result.ToArray();
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("UI References")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;

    [Header("Typing Settings")]
    public float typingSpeed = 0.02f;

    private string[] currentLines;
    private int currentLineIndex = 0;
    private Coroutine typingCoroutine;

    private bool isTyping = false;
    private bool lineFinished = false;
    private bool inputEnabled = false;
    private bool justOpened = false;

    private Interactable currentSpeaker;

    void Awake()
    {
        Instance = this;
        dialoguePanel.SetActive(false);
    }

    void Update()
    {
        if (!inputEnabled || justOpened) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (isTyping)
            {
                StopCoroutine(typingCoroutine);
                dialogueText.text = currentLines[currentLineIndex];
                isTyping = false;
                lineFinished = true;
            }
            else if (lineFinished)
            {
                ShowNextLine();
            }
        }
    }

    public void StartDialogue(string[] lines, Interactable speaker)
    {
        // If the same speaker is already talking, do nothing (Update will handle the click)
        if (IsDialoguePlaying() && CurrentSpeakerIs(speaker))
            return;

        // Otherwise, start new dialogue
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);

        currentSpeaker = speaker;
        currentLines = BreakLongLines(lines);
        currentLineIndex = 0;

        dialoguePanel.SetActive(true);
        inputEnabled = false;
        justOpened = true;

        ShowLine(currentLines[currentLineIndex]);
        StartCoroutine(EnableInputAfterDelay());
    }

    private IEnumerator EnableInputAfterDelay()
    {
        yield return null; // wait one frame
        inputEnabled = true;
        justOpened = false;
    }

    private void ShowLine(string line)
    {
        lineFinished = false;
        typingCoroutine = StartCoroutine(TypeLine(line));
    }

    private IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char c in line)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        lineFinished = true;
    }

    private void ShowNextLine()
    {
        currentLineIndex++;

        if (currentLineIndex >= currentLines.Length)
        {
            EndDialogue();
            return;
        }

        ShowLine(currentLines[currentLineIndex]);
    }

    private void EndDialogue()
    {
        inputEnabled = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
        currentSpeaker = null;
    }

    private string[] BreakLongLines(string[] originalLines)
    {
        var result = new List<string>();

        foreach (string rawLine in originalLines)
        {
            string line = rawLine;

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

    public bool IsDialoguePlaying()
    {
        return inputEnabled;
    }

    public bool CurrentSpeakerIs(Interactable speaker)
    {
        return currentSpeaker == speaker;
    }
}
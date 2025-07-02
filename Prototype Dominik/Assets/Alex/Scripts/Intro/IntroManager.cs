using System.Collections;
using UnityEngine;
using TMPro;

public class IntroManager : MonoBehaviour
{
    [SerializeField] private TMP_Text textUI;
    [TextArea(2, 5)] public string[] lines;
    public float typingSpeed = 0.04f;

    [Header("Typing Sound")]
    public AudioSource typingAudioSource;
    public float typingSoundDelay = 0.05f;

    private int currentLine = 0;
    private bool isTyping = false;
    private Coroutine typingCoroutine;
    private float lastSoundTime = 0f;

    void Start()
    {
        textUI.text = "";
        StartTyping();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                StopCoroutine(typingCoroutine);
                textUI.text = lines[currentLine];
                isTyping = false;
            }
            else
            {
                currentLine++;
                if (currentLine < lines.Length)
                {
                    StartTyping();
                }
                else
                {
                    Debug.Log("Intro finished!");
                }
            }
        }
    }

    void StartTyping()
    {
        textUI.text = "";
        typingCoroutine = StartCoroutine(TypeLine(lines[currentLine]));
    }

    IEnumerator TypeLine(string line)
    {
        isTyping = true;
        foreach (char c in line.ToCharArray())
        {
            textUI.text += c;

            //if (c != ' ' && typingAudioSource != null && Time.time - lastSoundTime > typingSoundDelay)
           // {
          //      typingAudioSource.PlayOneShot(typingAudioSource.clip);
           //     lastSoundTime = Time.time;
           // }

            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;
    }
}

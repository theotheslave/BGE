using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Diagnostics;

public class SceneLogic : MonoBehaviour
{
    public DialogueManager dialogueManager;
    public IdealGasMinigame MiniGameManager;

[Header("Object to deactivate on click")]
 //   [SerializeField] private GameObject objectToDeactivate;

    [SerializeField] private Interactable speaker;
    [SerializeField] private string[] dialogueLines1;
    [SerializeField] private string[] dialogueLines2;
    [SerializeField] private string[] dialogueLines3;

    private int currentLineIndex = 0;

    [SerializeField] private bool MachineDialogue1 = true;

    [SerializeField] private TMP_Text missionText;

    [SerializeField] private GameObject gogglesUI;
    [SerializeField] private GameObject gogglesButton;
    [SerializeField] private GameObject UIBlueFrame1;
    [SerializeField] private GameObject UIBlueFrame2;
    [SerializeField] private GameObject UIBlueFrame3;

    [Header("Step 1 – Dillan Interaction")]
    [SerializeField] private GameObject dillan;
    [SerializeField] private GameObject pvtMachine;

    [Header("Step 2 – Machine and Phantom Mouse")]
//    [SerializeField] private float machineTimer = 3f;
    [SerializeField] private GameObject goggles;

    private Collider dillanHitbox;
    private Collider pvtMachineHitbox;
    private Collider gogglesHitbox;

    private bool FirstTimeGogglesDialogue = false;
    private bool FirstWin = true;

    [Header("Mission Texts")]
    [SerializeField] private string machineInteractionText = "Inspect the machine behind Dillan.";
    [SerializeField] private string missionGoggles = "Take the goggles on the table beside of Dillan";
    [SerializeField] private string gogglesAcquiredText = "Inspect the machine and use goggles on it.";

    public CameraMovement CameraMovement;

    void Awake()
    {
        // Cache colliders
        dillanHitbox = dillan.GetComponent<Collider>();
        pvtMachineHitbox = pvtMachine.GetComponent<Collider>();
        gogglesHitbox = goggles.GetComponent<Collider>();
    }

    void Start()
    {
        CameraMovement.Tutorial = true;
        // Initial state
        pvtMachineHitbox.enabled = false;
        goggles.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            HandleClick();
        if (MiniGameManager.hasWon && FirstWin == true)
        {
            DialogueManager.Instance.StartDialogue(dialogueLines3, speaker);
            FirstWin = false;
        }
    }

    private void HandleClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit)) return;

        Collider clicked = hit.collider;

        // Step 1: Dillan interaction
        if (clicked == dillanHitbox)
        {
            dillanHitbox.enabled = false;
            pvtMachineHitbox.enabled = true;
            missionText.text = machineInteractionText;
            return;
        }


        // step 2: machine interaction
        if (clicked == pvtMachineHitbox && dialogueManager.inputEnabled == false)
        {
            if (MachineDialogue1)
            {
                goggles.SetActive(true);
                DialogueManager.Instance.StartDialogue(dialogueLines1, speaker);
                MachineDialogue1 = false;
                missionText.text = missionGoggles;
                return;
            }
        }

        if (clicked == gogglesHitbox)
        {
            goggles.SetActive(false);
            CameraMovement.Tutorial = false;
            UIBlueFrame1.SetActive(true);
            UIBlueFrame2.SetActive(true);
            UIBlueFrame3.SetActive(true);
            missionText.text = gogglesAcquiredText;
            FirstTimeGogglesDialogue = true;
            return;
        }

        if (clicked == pvtMachineHitbox && FirstTimeGogglesDialogue == true)
        {
            DialogueManager.Instance.StartDialogue(dialogueLines2, speaker);
            FirstTimeGogglesDialogue = false;
        }
    }

//if (firsthand)
//{
//    startcoroutine(machinesequence());
//    return;
//}

    //    // Back button interaction
    //    if (clicked == backButtonCollider)
    //    {
    //        // Logic for destroying phantomMouse could go here
    //        return;
    //    }

    //    // Step 3: Goggles
    //    if (clicked == gogglesHitbox && goggles.activeSelf)
    //    {
    //        if (gogglesUI != null)
    //            gogglesUI.SetActive(true);

    //        gogglesHitbox.enabled = false;
    //        goggles.SetActive(false);
    //        gogglesButton.SetActive(true);
    //        FirstHand = false;
    //        SecondHand = true;
    //        return;
    //    }

    //    // Re-check machine in case of step logic
    //    if (clicked == machineHitbox && SecondHand)
    //    {
    //        UnityEngine.Debug.Log($"HandleClick: clicked on '{clicked.name}', collider: {clicked}.");
    //        DialogueManager.Instance.StartDialogue(dialogueLines2, speaker);
    //    }
    //}

    //private IEnumerator MachineSequence()
    //{
    //    yield return new WaitForSeconds(machineTimer);
    //    if (FirstHand)
    //    {
    //        goggles.SetActive(true);
    //        gogglesHitbox.enabled = true;
    //        DialogueManager.Instance.StartDialogue(dialogueLines1, speaker);
    //        backButton.SetActive(true);
    //    }
    //}

    //private IEnumerator MachineSequence2()
    //{
    //    yield return new WaitForSeconds(machineTimer / 8f);
    //    // Any additional logic for the second sequence can go here
    //}

    //private void HandleButtonClick()
    //{
    //    if (objectToDeactivate != null)
    //        objectToDeactivate.SetActive(false);

    //    ThirdHand = true;
    //    SecondHand = false;
    //}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneLogic : MonoBehaviour
{
    [Header("Object to deactivate on click")]
    [SerializeField] private GameObject objectToDeactivate;

    [SerializeField] private Interactable speaker;
    [SerializeField] private string[] dialogueLines1;
    [SerializeField] private string[] dialogueLines2;

    [SerializeField] private GameObject gogglesUI;
    [SerializeField] private GameObject gogglesButton;

    [Header("Step 1 – Dillan Interaction")]
    [SerializeField] private GameObject dillan;
    [SerializeField] private GameObject pvtMachine;

    [Header("Step 2 – Machine and Phantom Mouse")]
    [SerializeField] private float machineTimer = 3f;
    [SerializeField] private GameObject goggles;

    private Collider dillanHitbox;
    private Collider pvtMachineHitbox;
    private Collider gogglesHitbox;

    private GameObject currentPhantomMouse;

    void Awake()
    {
        // Cache colliders
        dillanHitbox = dillan.GetComponent<Collider>();
        pvtMachineHitbox = pvtMachine.GetComponent<Collider>();
        gogglesHitbox = goggles.GetComponent<Collider>();
    }

    void Start()
    {
        // Initial state
        pvtMachineHitbox.enabled = false;
        goggles.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            HandleClick();
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
            return;
        }
    }

    //    // Step 2: Machine interaction
    //    if (clicked == machineHitbox && currentPhantomMouse == null)
    //    {
    //        if (SecondHand)
    //        {
    //            DialogueManager.Instance.StartDialogue(dialogueLines2, speaker);
    //            StartCoroutine(MachineSequence2());
    //            return;
    //        }

    //        if (FirstHand)
    //        {
    //            StartCoroutine(MachineSequence());
    //            return;
    //        }
    //    }

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

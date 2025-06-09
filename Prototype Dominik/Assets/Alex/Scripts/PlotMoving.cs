using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class SequenceController : MonoBehaviour
{
    // UI hand to move
    [SerializeField] private Vector2 targetAnchoredPosition; // target position in canvas space
    [SerializeField] private float moveDuration = 1f;        // time to move there

    private Vector2 startAnchoredPosition;


    [Header("Step 1 – Dillan Interaction")]
    [SerializeField] private GameObject dillan;
    [SerializeField] private GameObject pvtMachine;

    [Header("Step 2 – Machine and Phantom Mouse")]
    [SerializeField] private GameObject machine;
    [SerializeField] private float machineTimer = 3f;
    [SerializeField] private GameObject goggles;
    [SerializeField] private Transform[] mousePath;
    [SerializeField] private GameObject phantomMousePrefab;
    [SerializeField] private GameObject backButton;

    [Header("UI Settings")]
    [SerializeField] private RectTransform uiCanvas;
    [SerializeField] private RectTransform phantomHandUI;      // Pre-placed UI hand
    [SerializeField] private float handMoveDuration = 1f;      // duration in seconds

    [Header("UI Targets for Hand")]
    [SerializeField] private RectTransform handStartPointUI;
    [SerializeField] private RectTransform[] buttonTargetsUI;

    [Header("Final Hand Movement")]
    [SerializeField] private RectTransform finalHandEndUI;
    [SerializeField] private float finalHandDuration = 10f;

    private Collider dillanHitbox;
    private Collider pvtMachineHitbox;
    private Collider machineHitbox;
    private Collider gogglesHitbox;
    private Collider backButtonCollider;
    private Collider[] buttonHitboxes;

    private GameObject currentPhantomMouse;

    // Hand movement state
    private bool handMoving = false;
    private List<Vector2> handControlPoints;
    private float handElapsed = 0f;

    void Awake()
    {

        // Cache colliders
        dillanHitbox = dillan.GetComponent<Collider>();
        pvtMachineHitbox = pvtMachine.GetComponent<Collider>();
        machineHitbox = machine.GetComponent<Collider>();
        gogglesHitbox = goggles.GetComponent<Collider>();
        backButtonCollider = backButton.GetComponent<Collider>();

        buttonHitboxes = new Collider[buttonTargetsUI.Length];
        for (int i = 0; i < buttonTargetsUI.Length; i++)
            buttonHitboxes[i] = buttonTargetsUI[i].GetComponent<Collider>();
    }

    void Start()
    {
        // Initial state
        pvtMachineHitbox.enabled = false;
        goggles.SetActive(false);
        gogglesHitbox.enabled = false;
        backButton.SetActive(false);
        DisableColliders(buttonHitboxes);

        // Setup UI hand
        if (phantomHandUI != null && uiCanvas != null)
        {
            phantomHandUI.SetParent(uiCanvas, false);
            phantomHandUI.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // Click handling
        if (Input.GetMouseButtonDown(0)) HandleClick();

        // Hand movement update
        if (handMoving && phantomHandUI != null)
        {
            handElapsed += Time.deltaTime;
            float t = handElapsed / handMoveDuration;
            if (t >= 1f)
            {
                phantomHandUI.anchoredPosition = handControlPoints[handControlPoints.Count - 1];
                handMoving = false;
            }
            else
            {
                Vector2 pos = Bezier2D(handControlPoints, t);
                phantomHandUI.anchoredPosition = pos;
            }
        }
    }

    private void HandleClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit)) return;

        Collider clicked = hit.collider;

        // Step 1: Dillan
        if (clicked == dillanHitbox)
        {
            dillanHitbox.enabled = false;
            pvtMachineHitbox.enabled = true;
            // TODO: trigger Dillan dialogue
            return;
        }

        // Step 2: Machine
        if (clicked == machineHitbox && currentPhantomMouse == null)
        {
            StartCoroutine(MachineSequence());
            return;
        }

        // Back button
        if (clicked == backButtonCollider)
        {
            if (currentPhantomMouse != null)
            {
                Destroy(currentPhantomMouse);
                backButton.SetActive(false);
            }
            return;
        }

        // Step 3: Goggles
        if (clicked == gogglesHitbox && goggles.activeSelf)
        {
            gogglesHitbox.enabled = false;
            goggles.SetActive(false);
            // TODO: trigger goggles dialogue
            return;
        }

        // Steps 4 & 5: Button targets
        for (int i = 0; i < buttonHitboxes.Length; i++)
        {
            if (clicked == buttonHitboxes[i])
            {
                // Activate and position hand
                phantomHandUI.gameObject.SetActive(true);
                phantomHandUI.anchoredPosition = handStartPointUI.anchoredPosition;

                // Prepare control points (start -> target)
                handControlPoints = new List<Vector2> {
                    handStartPointUI.anchoredPosition,
                    buttonTargetsUI[i].anchoredPosition
                };
                handElapsed = 0f;
                handMoving = true;

                // TODO: trigger button dialogue
                return;
            }
        }
    }

    private IEnumerator MachineSequence()
    {
        yield return new WaitForSeconds(machineTimer);

        goggles.SetActive(true);
        gogglesHitbox.enabled = true;
        // TODO: post-timer dialogue


        // Cache start position (center of screen assumed at Start)
        startAnchoredPosition = phantomHandUI.anchoredPosition;
        phantomHandUI.gameObject.SetActive(true);
        StartCoroutine(MoveAndResetLoop());
        backButton.SetActive(true);
    }

    private Vector2 Bezier2D(List<Vector2> points, float t)
    {
        if (points.Count == 1)
            return points[0];
        List<Vector2> next = new List<Vector2>();
        for (int i = 0; i < points.Count - 1; i++)
            next.Add(Vector2.Lerp(points[i], points[i + 1], t));
        return Bezier2D(next, t);
    }

    private IEnumerator MoveAndResetLoop()
    {
        while (true)
        {
            // Move from start to target
            yield return MoveHand(startAnchoredPosition, targetAnchoredPosition, moveDuration);
            // Teleport back instantly
            phantomHandUI.anchoredPosition = startAnchoredPosition;
            // Optionally wait before next move
            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator MoveHand(Vector2 from, Vector2 to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            phantomHandUI.anchoredPosition = Vector2.Lerp(from, to, t);
            yield return null;
        }
        // Ensure final position
        phantomHandUI.anchoredPosition = to;
    }

    private void DisableColliders(Collider[] cols)
    {
        foreach (var c in cols)
            if (c != null)
                c.enabled = false;
    }
}
using UnityEngine;
using System.Collections;
using Unity.Cinemachine;
using System;

public class CameraMovement : MonoBehaviour
{
    public enum ActivePuzzleType { None, Isochoric, Isothermal }
    public static CameraMovement Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private GameObject Goggles;
    [SerializeField] private GameObject UIGoggles;
    [SerializeField] private GameObject isochoricGoggles;
    [SerializeField] private GameObject isothermalGoggles;

    [Header("Camera Components")]
    [SerializeField] private CinemachineCamera virtualCam;
    [SerializeField] private CinemachineSplineDolly dolly;

    [Header("Movement Settings")]
    [SerializeField] private float travelSeconds = 1.2f;
    [SerializeField] private float gogglesSplinePosition = 0.8f;
    [SerializeField] private Transform gogglesFocusTarget;

    [Header("Optional")]
    [SerializeField] private Transform defaultLookAt;

    private Quaternion originalRotation;
    private float originalSplinePosition;
    private Transform originalLookAt;
    public Transform lastTarget;

    private Coroutine mover;
    private bool isMoving = false;
    private bool lastTargetWasDeactivated;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        InitializeCamera();
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }
    public static void Register(CameraMovement cam)
    {
        if (Instance == null)
        {
            Instance = cam;
            cam.InitializeCamera();
        }
    }
    public void InitializeCamera()
    {
        Goggles?.SetActive(false);
        if (dolly != null) originalSplinePosition = Mathf.Clamp01(dolly.CameraPosition);
        if (defaultLookAt != null) originalLookAt = defaultLookAt;
        if (virtualCam != null) originalRotation = virtualCam.transform.rotation;
    }

    public void FocusTo(float splinePos, Transform lookTarget)
    {
        if (isMoving) return;
        lastTarget = lookTarget;

        if (virtualCam != null)
            virtualCam.LookAt = lookTarget != null ? lookTarget : defaultLookAt;

        lastTargetWasDeactivated = lookTarget != null && !lookTarget.gameObject.activeSelf;
        if (lastTargetWasDeactivated)
            lookTarget.gameObject.SetActive(true);

        StartMove(splinePos);
//        Goggles?.SetActive(true);
    }

    public void FocusToGogglesView()
    {
        if (isMoving) return;

        if (virtualCam != null)
            virtualCam.LookAt = gogglesFocusTarget != null ? gogglesFocusTarget : defaultLookAt;

        StartMove(gogglesSplinePosition);
        Goggles?.SetActive(true);
    }

    public void ReturnToStart()
    {
        if (isMoving) return;

        if (virtualCam != null)
            virtualCam.LookAt = originalLookAt;

        if (lastTarget != null && !lastTarget.gameObject.activeSelf)
        {
            lastTarget.gameObject.SetActive(true);
        }


        lastTarget = null;

        if (UIGoggles != null && UIGoggles.activeSelf)
        {
            UIGoggles.SetActive(false);
        }

        StartMove(originalSplinePosition, true);
    }

    private void StartMove(float targetSplinePos, bool returningToStart = false)
    {
        if (mover != null) StopCoroutine(mover);
        mover = StartCoroutine(Move(dolly.CameraPosition, Mathf.Clamp01(targetSplinePos), returningToStart));
    }

    private IEnumerator Move(float from, float to, bool returningToStart = false)
    {
        isMoving = true;

        Quaternion startRotation = virtualCam.transform.rotation;
        Quaternion endRotation = returningToStart ? originalRotation : virtualCam.transform.rotation;

        for (float t = 0f; t < 1f; t += Time.deltaTime / travelSeconds)
        {
            float eased = 0.5f - 0.5f * Mathf.Cos(t * Mathf.PI);
            dolly.CameraPosition = Mathf.Lerp(from, to, eased);
            if (returningToStart)
                virtualCam.transform.rotation = Quaternion.Slerp(startRotation, endRotation, eased);
            yield return null;
        }

        dolly.CameraPosition = to;
        if (returningToStart)
        {
            virtualCam.transform.rotation = endRotation;
            SelectableObject.ClearSelection();
            SelectableObjectRoom2.ClearSelection();
        }

        isMoving = false;
    }
}
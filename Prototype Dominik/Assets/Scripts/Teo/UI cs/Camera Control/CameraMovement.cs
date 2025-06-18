using UnityEngine;
using System.Collections;
using Unity.Cinemachine;

public class CameraMovement : MonoBehaviour
{
    public static CameraMovement Instance { get; set; }

    [Header("UI")]
    [SerializeField] private GameObject Goggles;
    [SerializeField] private GameObject UIGoggles;

    [Header("Camera Components")]
    [SerializeField] private CinemachineCamera virtualCam;
    [SerializeField] private CinemachineSplineDolly dolly;
    private Quaternion originalRotation;

    [Header("Movement Settings")]
    [SerializeField] private float travelSeconds = 1.2f;
    [SerializeField] private float gogglesSplinePosition = 0.8f;
    [SerializeField] private Transform gogglesFocusTarget;

    [Header("Optional")]
    [SerializeField] private Transform defaultLookAt;

    private Coroutine mover;
    private float originalSplinePosition;
    private Transform originalLookAt;
    public Transform lastTarget;

    private bool isMoving = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        if (gameObject.scene.name == "DontDestroyOnLoad") // It's been carried across
        {
            Debug.LogWarning("Destroying UIManager that was preserved by mistake.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        InitializeCamera();
    }

    public void InitializeCamera()
    {
        if (Goggles != null) Goggles.SetActive(false);

        if (dolly != null)
            originalSplinePosition = dolly.CameraPosition;

        if (defaultLookAt != null)
            originalLookAt = defaultLookAt;

        if (virtualCam != null)
            originalRotation = virtualCam.transform.rotation;
    }

    public void Focus(float destination, Transform lookTarget)
    {
        if (isMoving) return;

        if (virtualCam != null)
            virtualCam.LookAt = defaultLookAt;

        if (mover != null)
            StopCoroutine(mover);

        lastTarget = lookTarget;
        mover = StartCoroutine(Move(dolly.CameraPosition, destination));
        if (Goggles != null) Goggles.SetActive(true);
    }

    public void FocusGoggles()
    {
        if (isMoving) return;

        if (virtualCam != null)
            virtualCam.LookAt = gogglesFocusTarget != null ? gogglesFocusTarget : defaultLookAt;

        if (mover != null)
            StopCoroutine(mover);

        mover = StartCoroutine(Move(dolly.CameraPosition, gogglesSplinePosition));
        if (Goggles != null) Goggles.SetActive(true);
    }

    public void ReturnToStart()
    {
        if (isMoving) return;

        if (virtualCam != null)
            virtualCam.LookAt = originalLookAt;

        if (mover != null)
            StopCoroutine(mover);

        if (lastTarget != null && !lastTarget.gameObject.activeSelf)
        {
            lastTarget.gameObject.SetActive(true);
        }

        mover = StartCoroutine(Move(dolly.CameraPosition, originalSplinePosition, true));
        if (Goggles != null) Goggles.SetActive(false);
        lastTarget = null;

        if (UIGoggles != null && UIGoggles.activeSelf)
        {
            UIGoggles.SetActive(false);
        }
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
        }

        isMoving = false;
    }
}

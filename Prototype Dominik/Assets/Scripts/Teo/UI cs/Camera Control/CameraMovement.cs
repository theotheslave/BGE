using UnityEngine;
using System.Collections;
using Unity.Cinemachine;

public class CameraMovement : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject Goggles;

    [Header("Camera Components")]
    [SerializeField] private CinemachineCamera virtualCam;
    [SerializeField] private CinemachineSplineDolly dolly;
    private Quaternion originalRotation;
    [Header("Movement Settings")]
    [SerializeField] private float travelSeconds = 1.2f;


    [Header("Optional")]
    [SerializeField] private Transform defaultLookAt;

    private Coroutine mover;
    private float originalSplinePosition;
    private Transform originalLookAt;
    private Transform lastTarget;

    private bool isMoving = false; 

    void Awake()
    {
        Goggles.SetActive(false);
        originalSplinePosition = dolly.CameraPosition;
        originalLookAt = defaultLookAt != null ? defaultLookAt : null;
        originalRotation = virtualCam.transform.rotation;
    }

    public void Focus(float destination, Transform lookTarget)
    {
        if (isMoving) return; 

        if (virtualCam != null)
            virtualCam.LookAt = lookTarget;

        if (mover != null)
            StopCoroutine(mover);

        lastTarget = lookTarget;
        mover = StartCoroutine(Move(dolly.CameraPosition, destination));
        Goggles.SetActive(true);
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
        Goggles.SetActive(false);
        lastTarget = null;
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
            {

                virtualCam.transform.rotation = Quaternion.Slerp(startRotation, endRotation, eased);
            }

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



using UnityEngine;
using System.Collections;
using Unity.Cinemachine;

public class CameraMovement : MonoBehaviour
{
    [Header("Camera Components")]
    [SerializeField] private CinemachineVirtualCamera virtualCam;   // Reference to the virtual camera
    [SerializeField] private CinemachineSplineDolly dolly;          // Reference to the dolly component

    [Header("Movement Settings")]
    [SerializeField] private float travelSeconds = 1.2f;            // Duration of glide along the spline

    private Coroutine mover;

    public void Focus(float destination, Transform lookTarget)
    {
        if (virtualCam != null)
            virtualCam.LookAt = lookTarget;

        if (mover != null)
            StopCoroutine(mover);

        mover = StartCoroutine(Move(dolly.CameraPosition, destination));
        Debug.Log("Focus called: " + destination + " | Looking at: " + lookTarget.name);
    }

    private IEnumerator Move(float from, float to)
    {
        for (float t = 0f; t < 1f; t += Time.deltaTime / travelSeconds)
        {
            float eased = 0.5f - 0.5f * Mathf.Cos(t * Mathf.PI); 
            dolly.CameraPosition = Mathf.Lerp(from, to, eased);
            yield return null;
        }
        dolly.CameraPosition = to;
    }
}


using UnityEngine;

public class MachineInteractable : MonoBehaviour
{
    public GameObject machineDetailView;
    public Transform focusPoint;

    private bool hasActivated = false;

    void OnMouseDown()
    {
        if (hasActivated) return;
        hasActivated = true;

        if (BlurController.Instance != null)
            BlurController.Instance.EnableBlur();

        if (machineDetailView != null)
        {
            if (focusPoint != null)
                machineDetailView.transform.position = focusPoint.position;

            machineDetailView.SetActive(true);
        }
    }
}

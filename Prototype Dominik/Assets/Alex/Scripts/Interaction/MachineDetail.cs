using UnityEngine;

public class MachineDetail : MonoBehaviour
{
    public void CloseMachine()
    {
        gameObject.SetActive(false);
        BlurController.Instance.DisableBlur();
    }
}

using UnityEngine;

public class Finishedroomactivation : MonoBehaviour
{
    [SerializeField] private MachineWorkTransition machineToActivate;
    [SerializeField] private HeatingFeedbackScriptVA heatingFeedback;


    void Start()
    {

        if (machineToActivate != null)
        {
            machineToActivate.SetWorkTrigger(true);
        }
        if (heatingFeedback != null)
        {

            heatingFeedback.HeatingUp(true);

        }

    }

   
}

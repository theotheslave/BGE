using UnityEngine;

public class MachineProgressManager : MonoBehaviour
{
    public static MachineProgressManager Instance { get; private set; }

    public bool isobaricCompleted = false;
    public bool isochoricCompleted = false;
    public bool isothermalCompleted = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IsochoricMinigame : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Slider nSlider;
    [SerializeField] private Slider tSlider;
    [SerializeField] private TextMeshProUGUI temperatureDisplay;
    [SerializeField] private TextMeshProUGUI pressureDisplay;
    [SerializeField] private TextMeshProUGUI winText;
    [SerializeField] private Collider objectToLock;
    [Header("Gas Constants")]
    private float R = 8.314f;
    [SerializeField] private float containerVolume = 0.015f;

    [Header("Problem Setup")]
    [SerializeField] private float initialPressure = 125000f; 
    [SerializeField] private float targetPressure = 250000f; 
    [SerializeField] private float initialTemperature = 670f;
    [SerializeField] private float targetTemperature;
    private float currentTemperature;
    private float currentMoles;
    private float currentPressure;

    [Header("Win Condition")]
    [SerializeField] private float temperatureTolerance = 5f;
    [SerializeField] private float winHoldTime = 2f;
    private float indicator1Timer = 0f;
    private float indicator2Timer = 0f;
    [SerializeField] private float indicatorHoldTime = 1.5f;
    private float winTimer;
    private bool phase1Complete = false;
    private bool hasWon = false;

    [Header("References")]
    [SerializeField] private IndicatorsInsideScriptVA indicatorController;

    public Spawner moleculeSpawner;
    public event System.Action OnPuzzleComplete;
    [SerializeField] private float phase1HoldTime = 2f;

    private float phase1Timer = 0f;
    void Start()
    {
       //if (!MachineProgressManager.Instance.isobaricCompleted)
       //{
       //     Debug.LogWarning("Isobaric puzzle not completed. Access denied.");  
       //     return;
       //}
        currentTemperature = initialTemperature;

        targetTemperature = (targetPressure * currentTemperature) / initialPressure;

        float neededN = (initialPressure * containerVolume) / (R * currentTemperature);

        float minN = 0.01f;
        float maxN = 1.0f;
        float startN = neededN * 0.6f;
        float normalizedValue = Mathf.InverseLerp(minN, maxN, startN);
        nSlider.value = normalizedValue;

        tSlider.interactable = false;
        winText.gameObject.SetActive(false);

        moleculeSpawner.SpawnMolecules(moleculeSpawner.startCount, currentTemperature);
    }

    void Update()
    {
        float minN = 0.01f;
        float maxN = 1.0f;

        if (!phase1Complete)
        {
            currentMoles = Mathf.Lerp(minN, maxN, nSlider.value);
            currentPressure = (currentMoles * R * currentTemperature) / containerVolume;

            float targetN = (initialPressure * containerVolume) / (R * currentTemperature);
            float deltaN = Mathf.Abs(currentMoles - targetN);

            //if (indicatorController != null)
            //{
            //    indicatorController.IndicatorTrigger1 = deltaN <= currentMoles * 0.1f;
            //}

            if (deltaN <= currentMoles * 0.1f)
            {
                indicator1Timer += Time.deltaTime;
                if (indicator1Timer >= indicatorHoldTime)
                    indicatorController.IndicatorTrigger1 = true;
                phase1Timer += Time.deltaTime;
                if (phase1Timer >= phase1HoldTime)
                {
                   
                    phase1Complete = true;
                    nSlider.interactable = false;
                    tSlider.interactable = true;
                }
            }
            else
            {
                indicator1Timer = 0f;
                indicatorController.IndicatorTrigger1 = false;
                phase1Timer = 0f;
                if (indicatorController != null)
                {
                    indicatorController.IndicatorTrigger1 = false;
                }
            }
        }
        else if (!hasWon)
        {
            currentTemperature = Mathf.Lerp(273f, 1500f, tSlider.value);
            currentPressure = (currentMoles * R * currentTemperature) / containerVolume;

            float deltaT = Mathf.Abs(currentTemperature - targetTemperature);
            //if (indicatorController != null)
            //{
            //    indicatorController.IndicatorTrigger3 = deltaT <= temperatureTolerance;
            //}
            if (deltaT <= temperatureTolerance)
            {
                indicator2Timer += Time.deltaTime;
                if (indicator2Timer >= indicatorHoldTime)
                    indicatorController.IndicatorTrigger2 = true;
                winTimer += Time.deltaTime;
                if (winTimer >= winHoldTime)
                {
                    Win();
                }
            }
            else
            {
                indicator2Timer = 0f;
                indicatorController.IndicatorTrigger2 = false;
                winTimer = 0f;
                if (indicatorController != null)
                    indicatorController.IndicatorTrigger3 = false;
            }
        }

        temperatureDisplay.text = $"T = {currentTemperature:F1} K";
        pressureDisplay.text = $"n = {(currentMoles):F2} kPa";

        moleculeSpawner.ApplyTemperature(currentTemperature);
        moleculeSpawner.currentTemperature = currentTemperature;
    }

    void Win()
    {
        hasWon = true;
        winText.gameObject.SetActive(true);
        winText.text = "Correct!";
        tSlider.interactable = false;
        if (objectToLock != null)
        {
            objectToLock.enabled = false;
        }
        GogglesManagerRoom2.Instance?.ForceClose();
        CameraMovement.Instance?.ReturnToStart();
        OnPuzzleComplete?.Invoke();
        //MachineProgressManager.Instance.isochoricCompleted = true;
    }
}
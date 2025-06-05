using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class IsochoricMinigame : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Slider nSlider;
    [SerializeField] private Slider tSlider;
    [SerializeField] private TextMeshProUGUI temperatureDisplay;
    [SerializeField] private TextMeshProUGUI pressureDisplay;
    [SerializeField] private TextMeshProUGUI winText;

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
    private float winTimer;
    private bool phase1Complete = false;
    private bool hasWon = false;

    void Start()
    {
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

            if (deltaN <= targetN * 0.01f)
            {
                phase1Complete = true;
                nSlider.interactable = false;
                tSlider.interactable = true;
            }
        }
        else if (!hasWon)
        {
            currentTemperature = Mathf.Lerp(273f, 1340f, tSlider.value);
            currentPressure = (currentMoles * R * currentTemperature) / containerVolume;

            float deltaT = Mathf.Abs(currentTemperature - targetTemperature);
            if (deltaT <= temperatureTolerance)
            {
                winTimer += Time.deltaTime;
                if (winTimer >= winHoldTime)
                {
                    Win();
                }
            }
            else
            {
                winTimer = 0f;
            }
        }

        temperatureDisplay.text = $"T = {currentTemperature:F1} K";
        pressureDisplay.text = $"n = {(currentMoles):F2} kPa";
    }

    void Win()
    {
        hasWon = true;
        winText.gameObject.SetActive(true);
        winText.text = "Correct!";
        tSlider.interactable = false;
    }
}
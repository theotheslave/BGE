using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class IsochoricMinigame : MonoBehaviour
{
    [Header("UI")]
    public Slider nSlider;
    public Slider tSlider;
    public TextMeshProUGUI temperatureDisplay;
    public TextMeshProUGUI pressureDisplay;
    public TextMeshProUGUI winText;

    [Header("Gas Constants")]
    public float R = 8.314f;
    public float containerVolume = 0.015f;

    [Header("Problem Setup")]
    public float initialPressure = 125000f; 
    public float targetPressure = 250000f; 
    public float initialTemperature = 670f;
    private float targetTemperature;
    private float currentTemperature;
    private float currentMoles;
    private float currentPressure;

    [Header("Win Condition")]
    public float temperatureTolerance = 5f;
    public float winHoldTime = 2f;
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
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IdealGasMinigame : MonoBehaviour
{
    [Header("UI")]
    public Slider tSlider;
    public Slider vSlider;
    public Slider nSlider;
    public TextMeshProUGUI temperatureDisplay;
    public TextMeshProUGUI volumeDisplay;
    public TextMeshProUGUI moleDisplay;
    public TextMeshProUGUI pressureDisplay;
    public TextMeshProUGUI winText;

    [Header("Constants")]
    public float R = 8.314f;

    [Header("Wall Movement")]
    public Transform leftWall;
    public Transform rightWall;
    public float wallMinX = -0.5f;
    public float wallMaxX = -1.5f;

    [Header("Parameter Ranges")]
    public float minTemp = 273f;
    public float maxTemp = 900f;
    public float minVol = 0.001f;
    public float maxVol = 0.01f; 
    public float minMol = 0.01f;
    public float maxMol = 1.0f;

    [Header("Target Conditions")]
    public float targetPressure = 300000f;
    public float pressureTolerance = 5000f;
    public float winHoldTime = 2f;

    private float currentTemp;
    private float currentVol;
    private float currentMol;
    private float currentPressure;

    private float winTimer;
    private bool hasWon = false;

    void Start()
    {
        winText.gameObject.SetActive(false);
    }

    void Update()
    {
        currentTemp = Mathf.Lerp(minTemp, maxTemp, tSlider.value);
        currentVol = Mathf.Lerp(minVol, maxVol, vSlider.value);
        currentMol = Mathf.Lerp(minMol, maxMol, nSlider.value);

        float normVol = Mathf.InverseLerp(minVol, maxVol, currentVol);
        float leftX = Mathf.Lerp(wallMinX, wallMaxX, normVol);
        float rightX = -leftX;

        Vector3 lPos = leftWall.localPosition;
        Vector3 rPos = rightWall.localPosition;

        leftWall.localPosition = new Vector3(leftX, lPos.y, lPos.z);
        rightWall.localPosition = new Vector3(rightX, rPos.y, rPos.z);

        currentPressure = (currentMol * R * currentTemp) / currentVol;

        temperatureDisplay.text = $"T = {currentTemp:F1} K";
        volumeDisplay.text = $"V = {(currentVol * 1000f):F2} L";
        moleDisplay.text = $"n = {currentMol:F3} mol";
        pressureDisplay.text = $"P = {(currentPressure / 1000f):F1} kPa";

        float deltaP = Mathf.Abs(currentPressure - targetPressure);
        if (!hasWon && deltaP <= pressureTolerance)
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

    void Win()
    {
        hasWon = true;
        winText.gameObject.SetActive(true);
        winText.text = "Correct!";
        tSlider.interactable = false;
        vSlider.interactable = false;
        nSlider.interactable = false;
    }
}
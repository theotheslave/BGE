using UnityEngine;
using UnityEngine.UI;

public class IsobaricMinigame : MonoBehaviour
{
    public GraphVisualize graphVisualizer;
    public float graphSampleInterval = 0.2f;
    private float graphSampleTimer = 0f;

    [Header("UI")]
    public Slider heatSlider;
    public Button toggleGraphButton;
    public GameObject graphPanel;
    public Text debugText;

    [Header("Piston Visual")]
    public Transform piston;
    public float pistonMinY = 0f;
    public float pistonMaxY = 5f;

    [Header("Gas Constants")]
    public float pressure = 101325f;
    public float R = 8.314f; 
    public float molarMass = 0.02897f;
    public float containerVolume = 0.03f; 

    [Header("Gas State")]
    public float initialMoles = 1f;
    private float currentMoles;
    private float currentTemp;
    private float targetTemp;
    private float volume;

    [Header("Escape & Refill")]
    public float escapeRate = 0.1f;
    public float refillThresholdFraction = 0.7f;

    [Header("Heat Transfer")]
    public float heatTransferRate = 1f;

    private void Start()
    {
        currentMoles = initialMoles;
        currentTemp = 273f; 
        toggleGraphButton.onClick.AddListener(() => {
            graphPanel.SetActive(!graphPanel.activeSelf);
        });
    }

    void Update()
    {
        targetTemp = Mathf.Lerp(273f, 800f, heatSlider.value);

        currentTemp = Mathf.Lerp(currentTemp, targetTemp, heatTransferRate * Time.deltaTime);

        volume = (currentMoles * R * currentTemp) / pressure;

        if (volume > containerVolume)
        {
            float lost = escapeRate * Time.deltaTime;
            currentMoles -= lost;
            currentMoles = Mathf.Max(0.1f, currentMoles);
            debugText.text = $"Gas escaping: -{lost:F3} mol";
        }

        if (currentMoles < initialMoles * refillThresholdFraction)
        {
            currentMoles = initialMoles;
            debugText.text = "Auto-refill";

            graphVisualizer.Clear();
        }

        float pistonY = Mathf.Lerp(pistonMinY, pistonMaxY, Mathf.Clamp01(volume / containerVolume));
        piston.position = new Vector3(piston.position.x, pistonY, piston.position.z);

        if (graphPanel.activeSelf)
        {
            debugText.text += $"\nT: {currentTemp:F1} K\nTarget T: {targetTemp:F1} K" +
                              $"\nV: {(volume * 1000f):F2} L\nn: {currentMoles:F2}";
        }

        graphSampleTimer += Time.deltaTime;
        if (graphSampleTimer >= graphSampleInterval)
        {
            graphVisualizer.AddPoint(currentTemp, volume);
            graphSampleTimer = 0f;
        }
    }
}

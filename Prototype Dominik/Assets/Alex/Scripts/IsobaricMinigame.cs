using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class IsobaricMinigame : MonoBehaviour
{
    [Header("Graph")]
    public GraphVisualize graphVisualizer;
    public float graphSampleInterval = 0.2f;
    private float graphSampleTimer = 0f;

    [Header("UI")]
    public Slider heatSlider;
    public Button toggleGraphButton;
    public GameObject graphPanel;
    public TextMeshProUGUI debugText;

    [Header("Piston Visual")]
    public Transform piston;
    public float pistonMinY = -1.3f;
    public float pistonMaxY = 2.3f;

    [Header("Gas Constants")]
    public float pressure = 101325f;
    public float R = 8.314f;
    public float molarMass = 0.02897f;
    public float containerVolume = 0.065f;

    [Header("Gas State")]
    public float initialMoles = 1f;
    private float Vmin;
    private float Vmax;
    private float currentMoles;
    private float currentTemp;
    private float targetTemp;
    private float volume;

    [Header("Escape & Refill")]
    public float escapeRate = 0.1f;
    public float refillThresholdFraction = 0.7f;
    private bool isCycling = false;
    private Coroutine cycleCoroutine;

    [Header("Heat Transfer")]
    public float heatTransferRate = 1f;

    private void Start()
    {
        currentMoles = initialMoles;
        currentTemp = 273f;
        Vmin = (initialMoles * R * 273f) / pressure;
        Vmax = (initialMoles * R * 800f) / pressure;

        graphPanel.SetActive(false); // turn graph off by default

        toggleGraphButton.onClick.AddListener(() =>
        {
            graphPanel.SetActive(!graphPanel.activeSelf);
        });
    }

    void Update()
    {
        targetTemp = Mathf.Lerp(273f, 800f, heatSlider.value);
        currentTemp = Mathf.Lerp(currentTemp, targetTemp, heatTransferRate * Time.deltaTime);
        volume = (currentMoles * R * currentTemp) / pressure;

        string log = "";

        if (!isCycling && volume >= containerVolume - 0.0001f)
        {
            cycleCoroutine = StartCoroutine(RunCycle());
        }

        if (currentMoles < initialMoles * refillThresholdFraction)
        {
            currentMoles = initialMoles;
            log += "Auto-refill\n";
            graphVisualizer.Clear();
        }

        float normVolume = Mathf.InverseLerp(Vmin, Vmax, volume);
        float pistonY = Mathf.Lerp(pistonMinY, pistonMaxY, normVolume);
        piston.position = new Vector3(piston.position.x, pistonY, piston.position.z);

        log += $"T: {currentTemp:F1} K\nTarget T: {targetTemp:F1} K\n" +
               $"V: {(volume * 1000f):F2} L\nn: {currentMoles:F2}\nSlider: {heatSlider.value:F2}";
        debugText.text = log;

        if (graphPanel.activeSelf)
        {
            graphSampleTimer += Time.deltaTime;
            if (graphSampleTimer >= graphSampleInterval)
            {
                graphVisualizer.AddPoint(currentTemp, volume);
                graphSampleTimer = 0f;
            }
        }
    }
    IEnumerator RunCycle()
    {
        isCycling = true;
        heatSlider.interactable = false;

        debugText.text = "Gas escaping...\n";

        float escapeDuration = 1.5f;
        float elapsed = 0f;
        float startMoles = currentMoles;
        float targetMoles = initialMoles * refillThresholdFraction;

        while (elapsed < escapeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / escapeDuration;
            currentMoles = Mathf.Lerp(startMoles, targetMoles, t);
            yield return null;
        }

        currentMoles = targetMoles;
        debugText.text += "Cooling down...\n";

        float coolingDuration = 2f;
        float coolingElapsed = 0f;
        float startTemp = currentTemp;
        float endTemp = 273f;
        float refillVolumeThreshold = (initialMoles * R * endTemp) / pressure * 1.05f; // 5% tolerance

        while (coolingElapsed < coolingDuration)
        {
            coolingElapsed += Time.deltaTime;
            float t = coolingElapsed / coolingDuration;
            currentTemp = Mathf.Lerp(startTemp, endTemp, t);
            yield return null;
        }

        currentTemp = endTemp;

        while (true)
        {
            float currentVolume = (currentMoles * R * currentTemp) / pressure;
            if (currentVolume <= refillVolumeThreshold)
                break;
            yield return null;
        }

        // --- 4. Refill gas
        debugText.text += "Refilling gas...\n";
        currentMoles = initialMoles;

        // Optional: reset temperature slightly if you want
        // currentTemp = 273f;
        targetTemp = 273f;
        heatSlider.value = 0;

        yield return new WaitForSeconds(0.5f);

        // --- 5. Resume control
        heatSlider.interactable = true;
        isCycling = false;
        graphVisualizer.Clear();
    }

}
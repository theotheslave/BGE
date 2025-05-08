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
    [SerializeField] private TextMeshProUGUI winText;

    [Header("Piston Visual")]
    public Transform piston;
    public float pistonMinY = -1.3f;
    public float pistonMaxY = 2.3f;

    [Header("Gas Constants")]
    public float pressure = 101325f;
    public float R = 8.314f;
    public float containerVolume = 0.065f;

    [Header("References")]
    public Spawner moleculeSpawner;

    [Header("Gas State")]
    public float initialMoles = 1f;
    private float Vmin;
    private float Vmax;
    private float currentMoles;
    private float currentTemp;
    private float targetTemp;
    private float volume;

    [Header("Escape & Refill")]
    public float refillThresholdFraction = 0.7f;
    private bool isCycling = false;
    private Coroutine cycleCoroutine;

    [Header("Heat Transfer")]
    public float heatTransferRate = 1f;

    [Header("Win Condition")]
    [SerializeField] private int cyclesToWin = 3;
    private int completedCycles = 0;
    private bool hasCompleted = false;

    private void Start()
    {
        winText.gameObject.SetActive(false);

        currentMoles = initialMoles;
        currentTemp = 273f;

        Vmin = (initialMoles * R * 273f) / pressure;
        Vmax = (initialMoles * R * 800f) / pressure;

        moleculeSpawner.SpawnMolecules(currentTemp);

        graphPanel.SetActive(false);
        toggleGraphButton.onClick.AddListener(() =>
        {
            graphPanel.SetActive(!graphPanel.activeSelf);
        });
    }

    void Update()
    {
        // Step 1: Heat
        targetTemp = Mathf.Lerp(273f, 800f, heatSlider.value);
        currentTemp = Mathf.Lerp(currentTemp, targetTemp, heatTransferRate * Time.deltaTime);

        // Step 2: Molecule count from spawner (not spatial tracking)
        int activeMolecules = moleculeSpawner.GetActiveMoleculeCount();
        float fraction = moleculeSpawner.startMoleculeCount > 0 ?
                         (float)activeMolecules / moleculeSpawner.startMoleculeCount : 0f;
        currentMoles = initialMoles * fraction;

        if (currentMoles < 0.0001f)
            currentMoles = 0.01f; // fallback to avoid piston freeze

        // Step 3: Volume from ideal gas law
        volume = (currentMoles * R * currentTemp) / pressure;

        // Step 4: Auto refill if piston down and under-filled
        bool pistonDown = volume < containerVolume * 0.95f;
        int totalSpawned = moleculeSpawner.TotalSpawnedCount();
        int maxAllowed = moleculeSpawner.startMoleculeCount;

        if (pistonDown && !isCycling && currentMoles < initialMoles && totalSpawned < maxAllowed)
        {
            int missing = Mathf.Clamp(maxAllowed - activeMolecules, 0, maxAllowed - totalSpawned);
            if (missing > 0)
            {
                moleculeSpawner.AddNewMolecules(missing, currentTemp);
            }
        }

        // Step 5: Trigger escape cycle
        if (!isCycling && volume >= containerVolume - 0.0001f)
        {
            cycleCoroutine = StartCoroutine(RunCycle());
        }

        // Step 6: Piston movement
        float normVolume = Mathf.InverseLerp(Vmin, Vmax, volume);
        float pistonY = Mathf.Lerp(pistonMinY, pistonMaxY, normVolume);
        piston.position = new Vector3(piston.position.x, pistonY, piston.position.z);

        // Step 7: UI Debug Info
        string log = $"T: {currentTemp:F1} K\n" +
                     $"Target T: {targetTemp:F1} K\n" +
                     $"V: {(volume * 1000f):F2} L\n" +
                     $"n (moles): {currentMoles:F2}\n" +
                     $"Molecule Count: {activeMolecules}\n" +
                     $"Slider: {heatSlider.value:F2}";
        debugText.text = log;

        // Step 8: Graphing
        if (graphPanel.activeSelf)
        {
            graphSampleTimer += Time.deltaTime;
            if (graphSampleTimer >= graphSampleInterval)
            {
                graphVisualizer.AddPoint(currentTemp, volume);
                graphSampleTimer = 0f;
            }
        }

        // Step 9: Apply temperature to molecules
        moleculeSpawner.ApplyTemperature(currentTemp);
        moleculeSpawner.currentTemperature = currentTemp;
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

        while (coolingElapsed < coolingDuration)
        {
            coolingElapsed += Time.deltaTime;
            float t = coolingElapsed / coolingDuration;
            currentTemp = Mathf.Lerp(startTemp, endTemp, t);
            yield return null;
        }

        currentTemp = endTemp;

        debugText.text += "Refilling gas...\n";
        currentMoles = initialMoles;
        targetTemp = 273f;
        heatSlider.value = 0;

        yield return new WaitForSeconds(0.5f);

        heatSlider.interactable = true;
        isCycling = false;
        graphVisualizer.Clear();

        if (!hasCompleted && completedCycles + 1 >= cyclesToWin)
        {
            hasCompleted = true;
            winText.gameObject.SetActive(true);
            winText.text = "Completed!";

            heatSlider.interactable = false;
            toggleGraphButton.interactable = false;


            StartCoroutine(AutoCycleLoop());
        }
        completedCycles++;
    }

    IEnumerator AutoCycleLoop()
    {
        while (true)
        {
            float targetHeat = Random.Range(0.5f, 1.0f);
            heatSlider.value = targetHeat;

            yield return StartCoroutine(RunCycle());
            yield return new WaitForSeconds(1f);
        }
    }
}

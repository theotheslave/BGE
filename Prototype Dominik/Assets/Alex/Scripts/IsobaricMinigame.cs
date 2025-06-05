using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IsobaricMinigame : MonoBehaviour
{
    [Header("Graph")]
    public GraphVisualize graphVisualizer;
    public float graphSampleInterval = 0.2f;
    private float graphSampleTimer;

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
    public float pressure = 101_325f;
    public float R = 8.314f;
    public float containerVolume = 0.065f; // Not used for win condition now

    [Header("Gas State")]
    public float initialMoles = 1f;
    private float currentMoles;
    private float currentTemp;
    private float targetTemp;
    private float volume;

    [Header("Heat Transfer")]
    public float heatTransferRate = 1f;

    [Header("Win Condition")]
    [SerializeField] private float targetVolume = 0.003f; // 3 liters
    [SerializeField] private float volumeTolerance = 0.0001f;
    [SerializeField] private float requiredHoldTime = 2f;
    private float correctHoldTimer = 0f;
    private bool puzzleCompleted = false;

    [Header("Piston Animation After Win")]
    public float pistonMoveAmplitude = 0.2f;
    public float pistonMoveSpeed = 2f;
    private float pistonBaseY;

    [Header("References")]
    public Spawner moleculeSpawner;

    void Start()
    {
        winText.gameObject.SetActive(false);
        pistonBaseY = piston.position.y;

        currentMoles = initialMoles;
        currentTemp = 273f;

        moleculeSpawner.SpawnMolecules(moleculeSpawner.startCount, currentTemp);

        graphPanel.SetActive(false);
        toggleGraphButton.onClick.AddListener(() =>
        {
            graphPanel.SetActive(!graphPanel.activeSelf);
        });
    }

    void Update()
    {
        // Heat simulation
        targetTemp = Mathf.Lerp(273f, 800f, heatSlider.value);
        currentTemp = Mathf.Lerp(currentTemp, targetTemp, heatTransferRate * Time.deltaTime);

        // Molecule fraction → moles
        int activeMolecules = moleculeSpawner.ActiveCount();
        float fraction = moleculeSpawner.startCount > 0 ? (float)activeMolecules / moleculeSpawner.startCount : 0f;
        currentMoles = initialMoles * Mathf.Max(fraction, 0.01f);

        // Ideal gas law
        volume = (currentMoles * R * currentTemp) / pressure;

        // Visual piston position
        float Vmin = (initialMoles * R * 273f) / pressure;
        float Vmax = (initialMoles * R * 800f) / pressure;
        float normVolume = Mathf.InverseLerp(Vmin, Vmax, volume);
        float pistonY = Mathf.Lerp(pistonMinY, pistonMaxY, normVolume);
        piston.position = new Vector3(piston.position.x, pistonY, piston.position.z);

        // Debug info
        debugText.text = $"T: {currentTemp:F1} K\n" +
                         $"Target T: {targetTemp:F1} K\n" +
                         $"V: {(volume * 1000f):F2} L\n" +
                         $"n (moles): {currentMoles:F2}\n" +
                         $"Molecules: {activeMolecules}\n" +
                         $"Slider: {heatSlider.value:F2}";

        // Graph
        if (graphPanel.activeSelf)
        {
            graphSampleTimer += Time.deltaTime;
            if (graphSampleTimer >= graphSampleInterval)
            {
                graphVisualizer.AddPoint(currentTemp, volume);
                graphSampleTimer = 0f;
            }
        }

        // Apply heat visually
        moleculeSpawner.ApplyTemperature(currentTemp);
        moleculeSpawner.currentTemperature = currentTemp;

        // Win condition check
        if (!puzzleCompleted)
        {
            float delta = Mathf.Abs(volume - targetVolume);

            if (delta <= volumeTolerance)
            {
                correctHoldTimer += Time.deltaTime;
                if (correctHoldTimer >= requiredHoldTime)
                {
                    puzzleCompleted = true;
                    Win();
                }
            }
            else
            {
                correctHoldTimer = 0f;
            }
        }
        else
        {
            AnimatePiston();
        }
    }

    private void Win()
    {
        UIManager.Instance?.HandlePuzzleSolved("Puzzle_A");

        winText.gameObject.SetActive(true);
        winText.text = "Correct!";
        heatSlider.interactable = false;
        toggleGraphButton.interactable = false;
    }

    private void AnimatePiston()
    {
        float offsetY = Mathf.Sin(Time.time * pistonMoveSpeed) * pistonMoveAmplitude;
        piston.position = new Vector3(piston.position.x, pistonBaseY + offsetY, piston.position.z);
    }
}
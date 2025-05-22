using UnityEngine;
using System.Collections;
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
    public float containerVolume = 0.065f; 

    [Header("References")]
    public Spawner moleculeSpawner;  


    [Header("Gas State")]
    public float initialMoles = 1f;
    private float Vmin, Vmax;
    private float currentMoles;
    private float currentTemp;
    private float targetTemp;
    private float volume;

    [Header("Escape & Refill")]
    public float refillThresholdFraction = 0.7f;
    private bool isCycling;
    private Coroutine cycleCoroutine;


    [Header("Heat Transfer")]
    public float heatTransferRate = 1f;

    
    [Header("Win Condition")]
    [SerializeField] private int cyclesToWin = 3;
    private int completedCycles;
    private bool hasCompleted;

   

    void Start()
    {
        winText.gameObject.SetActive(false);

    
        currentMoles = initialMoles;
        currentTemp = 273f;                 

        Vmin = (initialMoles * R * 273f) / pressure;
        Vmax = (initialMoles * R * 800f) / pressure;

        
        moleculeSpawner.SpawnMolecules(moleculeSpawner.startCount, currentTemp);

        graphPanel.SetActive(false);
        toggleGraphButton.onClick.AddListener(() =>
        {
            graphPanel.SetActive(!graphPanel.activeSelf);
        });
    }



    void Update()
    {
      
        targetTemp = Mathf.Lerp(273f, 800f, heatSlider.value);
        currentTemp = Mathf.Lerp(currentTemp, targetTemp, heatTransferRate * Time.deltaTime);

        int activeMolecules = moleculeSpawner.ActiveCount();
        float fraction = moleculeSpawner.startCount > 0
                       ? (float)activeMolecules / moleculeSpawner.startCount
                       : 0f;
        currentMoles = initialMoles * fraction;

        if (currentMoles < 0.0001f) currentMoles = 0.01f;
       
        volume = (currentMoles * R * currentTemp) / pressure;

       
        bool pistonDown = volume < containerVolume * 0.95f;
        int maxAllowed = moleculeSpawner.startCount;

        if (pistonDown && !isCycling &&
            currentMoles < initialMoles &&
            activeMolecules < maxAllowed)
        {
            int missing = maxAllowed - activeMolecules;
            moleculeSpawner.AddNewMolecules(missing, currentTemp);
        }

     
        if (!isCycling && volume >= containerVolume - 0.0001f)
        {
            cycleCoroutine = StartCoroutine(RunCycle());
        }

      
        float normVolume = Mathf.InverseLerp(Vmin, Vmax, volume);
        float pistonY = Mathf.Lerp(pistonMinY, pistonMaxY, normVolume);
        piston.position = new Vector3(piston.position.x, pistonY, piston.position.z);

        debugText.text = $"T: {currentTemp:F1} K\n" +
                         $"Target T: {targetTemp:F1} K\n" +
                         $"V: {(volume * 1000f):F2} L\n" +
                         $"n (moles): {currentMoles:F2}\n" +
                         $"Molecules: {activeMolecules}\n" +
                         $"Slider: {heatSlider.value:F2}";

     
        if (graphPanel.activeSelf)
        {
            graphSampleTimer += Time.deltaTime;
            if (graphSampleTimer >= graphSampleInterval)
            {
                graphVisualizer.AddPoint(currentTemp, volume);
                graphSampleTimer = 0f;
            }
        }

        moleculeSpawner.ApplyTemperature(currentTemp);
        moleculeSpawner.currentTemperature = currentTemp; 
    }

   

    IEnumerator RunCycle()
    {
        isCycling = true;
        heatSlider.interactable = false;

        UIManager.Instance?.HandlePuzzleSolved("Puzzle_A");
        debugText.text = "Gas escaping...\n";

       
        const float escapeDuration = 1.5f;
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

      
        debugText.text += "Cooling down...\n";

        const float coolingDuration = 2f;
        elapsed = 0f;
        float startTemp = currentTemp;
        const float endTemp = 273f;

        while (elapsed < coolingDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / coolingDuration;
            currentTemp = Mathf.Lerp(startTemp, endTemp, t);
            yield return null;
        }

    
        debugText.text += "Refilling gas...\n";
        currentMoles = initialMoles;
        targetTemp = 273f;
        heatSlider.value = 0;

        yield return new WaitForSeconds(0.5f);

        heatSlider.interactable = true;
        isCycling = false;
        graphVisualizer.Clear();

        completedCycles++;

        /* ---- win check ---- */
        if (!hasCompleted && completedCycles >= cyclesToWin)
        {
            hasCompleted = true;
            winText.gameObject.SetActive(true);
            winText.text = "Completed!";

            heatSlider.interactable = false;
            toggleGraphButton.interactable = false;

            StartCoroutine(AutoCycleLoop());
        }
    }

    

    IEnumerator AutoCycleLoop()
    {
        while (true)
        {
            heatSlider.value = Random.Range(0.5f, 1f);
            yield return StartCoroutine(RunCycle());
            yield return new WaitForSeconds(1f);
        }
    }
}
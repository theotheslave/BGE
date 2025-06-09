using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.Rendering.Universal;

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
    public TextMeshProUGUI temperatureDisplay;
    public TextMeshProUGUI volumeDisplay;
    [SerializeField] private TextMeshProUGUI winText;

    [Header("Piston Visual")]
    public Transform piston;
    public float pistonMinY = -1.3f;
    public float pistonMaxY = 2.3f;

    [Header("Gas Constants")]
    public float pressure = 101_325f;
    public float R = 8.314f;
    public float containerVolume = 0.065f;

    [Header("Gas State")]
    public float initialMoles = 1f;
    private float currentMoles;
    private float currentTemp;
    private float targetTemp;
    private float volume;

    [Header("Heat Transfer")]
    public float heatTransferRate = 1f;

    [Header("Win Condition")]
    [SerializeField] private float targetVolume = 0.003f; 
    [SerializeField] private float volumeTolerance = 0.0001f;
    [SerializeField] private float requiredHoldTime = 2f;
    private float correctHoldTimer = 0f;
    private bool puzzleCompleted = false;

    [Header("Piston Animation After Win")]
    public float pistonMoveAmplitude = 0.2f;
    public float pistonMoveSpeed = 2f;
    private float pistonBaseY;

    [Header("Fade Settings")]
    [SerializeField] private List<DecalProjector> decalProjectors = new List<DecalProjector>();
    [SerializeField] private float decalFadeDuration = 1f;
    [SerializeField] private List<ParticleSystem> fogParticles = new List<ParticleSystem>();
    [SerializeField] private float fogFadeDuration = 1f;
    [Header("References")]
    public Spawner moleculeSpawner;
    public UnityEngine.Rendering.Volume globalVolume;
    public float volumeFadeDuration = 1f;
    public Collider objectToLock;
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
        targetTemp = Mathf.Lerp(273f, 800f, heatSlider.value);
        currentTemp = Mathf.Lerp(currentTemp, targetTemp, heatTransferRate * Time.deltaTime);

        int activeMolecules = moleculeSpawner.ActiveCount();
        float fraction = moleculeSpawner.startCount > 0 ? (float)activeMolecules / moleculeSpawner.startCount : 0f;
        currentMoles = initialMoles * Mathf.Max(fraction, 0.01f);

        volume = (currentMoles * R * currentTemp) / pressure;

        float Vmin = (initialMoles * R * 273f) / pressure;
        float Vmax = (initialMoles * R * 800f) / pressure;
        float normVolume = Mathf.InverseLerp(Vmin, Vmax, volume);
        float pistonY = Mathf.Lerp(pistonMinY, pistonMaxY, normVolume);
        piston.position = new Vector3(piston.position.x, pistonY, piston.position.z);

        temperatureDisplay.text = $"T = {currentTemp:F1} K";
        volumeDisplay.text = $"V = {(volume * 1000f):F2} L";

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

    private IEnumerator FadeOutVolume()
    {
        float startWeight = globalVolume.weight;
        float time = 0f;

        while (time < volumeFadeDuration)
        {
            time += Time.deltaTime;
            float t = time / volumeFadeDuration;
            globalVolume.weight = Mathf.Lerp(startWeight, 0f, t);
            yield return null;
        }

        globalVolume.weight = 0f;
    }

    private IEnumerator FadeOutIce()
    {
        float time = 0f;

        // Store initial colors
        List<Color> originalColors = new List<Color>();
        foreach (var decal in decalProjectors)
        {
            if (decal != null && decal.material.HasProperty("_BaseColor"))
                originalColors.Add(decal.material.GetColor("_BaseColor"));
            else
                originalColors.Add(Color.white);
        }

        while (time < decalFadeDuration)
        {
            time += Time.deltaTime;
            float t = time / decalFadeDuration;

            for (int i = 0; i < decalProjectors.Count; i++)
            {
                var decal = decalProjectors[i];
                if (decal == null) continue;

                Color c = originalColors[i];
                c.a = Mathf.Lerp(c.a, 0f, t);
                decal.material.SetColor("_BaseColor", c);
            }

            yield return null;
        }

        // Ensure it's fully faded at the end
        foreach (var decal in decalProjectors)
        {
            if (decal == null) continue;

            Color faded = decal.material.GetColor("_BaseColor");
            faded.a = 0f;
            decal.material.SetColor("_BaseColor", faded);
        }
    }

    private IEnumerator FadeOutFog()
    {
        float time = 0f;

        // Cache original start colors
        List<Color> originalColors = new List<Color>();
        foreach (var ps in fogParticles)
        {
            var main = ps.main;
            originalColors.Add(main.startColor.color);
        }

        while (time < fogFadeDuration)
        {
            time += Time.deltaTime;
            float t = time / fogFadeDuration;

            for (int i = 0; i < fogParticles.Count; i++)
            {
                var ps = fogParticles[i];
                var main = ps.main;
                Color c = originalColors[i];
                c.a = Mathf.Lerp(originalColors[i].a, 0f, t);
                main.startColor = c;
            }

            yield return null;
        }


        foreach (var ps in fogParticles)
        {
            ps.Stop();
        }
    }

    private void Win()
    {
        UIManager.Instance?.HandlePuzzleSolved("Puzzle_A");

        winText.gameObject.SetActive(true);
        winText.text = "Correct!";
        heatSlider.interactable = false;
        toggleGraphButton.interactable = false;
        StartCoroutine(FadeOutFog());
        if (globalVolume != null)
            StartCoroutine(FadeOutVolume());

        if (decalProjectors.Count > 0)
            StartCoroutine(FadeOutIce());

        if (CameraMovement.Instance != null)
        {
            CameraMovement.Instance.ReturnToStart();
        }
        if (objectToLock != null)
        {
            objectToLock.enabled = false;
        }
        GameObject.FindWithTag("Door")?.GetComponent<SceneManagerDoor>()?.UnlockDoor();
    }

    private void AnimatePiston()
    {
        float offsetY = Mathf.Sin(Time.time * pistonMoveSpeed) * pistonMoveAmplitude;
        piston.position = new Vector3(piston.position.x, pistonBaseY + offsetY, piston.position.z);
    }
}
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.Rendering.Universal;
using System.Collections;

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



    [Header("Fade Settings")]
    [SerializeField] private List<DecalProjector> decalProjectors = new List<DecalProjector>();
    [SerializeField] private float decalFadeDuration = 1f;
    [SerializeField] private List<ParticleSystem> fogParticles = new List<ParticleSystem>();
    [SerializeField] private float fogFadeDuration = 1f;

    [Header("Win Condition")]
    [SerializeField] private float temperatureTolerance = 5f;
    [SerializeField] private float winHoldTime = 2f;
    private float winTimer;
    private bool phase1Complete = false;
    private bool hasWon = false;

    [Header("References")]
    public Spawner moleculeSpawner;
    public UnityEngine.Rendering.Volume globalVolume;
    public float volumeFadeDuration = 1f;
    public Collider objectToLock;
    [SerializeField] private SceneManagerDoor doorToUnlock;

    [SerializeField] private float phase1HoldTime = 2f;

    private float phase1Timer = 0f;
    void Start()
    {
       if (!MachineProgressManager.Instance.isobaricCompleted)
       {
            Debug.LogWarning("Isobaric puzzle not completed. Access denied.");  
            return;
       }
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

            if (deltaN <= currentMoles * 0.1f)
            {
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
                phase1Timer = 0f;
            }
        }
        else if (!hasWon)
        {
            currentTemperature = Mathf.Lerp(273f, 1500f, tSlider.value);
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

        moleculeSpawner.ApplyTemperature(currentTemperature);
        moleculeSpawner.currentTemperature = currentTemperature;
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
                c.a = Mathf.Lerp(originalColors[i].a, 0f, t);
                decal.material.SetColor("_BaseColor", c);
            }

            yield return null;
        }

        // Ensure it's fully faded and deactivate
        foreach (var decal in decalProjectors)
        {
            if (decal == null) continue;

            Color faded = decal.material.GetColor("_BaseColor");
            faded.a = 0f;
            decal.material.SetColor("_BaseColor", faded);
            decal.gameObject.SetActive(false); // <--- Disabling here
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


    void Win()
    {
        hasWon = true;
        winText.gameObject.SetActive(true);
        winText.text = "Correct!";
        tSlider.interactable = false;
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
        if (doorToUnlock != null)
        {
            doorToUnlock.UnlockDoor();
        }
        MachineProgressManager.Instance.isochoricCompleted = true;
    }
}
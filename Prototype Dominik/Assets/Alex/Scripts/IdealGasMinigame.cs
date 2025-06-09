using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering.Universal;

public class IdealGasMinigame : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Slider tSlider;
    [SerializeField] private Slider vSlider;
    [SerializeField] private Slider nSlider;
    [SerializeField] private TextMeshProUGUI temperatureDisplay;
    [SerializeField] private TextMeshProUGUI volumeDisplay;
    [SerializeField] private TextMeshProUGUI moleDisplay;
    [SerializeField] private TextMeshProUGUI pressureDisplay;
    [SerializeField] private TextMeshProUGUI winText;

    [Header("Wall Movement")]
    public Transform leftWall;
    public Transform rightWall;
    public float wallMinX = -0.5f;
    public float wallMaxX = -1.5f;

    [Header("Parameter Ranges")]
    [SerializeField] private float minTemp = 273f;
    [SerializeField] private float maxTemp = 900f;
    [SerializeField] private float minVol = 0.001f;
    [SerializeField] private float maxVol = 0.01f;
    [SerializeField] private float minMol = 0.01f;
    [SerializeField] private float maxMol = 1.0f;

    [Header("Target Conditions")]
    [SerializeField] private float targetPressure = 300000f;
    [SerializeField] private float pressureTolerance = 5000f;
    [SerializeField] private float winHoldTime = 2f;

    [Header("Wall Animation")]
    public float wallMoveAmplitude = 0.05f;
    public float wallMoveSpeed = 2f;

    [Header("Fade Settings")]
    [SerializeField] private List<DecalProjector> decalProjectors = new List<DecalProjector>();
    [SerializeField] private float decalFadeDuration = 1f;
    [SerializeField] private List<ParticleSystem> fogParticles = new List<ParticleSystem>();
    [SerializeField] private float fogFadeDuration = 1f;
    [Header("References")]
    public Spawner moleculeSpawner;
    public UnityEngine.Rendering.Volume globalVolume;  
    public float volumeFadeDuration = 1f;
    private float R = 8.314f;

    private float baseLeftX;
    private float baseRightX;
    private bool isAnimating = false;

    private float currentTemp;
    private float currentVol;
    private float currentMol;
    private float currentPressure;

    private float winTimer;
    private bool hasWon = false;

    void Start()
    {
        winText.gameObject.SetActive(false);

        baseLeftX = leftWall.localPosition.x;
        baseRightX = rightWall.localPosition.x;
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

        if (isAnimating)
        {
            float offset = Mathf.Sin(Time.time * wallMoveSpeed) * wallMoveAmplitude;

            leftWall.localPosition = new Vector3(baseLeftX + offset, leftWall.localPosition.y, leftWall.localPosition.z);
            rightWall.localPosition = new Vector3(baseRightX - offset, rightWall.localPosition.y, rightWall.localPosition.z);
        }
        else
        {
            leftWall.localPosition = new Vector3(leftX, leftWall.localPosition.y, leftWall.localPosition.z);
            rightWall.localPosition = new Vector3(rightX, rightWall.localPosition.y, rightWall.localPosition.z);
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

    void Win()
    {
        hasWon = true;
        winText.gameObject.SetActive(true);
        winText.text = "Correct!";
        tSlider.interactable = false;
        vSlider.interactable = false;
        nSlider.interactable = false;
        StartCoroutine(FadeOutFog());
        isAnimating = true;
        if (globalVolume != null)
            StartCoroutine(FadeOutVolume());

        if (decalProjectors.Count > 0)
            StartCoroutine(FadeOutIce());
        if (CameraMovement.Instance != null)
        {
            CameraMovement.Instance.ReturnToStart();
        }


    }
}
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
    [SerializeField] private float minMol = 0.1f;
    [SerializeField] private float maxMol = 1.0f;

    [Header("Individual Slider Targets")]
    [SerializeField] private float targetTemp = 300f;
    [SerializeField] private float targetVol = 0.005f;
    [SerializeField] private float targetMol = 0.5f;
    [SerializeField] private float tempTolerance = 0.5f;     // For temperature
    [SerializeField] private float volumeTolerance = 0.0001f; // Volume 
    [SerializeField] private float molTolerance = 0.01f;      // Mol
    [Header("Target Conditions")]
    [SerializeField] private float targetPressure = 300000f;
    [SerializeField] private float pressureTolerance = 5000f;
    [SerializeField] private float winHoldTime = 2f;
    [SerializeField] private float indicatorHoldTime = 1.5f;
    private float tempHoldTimer = 0f;
    private float volHoldTimer = 0f;
    private float molHoldTimer = 0f;
    [Header("Wall Animation")]
    public float wallMoveAmplitude = 0.05f;
    public float wallMoveSpeed = 2f;

    [Header("Fade Settings")]
    [SerializeField] private List<DecalProjector> decalProjectors = new List<DecalProjector>();
    [SerializeField] private float decalFadeDuration = 1f;
    [SerializeField] private List<ParticleSystem> fogParticles = new List<ParticleSystem>();
    [SerializeField] private float fogFadeDuration = 1f;
    [Header("References")]
    [SerializeField] private IndicatorsInsideScriptVA indicatorController;
    [SerializeField] private SceneManagerDoor doorToUnlock;
    [SerializeField] private MachineWorkTransition machineToActivate;
    [SerializeField] private HeatingFeedbackScriptVA heatingFeedback;
    [SerializeField] private UIdefrostingVA uiDefrosting;
    [SerializeField] private GameObject machineOut;
    public Collider objectToLock;
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
    public bool hasWon = false;

    void Start()
    {
        winText.gameObject.SetActive(false);

        baseLeftX = leftWall.localPosition.x;
        baseRightX = rightWall.localPosition.x;
        nSlider.value = 0.005f;

        moleculeSpawner.SpawnMolecules(moleculeSpawner.maxMoleculeCount, currentTemp);
    }
    private bool UpdateIndicator(ref float holdTimer, float delta, float tolerance, float holdTime)
    {
        if (delta <= tolerance)
        {
            holdTimer += Time.deltaTime;
            return holdTimer >= holdTime;
        }
        else
        {
            holdTimer = 0f;
            return false;
        }
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


        if (indicatorController != null)
        {
            float deltaT = Mathf.Abs(currentTemp - targetTemp);
            float deltaV = Mathf.Abs(currentVol - targetVol);
            float deltaN = Mathf.Abs(currentMol - targetMol);

            indicatorController.IndicatorTrigger1 = UpdateIndicator(ref tempHoldTimer, deltaT, tempTolerance, indicatorHoldTime);
            indicatorController.IndicatorTrigger2 = UpdateIndicator(ref volHoldTimer, deltaV, volumeTolerance, indicatorHoldTime);
            indicatorController.IndicatorTrigger3 = UpdateIndicator(ref molHoldTimer, deltaN, molTolerance, indicatorHoldTime);
        }

        normVol = Mathf.InverseLerp(minVol, maxVol, currentVol);
        float nMultiplier = Mathf.InverseLerp(minMol, maxMol, currentMol);

        moleculeSpawner.UpdateConditions(currentTemp, normVol, nMultiplier);


        Debug.Log($"Temp: {currentTemp} | Vol: {currentVol} | Mol: {currentMol}");
        Debug.Log($"T Delta: {Mathf.Abs(currentTemp - targetTemp)}, V Delta: {Mathf.Abs(currentVol - targetVol)}, n Delta: {Mathf.Abs(currentMol - targetMol)}");

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
        vSlider.interactable = false;
        nSlider.interactable = false;
      machineOut.SetActive(true);
        isAnimating = true;

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
        GameObject.FindWithTag("Door")?.GetComponent<SceneManagerDoor1>()?.UnlockDoor();
        if (machineToActivate != null)
        {
            machineToActivate.SetWorkTrigger(true);
        }
        if(heatingFeedback != null)
        {

            heatingFeedback.HeatingUp(true);

        }

        if (uiDefrosting != null)
        {
            uiDefrosting.DisableUIdefrosting(true);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class IsothermalMinigameFinal : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Slider nSlider;
    [SerializeField] private Slider vSlider;
    [SerializeField] private TextMeshProUGUI volumeDisplay;
    [SerializeField] private TextMeshProUGUI moleDisplay;
    [SerializeField] private TextMeshProUGUI winText;

    [Header("Gas Constants")]
    [SerializeField] private float R = 8.314f;
    [SerializeField] private float temperature = 670f;

    [Header("Volume Limits")]
    [SerializeField] private float minVol = 0.01f;
    [SerializeField] private float maxVol = 0.1f;

    [Header("Target Conditions")]
    [SerializeField] private float initialPressure = 150000f;
    [SerializeField] private float targetPressure = 111205f;
    [SerializeField] private float pressureTolerance = 1000f;
    [SerializeField] private float winHoldTime = 2f;

    [Header("Piston Visual")]
    [SerializeField] private Transform piston;
    [SerializeField] private float pistonMinY = -1.3f;
    [SerializeField] private float pistonMaxY = 2.3f;
    [SerializeField] private float pistonMoveAmplitude = 0.2f;
    [SerializeField] private float pistonMoveSpeed = 2f;

    [Header("Fade Settings")]
    [SerializeField] private List<DecalProjector> decalProjectors = new List<DecalProjector>();
    [SerializeField] private float decalFadeDuration = 1f;
    [SerializeField] private List<ParticleSystem> fogParticles = new List<ParticleSystem>();
    [SerializeField] private float fogFadeDuration = 1f;
    public UnityEngine.Rendering.Volume globalVolume;
    public float volumeFadeDuration = 1f;
    
    [Header("Spawner")]
    [SerializeField] private Spawner moleculeSpawner;

   

    public Collider objectToLock;
    private float currentN;
    private float currentV;
    private float currentP;
    private float winTimer = 0f;
    private bool phase1Complete = false;
    private bool hasWon = false;
    private float pistonBaseY;

    [SerializeField] private float phase1HoldTime = 2f;
    private float phase1Timer = 0f;
    public event System.Action OnPuzzleComplete;
    void Start()
    {
        if (!MachineProgressManager.Instance.isochoricCompleted)
        {
            Debug.LogWarning("Isobaric puzzle not completed. Access denied.");
            return;
        }

        winText.gameObject.SetActive(false);
        vSlider.interactable = false;
        pistonBaseY = piston.position.y;
    }

    void Update()
    {
        if (!hasWon && !phase1Complete)
        {
            currentV = Mathf.Lerp(minVol, maxVol, vSlider.value);
            float normV = Mathf.InverseLerp(minVol, maxVol, currentV);
            float pistonY = Mathf.Lerp(pistonMinY, pistonMaxY, normV);
            piston.position = new Vector3(piston.position.x, pistonY, piston.position.z);
        }
        {
            if (!phase1Complete)
            {
                currentN = Mathf.Lerp(0.5f, 2.0f, nSlider.value);
                moleDisplay.text = $"n = {currentN:F3} mol";

                float targetVolumeForN = 0.05f;
                float calculatedN = (initialPressure * targetVolumeForN) / (R * temperature);
                float deltaN = Mathf.Abs(currentN - calculatedN);

                if (deltaN <= calculatedN * 0.1f)
                {
                    phase1Timer += Time.deltaTime;
                    if (phase1Timer >= phase1HoldTime)
                    {
                        phase1Complete = true;
                        nSlider.interactable = false;
                        vSlider.interactable = true;
                        Debug.Log("Phase 1 complete. Now adjust Volume.");
                    }
                }
                else
                {
                    phase1Timer = 0f;
                }
            }
            else if (!hasWon)
            {
                currentV = Mathf.Lerp(minVol, maxVol, vSlider.value);
                currentP = (currentN * R * temperature) / currentV;
                volumeDisplay.text = $"V = {(currentV * 1000f):F2} L";

                float deltaP = Mathf.Abs(currentP - targetPressure);
                Debug.Log($"Current P: {currentP}, Target P: {targetPressure}, delta: {deltaP}");

                if (deltaP <= pressureTolerance)
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

                float normV = Mathf.InverseLerp(minVol, maxVol, currentV);
                float pistonY = Mathf.Lerp(pistonMinY, pistonMaxY, normV);
                piston.position = new Vector3(piston.position.x, pistonY, piston.position.z);
            }
            else
            {
                AnimatePiston();
            }
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
            vSlider.interactable = false;
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
        OnPuzzleComplete?.Invoke();
        UIManager.Instance?.MarkPuzzleComplete();

    }

    void AnimatePiston()
        {
            float offsetY = Mathf.Sin(Time.time * pistonMoveSpeed) * pistonMoveAmplitude;
            piston.position = new Vector3(piston.position.x, pistonBaseY + offsetY, piston.position.z);
        }
    }


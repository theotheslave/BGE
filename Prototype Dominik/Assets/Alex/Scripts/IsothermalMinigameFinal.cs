using TMPro;
using UnityEngine;
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

    [Header("Spawner")]
    [SerializeField] private Spawner moleculeSpawner;

    private float currentN;
    private float currentV;
    private float currentP;
    private float winTimer = 0f;
    private bool phase1Complete = false;
    private bool hasWon = false;
    private float pistonBaseY;

    void Start()
    {
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
                    phase1Complete = true;
                    nSlider.interactable = false;
                    vSlider.interactable = true;
                    Debug.Log("Phase 1 complete. Now adjust Volume.");
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

        void Win()
        {
            hasWon = true;
            winText.gameObject.SetActive(true);
            winText.text = "Correct!";
            vSlider.interactable = false;
        }

        void AnimatePiston()
        {
            float offsetY = Mathf.Sin(Time.time * pistonMoveSpeed) * pistonMoveAmplitude;
            piston.position = new Vector3(piston.position.x, pistonBaseY + offsetY, piston.position.z);
        }
    }
}

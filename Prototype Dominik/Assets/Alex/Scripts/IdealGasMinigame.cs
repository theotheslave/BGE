using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IdealGasMinigame : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Slider heatSlider;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Slider nSlider;
    [SerializeField] private TextMeshProUGUI debugText;
    [SerializeField] private TextMeshProUGUI winText;

    [Header("Info Toggle")]
    [SerializeField] private GameObject infoPanel;
    [SerializeField] private Button toggleInfoButton;
    [SerializeField] private TextMeshProUGUI tLabel;
    [SerializeField] private TextMeshProUGUI vLabel;
    [SerializeField] private TextMeshProUGUI nLabel;
    [SerializeField] private TextMeshProUGUI tText;
    [SerializeField] private TextMeshProUGUI vText;
    [SerializeField] private TextMeshProUGUI nText;

    private bool showInfo = false;

    [Header("Gas Constants")]
    private float R = 8.314f; 
    [SerializeField] private float tMin = 200f;
    [SerializeField] private float tMax = 800f;
    [SerializeField] private float vMin = 0.01f;
    [SerializeField] private float vMax = 0.1f;
    [SerializeField] private float nMin = 0.1f;
    [SerializeField] private float nMax = 2f;

    [Header("Chamber")]
    [SerializeField] private Transform chamberWalls;
    [SerializeField] private Vector3 chamberMinScale;
    [SerializeField] private Vector3 chamberMaxScale;

    [Header("Chamber Walls (2D)")]
    [SerializeField] private Transform leftWall;
    [SerializeField] private Transform rightWall;
    [SerializeField] private float wallMoveRange = 2f; 
    [SerializeField] private float volumeMin = 0.01f;
    [SerializeField] private float volumeMax = 0.1f;
    private Vector3 leftWallStartPos;
    private Vector3 rightWallStartPos;

    private float T, V, n, P;

    private void Start()
    {
        leftWallStartPos = leftWall.localPosition;
        rightWallStartPos = rightWall.localPosition;

        toggleInfoButton.onClick.AddListener(ToggleInfo);
        infoPanel.SetActive(showInfo);
        UpdateSliderLabels();
    }
    void ToggleInfo()
    {
        showInfo = !showInfo;
        infoPanel.SetActive(showInfo);
        UpdateSliderLabels();
    }
    void UpdateSliderLabels()
    {
        if (showInfo)
        {
            tLabel.text = "T";
            vLabel.text = "V";
            nLabel.text = "n";
        }
        else
        {
            tLabel.text = "Heating";
            vLabel.text = "Expand chamber";
            nLabel.text = "Add gas";
        }
    }

    private void Update()
    {
        T = Mathf.Lerp(tMin, tMax, heatSlider.value);
        V = Mathf.Lerp(vMin, vMax, volumeSlider.value);
        n = Mathf.Lerp(nMin, nMax, nSlider.value);
        P = (n * R * T) / V;

        if (chamberWalls != null)
        {
            float chamberLerp = Mathf.InverseLerp(vMin, vMax, V);
            chamberWalls.localScale = Vector3.Lerp(chamberMinScale, chamberMaxScale, chamberLerp);
            float volumeLerp = Mathf.InverseLerp(volumeMin, volumeMax, V);
            float offset = Mathf.Lerp(0f, wallMoveRange, volumeLerp);

            leftWall.localPosition = leftWallStartPos + new Vector3(-offset, 0f, 0f);
            rightWall.localPosition = rightWallStartPos + new Vector3(offset, 0f, 0f);
        }

        debugText.text = $"T: {T:F1} K\nV: {V:F3} m3\nn: {n:F2} mol\nP: {P:F1} Pa";
        
        if (infoPanel.activeSelf)
        {
            tText.text = $"{T:F0} K";
            vText.text = $"{(V * 1000f):F0} L";
            nText.text = $"{n:F2} mol";
        }
    }
}

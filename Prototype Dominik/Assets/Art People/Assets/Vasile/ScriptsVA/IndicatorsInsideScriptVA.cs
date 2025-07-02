using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class IndicatorsInsideScriptVA : MonoBehaviour
{
    [Header("Trigger Value")] 
    [SerializeField] public bool IndicatorTrigger1 = false;
    [SerializeField] public bool IndicatorTrigger2 = false;
    [SerializeField] public bool IndicatorTrigger3 = false;
    [Header("GameObjects Material List")]
    [SerializeField] private GameObject IndicatorGameObject1;
    [SerializeField] private GameObject IndicatorGameObject2;
    [SerializeField] private GameObject IndicatorGameObject3;
    [SerializeField] private Material TargetMaterial;
    [SerializeField] private Material DefaultMaterial;
    [Header("Light Settings")]
    [SerializeField] private Light TargetLight1;
    [SerializeField] private Light TargetLight2;
    [SerializeField] private Light TargetLight3;
    [SerializeField] private float LightIntensity;
    [SerializeField] private float DefaultLightIntensity;
    [SerializeField] private Color TargetLightColor;
    [SerializeField] private Color DefaultLightColor;

    private Renderer renderer1;
    private Renderer renderer2;
    private Renderer renderer3;
    
    
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        renderer1 = IndicatorGameObject1.GetComponent<Renderer>();
        renderer2 = IndicatorGameObject2.GetComponent<Renderer>();
        renderer3 = IndicatorGameObject3.GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (IndicatorTrigger1 == true)
        {
            TargetLight1.intensity = LightIntensity;
            TargetLight1.color = TargetLightColor;
            renderer1.material = TargetMaterial;
            
        }
        else
        {
            renderer1.material = DefaultMaterial;
            TargetLight1.color = DefaultLightColor;
            TargetLight1.intensity = DefaultLightIntensity;
        }
        
        if (IndicatorTrigger2 == true)
        {
            TargetLight2.intensity = LightIntensity;
            TargetLight2.color = TargetLightColor;
            renderer2.material = TargetMaterial;
        }
        else
        {
            renderer2.material = DefaultMaterial;
            TargetLight2.color = DefaultLightColor;
            TargetLight2.intensity = DefaultLightIntensity;
            
        }
        
        if (IndicatorTrigger3 == true)
        {
            TargetLight3.intensity = LightIntensity;
            TargetLight3.color = TargetLightColor;
            renderer3.material = TargetMaterial;
        }
        else
        {
            renderer3.material = DefaultMaterial;
            TargetLight3.color = DefaultLightColor;
            TargetLight3.intensity = DefaultLightIntensity;
            
        }
    }
}

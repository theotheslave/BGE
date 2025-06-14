using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MachineWorkTransition : MonoBehaviour
{
    [Header("Trigger Value")] 
    [SerializeField] private bool WorkTrigger = false;
    [Header("Particle List/Settings")]
    [SerializeField] private List<ParticleSystem> SteamParticles = new List<ParticleSystem>();
    [Header("Object Rotation List")] 
    [SerializeField] private List<GameObject> ObjectRotation = new List<GameObject>();
    [Header("Object Rotation Values Arrows")] 
    [SerializeField] private float AngleX;
    [SerializeField] private float AngleY;
    [SerializeField] private float AngleZ;
    [SerializeField] private float AngleZ2;
    [SerializeField] private float TimeAtoB;
    [Header("Object Rotation Valves")] 
    [SerializeField] private List<GameObject> ObjectValveRotation = new List<GameObject>();
    [SerializeField] private float ValveAngleX;
    [SerializeField] private float ValveAngleY;
    [SerializeField] private float ValveAngleZ3;
    
    [Header("Emission Material List")]
    [SerializeField] private List<Material> EmissionMaterials = new List<Material>();
    [Header("Light Settings")]
    [SerializeField] private Light TargetLight;
    [SerializeField] private Light AuxiliaryLight;
    [SerializeField] private float AuxiliaryLightIntensity;
    [SerializeField] private float LightIntensity;
    [SerializeField] private Color TargetLightColor; 
    [SerializeField] private Color DefaultLightColor;
    
    
    Quaternion qA, qB;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        qA = Quaternion.Euler(AngleX,AngleY,AngleZ);
        qB = Quaternion.Euler(AngleX,AngleY,AngleZ2);
        
        
        foreach (ParticleSystem particle in SteamParticles)
        {
            particle.Pause(true);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (WorkTrigger == true )
        {
            AuxiliaryLight.intensity = AuxiliaryLightIntensity;
            TargetLight.intensity = LightIntensity;
            TargetLight.color = TargetLightColor;
            
            foreach (Material mat in EmissionMaterials)
            {
                mat.EnableKeyword("_EMISSION");
            }
            foreach (GameObject obj in ObjectValveRotation)
            {
                obj.transform.localRotation = Quaternion.Euler(ValveAngleX,ValveAngleY,ValveAngleZ3);
                //obj.transform.localRotation = Quaternion.Euler(AngleX,AngleY,AngleZ);
                //obj.transform.localRotation = Quaternion.Euler(AngleX,AngleY,Mathf.Clamp(AngleZ, 50, 110));
            }
            
            foreach (GameObject obj in ObjectRotation)
            {
                float t = Mathf.PingPong(Time.time / TimeAtoB, 0.5f);
                obj.transform.localRotation = Quaternion.Slerp(qA, qB, t);
                //obj.transform.localRotation = Quaternion.Euler(AngleX,AngleY,AngleZ);
                //obj.transform.localRotation = Quaternion.Euler(AngleX,AngleY,AngleZ);
                //obj.transform.localRotation = Quaternion.Euler(AngleX,AngleY,Mathf.Clamp(AngleZ, 50, 110));
            }
            
            foreach (ParticleSystem particle in SteamParticles)
            {
                particle.Pause(false);
                particle.Play(true);
            }
        }
        else
        {
          AuxiliaryLight.intensity = 0;  
          TargetLight.color = DefaultLightColor;
            foreach (Material mat in EmissionMaterials) // turns off the emisson map in the material
            {
                mat.DisableKeyword("_EMISSION");
            }
            foreach (GameObject obj in ObjectValveRotation) // keeps the valve rotation as default one after edit
            {
                obj.transform.localRotation = Quaternion.Euler(ValveAngleX,ValveAngleY,ValveAngleX);
                
            }
            foreach (GameObject obj in ObjectRotation) // re writes the rotation for bar meters arrows
            {
                obj.transform.localRotation = default;
            }
            foreach (ParticleSystem particle in SteamParticles) // just stops the particles from the list 
            {
                particle.Stop(true);
            }
        }
    }
}

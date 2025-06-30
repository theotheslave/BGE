using UnityEngine;
using System.Collections.Generic;

public class HeatingFeedbackScriptVA : MonoBehaviour
{
    [Header("Trigger Value")] 
    [SerializeField] private bool WorkTrigger = false;
    [Header("Particle List/Settings")]
    [SerializeField] private List<ParticleSystem> SteamParticles = new List<ParticleSystem>();
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (ParticleSystem particle in SteamParticles)
        {
            particle.Pause(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (WorkTrigger == true)
        {
            foreach (ParticleSystem particle in SteamParticles)
            {
                particle.Pause(false);
                particle.Play(true);
            } 
            
            
        }
        else
        {
            foreach (ParticleSystem particle in SteamParticles) // just stops the particles from the list 
            {
                particle.Stop(true);
            }
        }
    }


    public void HeatingUp(bool state)
    {


        WorkTrigger= state;

    }


}

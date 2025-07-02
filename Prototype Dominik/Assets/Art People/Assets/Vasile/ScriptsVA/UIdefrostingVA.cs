using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIdefrostingVA : MonoBehaviour
{
    [SerializeField] private bool Trigger;
    [Header("List of the UIdefrosting")] 
    [SerializeField] private List<Image> Images = new List<Image>(); 
    [Header("Materials of UIdefrosting")]
    [SerializeField] private Material DefaultMaterial;
    [SerializeField] private Material FrostedMaterial;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void DisableUIdefrosting( bool disable )
    {
        Trigger = disable;
    }
    
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Trigger == true)
        {
            foreach (Image img in Images)
            {
                img.material = DefaultMaterial;
            }
            
        }
        else
        {
            foreach (Image img in Images)
            {
                img.material = FrostedMaterial;
            }
        }
    }
}

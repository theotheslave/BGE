using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BlurController : MonoBehaviour
{
    public static BlurController Instance { get; private set; }

    private Volume volume;
    private DepthOfField dof;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        volume = GetComponent<Volume>();

        if (volume != null && volume.profile.TryGet(out dof))
        {
            dof.active = false;
        }
        else
        {
            Debug.LogWarning("Depth of Field not found in Volume Profile!");
        }
    }

    public void EnableBlur(float start = 0f, float end = 5f)
    {
        if (dof != null)
        {
            dof.gaussianStart.value = start;
            dof.gaussianEnd.value = end;
            dof.active = true;
        }
    }

    public void DisableBlur()
    {
        if (dof != null)
        {
            dof.active = false;
        }
    }
}
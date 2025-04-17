using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BlurController : MonoBehaviour
{
    public static BlurController Instance;
    public Volume postProcessingVolume;
    private DepthOfField dof;

    void Awake()
    {
        Instance = this;
        postProcessingVolume.profile.TryGet(out dof);
    }

    public void EnableBlur()
    {
        if (dof == null) return;
        dof.active = true;
        dof.mode.value = DepthOfFieldMode.Gaussian;
        dof.gaussianStart.value = 0f;
        dof.gaussianEnd.value = 5f;
    }

    public void DisableBlur()
    {
        if (dof != null) dof.active = false;
    }
}
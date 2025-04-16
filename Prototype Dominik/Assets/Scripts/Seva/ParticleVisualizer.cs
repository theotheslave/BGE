using UnityEngine;

public class ParticleVisualizer : MonoBehaviour
{
    [SerializeField] private Gradient speedGradient;
    [SerializeField] private float maxSpeedForColor = 5f;

    private Rigidbody rb;
    private Renderer rend;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rend = GetComponent<Renderer>();

        if (speedGradient.colorKeys.Length == 0)
        {
            // Default gradient from blue (slow) to red (fast)
            GradientColorKey[] colorKeys = new GradientColorKey[2];
            colorKeys[0].color = Color.blue;
            colorKeys[0].time = 0f;
            colorKeys[1].color = Color.red;
            colorKeys[1].time = 1f;

            GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
            alphaKeys[0].alpha = 1f;
            alphaKeys[0].time = 0f;
            alphaKeys[1].alpha = 1f;
            alphaKeys[1].time = 1f;

            speedGradient.SetKeys(colorKeys, alphaKeys);
        }
    }

    private void Update()
    {
        if (rb != null && rend != null)
        {
            float normalizedSpeed = Mathf.Clamp01(rb.linearVelocity.magnitude / maxSpeedForColor);
            rend.material.color = speedGradient.Evaluate(normalizedSpeed);
        }
    }
}
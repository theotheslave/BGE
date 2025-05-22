using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MoleculeParticle : MonoBehaviour
{
    public float baseSpeed = 3f;

    Rigidbody rb;
    Renderer rend;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rend = GetComponent<Renderer>();
    }

   

    public void InitializeVelocity(float temperatureK)
    {
        float speed = baseSpeed * Mathf.Sqrt(temperatureK / 273f);
        rb.linearVelocity = Random.onUnitSphere * speed;
        UpdateColor(temperatureK);
    }

   

    public void AdjustSpeed(float temperatureK)
    {
        float targetSpeed = baseSpeed * Mathf.Sqrt(temperatureK / 273f);
        Vector3 dir = rb.linearVelocity.sqrMagnitude < 0.0001f
                      ? Random.onUnitSphere
                      : rb.linearVelocity.normalized;

        rb.linearVelocity = dir * targetSpeed;
        UpdateColor(temperatureK);
    }


    void OnCollisionEnter(Collision c)
    {
        if (c.gameObject.CompareTag("Wall"))
        {
            FindAnyObjectByType<Spawner>()?.RemoveMolecule(this);
            Destroy(gameObject);
            return;
        }

        // Tiny random nudge to avoid perfect reflections
        rb.linearVelocity += Random.onUnitSphere * 0.3f;
    }

    

    void UpdateColor(float temperatureK)
    {
        if (!rend) return;
        float t = Mathf.InverseLerp(273f, 800f, temperatureK);
        rend.material.color = Color.Lerp(Color.blue, Color.red, t);
    }
}

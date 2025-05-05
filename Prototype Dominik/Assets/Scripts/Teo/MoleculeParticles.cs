using UnityEngine;

public class MoleculeParticle : MonoBehaviour
{
    private Rigidbody2D rb;
    public float baseSpeed = 3f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Called at spawn: sets a fresh random direction + speed
    public void InitializeVelocity(float temperatureK)
    {
        float newSpeed = baseSpeed * Mathf.Sqrt(temperatureK / 273f);
        Vector2 direction = Random.insideUnitCircle.normalized;
        rb.linearVelocity = direction * newSpeed;
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
      
        if (Random.value < 1f)
        {
            Vector2 randomNudge = Random.insideUnitCircle.normalized * 1f;
            rb.linearVelocity += randomNudge;
        }
    }

  
    public void AdjustSpeed(float temperatureK)
    {
        if (rb == null) return;

        float targetSpeed = baseSpeed * Mathf.Sqrt(temperatureK / 273f);
        float currentSpeed = rb.linearVelocity.magnitude;

        Vector2 direction = rb.linearVelocity.normalized;

        
        if (currentSpeed < 0.05f)
        {
            direction = Random.insideUnitCircle.normalized;
            rb.linearVelocity = direction * targetSpeed;
            return;
        }

        if (currentSpeed < targetSpeed * 0.95f)
        {
            rb.linearVelocity = direction * targetSpeed;
        }
    }
}
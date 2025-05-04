using UnityEngine;

public class MoleculeParticle : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    public float baseSpeed = 3f;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    // Called at spawn: sets a fresh random direction + speed
    public void InitializeVelocity(float temperatureK)
    {
        float newSpeed = baseSpeed * Mathf.Sqrt(temperatureK / 273f);
        Vector2 direction = Random.insideUnitCircle.normalized;
        rb.linearVelocity = direction * newSpeed;
        UpdateColor(temperatureK);
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            // Tell the Spawner we're removing this molecule
            Spawner spawner = FindAnyObjectByType<Spawner>();
            if (spawner != null)
            {
                spawner.RemoveMolecule(this);
            }

            Destroy(gameObject);
        }

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
            UpdateColor(temperatureK);
            return;
        }

        if (currentSpeed < targetSpeed * 0.95f)
        {
            rb.linearVelocity = direction * targetSpeed;
        }
       

        UpdateColor(temperatureK);
    }


    private void UpdateColor(float temperatureK)
    {
        if (sr == null) return;

        float t = Mathf.InverseLerp(273f, 800f, temperatureK);
        sr.color = Color.Lerp(Color.blue, Color.red, t);
    }
}
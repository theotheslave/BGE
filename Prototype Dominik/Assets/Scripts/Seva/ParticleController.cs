using UnityEngine;

public class ParticleController : MonoBehaviour
{
    [SerializeField] private float initialSpeed;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody>();

        initialSpeed = rb.linearVelocity.magnitude;
    }

    private void FixedUpdate()
    {
        // Optional: Maintain speed/energy by normalizing velocity periodically
        if (rb.linearVelocity.magnitude > 0)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * initialSpeed;
        }

        // Optional: Add small random forces for more realistic gas behavior
        if (UnityEngine.Random.value < 0.01f)
        {
            Vector2 randomForce = UnityEngine.Random.insideUnitCircle * 0.1f;
            rb.AddForce(new Vector3(randomForce.x, randomForce.y, 0), ForceMode.Impulse);
        }
    }
}
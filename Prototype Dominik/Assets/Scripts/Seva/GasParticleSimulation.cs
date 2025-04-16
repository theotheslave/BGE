using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;
using System;

public class GasParticleSimulation : MonoBehaviour
{
    [Header("Particle Settings")]
    [SerializeField] private GameObject particlePrefab;
    [SerializeField] private int initialParticleCount = 50;
    [SerializeField] private float minInitialSpeed = 1f;
    [SerializeField] private float maxInitialSpeed = 3f;
    [SerializeField] private float containerWidth = 10f;
    [SerializeField] private float containerHeight = 8f;
    [SerializeField] private int particlesPerSpawn = 10;

    [Header("UI References")]
    [SerializeField] private Button spawnButton;
    [SerializeField] private Text particleCountText;

    private List<GameObject> particles = new List<GameObject>();
    private Transform particlesParent;

    private void Start()
    {
        // Create a parent object to keep hierarchy clean
        particlesParent = new GameObject("Particles").transform;

        // Setup UI button
        if (spawnButton != null)
        {
            spawnButton.onClick.AddListener(SpawnParticles);
        }

        // Create container boundaries
        CreateContainer();

        // Spawn initial particles
        SpawnInitialParticles();

        // Update particle count display
        UpdateParticleCountDisplay();
    }

    private void CreateContainer()
    {
        // Create walls using primitive cubes
        CreateWall(new Vector3(0, -containerHeight / 2 - 0.5f, 0), new Vector3(containerWidth + 1, 1, 1)); // Bottom
        CreateWall(new Vector3(0, containerHeight / 2 + 0.5f, 0), new Vector3(containerWidth + 1, 1, 1)); // Top
        CreateWall(new Vector3(-containerWidth / 2 - 0.5f, 0, 0), new Vector3(1, containerHeight, 1)); // Left
        CreateWall(new Vector3(containerWidth / 2 + 0.5f, 0, 0), new Vector3(1, containerHeight, 1)); // Right
    }

    private void CreateWall(Vector3 position, Vector3 scale)
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.transform.position = position;
        wall.transform.localScale = scale;
        wall.GetComponent<Renderer>().material.color = Color.gray;
        wall.name = "Wall";

        // Add physics properties
        Rigidbody rb = wall.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    private void SpawnInitialParticles()
    {
        for (int i = 0; i < initialParticleCount; i++)
        {
            SpawnParticle();
        }
    }

    public void SpawnParticles()
    {
        for (int i = 0; i < particlesPerSpawn; i++)
        {
            SpawnParticle();
        }
        UpdateParticleCountDisplay();
    }

    private void SpawnParticle()
    {
        // Calculate random position within the container
        float posX = Random.Range(-containerWidth / 2 + 0.5f, containerWidth / 2 - 0.5f);
        float posY = Random.Range(-containerHeight / 2 + 0.5f, containerHeight / 2 - 0.5f);

        GameObject particle = Instantiate(particlePrefab, new Vector3(posX, posY, 0), Quaternion.identity, particlesParent);
        particle.name = "Particle_" + particles.Count;

        // Get or add Rigidbody component
        Rigidbody rb = particle.GetComponent<Rigidbody>();
        if (rb == null)
            rb = particle.AddComponent<Rigidbody>();

        // Configure physics properties
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        // Add random initial velocity
        float angle = Random.Range(0, 2 * Mathf.PI);
        float speed = Random.Range(minInitialSpeed, maxInitialSpeed);
        Vector3 velocity = new Vector3(Mathf.Cos(angle) * speed, Mathf.Sin(angle) * speed, 0);
        rb.velocity = velocity;

        // Add to list
        particles.Add(particle);
    }

    private void UpdateParticleCountDisplay()
    {
        if (particleCountText != null)
        {
            particleCountText.text = "Particles: " + particles.Count;
        }
    }
}

// Add this script to your particles if you want them to maintain energy
public class ParticleController : MonoBehaviour
{
    [SerializeField] private float initialSpeed;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody>();

        initialSpeed = rb.velocity.magnitude;
    }

    private void FixedUpdate()
    {
        // Optional: Maintain speed/energy by normalizing velocity periodically
        if (rb.velocity.magnitude > 0)
        {
            rb.velocity = rb.velocity.normalized * initialSpeed;
        }

        // Optional: Add small random forces for more realistic gas behavior
        if (Random.value < 0.01f)
        {
            Vector2 randomForce = Random.insideUnitCircle * 0.1f;
            rb.AddForce(new Vector3(randomForce.x, randomForce.y, 0), ForceMode.Impulse);
        }
    }
}

// Create this script to implement particle color changes based on speed
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
            float normalizedSpeed = Mathf.Clamp01(rb.velocity.magnitude / maxSpeedForColor);
            rend.material.color = speedGradient.Evaluate(normalizedSpeed);
        }
    }
}
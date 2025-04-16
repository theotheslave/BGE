using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GasParticleSimulation : MonoBehaviour
{
    [Header("Particle Settings")]
    [SerializeField] private GameObject particlePrefab;
///    [SerializeField] private GameObject coalPrefab;
    [SerializeField] private int initialParticleCount = 50;
    [SerializeField] private float minInitialSpeed = 1f;
    [SerializeField] private float maxInitialSpeed = 3f;
    [SerializeField] private float containerWidth = 1f;
    [SerializeField] private float containerHeight = 1f;
    [SerializeField] private int particlesPerSpawn = 10;


    [Header("UI References")]
    [SerializeField] private Button spawnButton;
    [SerializeField] private UnityEngine.UI.Text particleCountText;

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
 //       CreateContainer();

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
//        Instantiate(coalPrefab, new Vector3(0f, 1f, 0f), Quaternion.Euler(0f, 90f, 0f));
        // Calculate random position within the container
        float posX = UnityEngine.Random.Range(-containerWidth / 2 + 0.5f, containerWidth / 2 - 0.5f);
        float posY = UnityEngine.Random.Range(-containerHeight / 2 + 0.5f, containerHeight / 2 - 0.5f);

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
        float angle = UnityEngine.Random.Range(0, 2 * Mathf.PI);
        float speed = UnityEngine.Random.Range(minInitialSpeed, maxInitialSpeed);
        Vector3 velocity = new Vector3(Mathf.Cos(angle) * speed, Mathf.Sin(angle) * speed, 0);
        rb.linearVelocity = velocity;

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
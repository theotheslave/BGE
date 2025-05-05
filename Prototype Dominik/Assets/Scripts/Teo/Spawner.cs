using UnityEngine;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
    public GameObject moleculePrefab;
    public int count = 20;
    public float pistonMinY = -1.3f;
    public float pistonMaxY = 2.3f;
    public Transform containerArea;

    [HideInInspector] public float currentTemperature = 273f;

    private List<MoleculeParticle> molecules = new List<MoleculeParticle>();

    void Update()
    {
       
        if (Time.frameCount % 60 == 0)
        {
            DebugAverageSpeed();
        }
    }

    public void SpawnMolecules(float initialTemp)
    {
        for (int i = 0; i < count; i++)
        {
            Vector2 spawnPos = new Vector2(
                Random.Range(containerArea.position.x - containerArea.localScale.x / 1f + 0.2f,
                             containerArea.position.x + containerArea.localScale.x / 1f - 0.2f),
                Random.Range(containerArea.position.y - containerArea.localScale.y / 1f + 0.2f,
                             pistonMinY - 0.1f)
            );

            GameObject mol = Instantiate(moleculePrefab, spawnPos, Quaternion.identity, containerArea);
            var particle = mol.GetComponent<MoleculeParticle>();
            particle.InitializeVelocity(initialTemp);
            molecules.Add(particle);
        }
    }

    public void ApplyTemperature(float temp)
    {
        foreach (var mol in molecules)
        {
            mol.AdjustSpeed(temp);
        }
    }

    public void ResetMolecules(float temp)
    {
        foreach (var mol in molecules)
        {
            if (mol != null)
                Destroy(mol.gameObject);
        }

        molecules.Clear();
        SpawnMolecules(temp);
    }

    
    void DebugAverageSpeed()
    {
        if (molecules == null || molecules.Count == 0) return;

        float totalSpeed = 0f;

        foreach (var mol in molecules)
        {
            if (mol == null) continue;
            Rigidbody2D rb = mol.GetComponent<Rigidbody2D>();
            totalSpeed += rb.linearVelocity.magnitude;
        }

        float avgSpeed = totalSpeed / molecules.Count;
        Debug.Log($"[Molecule Debug] Average Speed: {avgSpeed:F2}");
    }
}
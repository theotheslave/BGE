using UnityEngine;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
    public GameObject moleculePrefab;
    public int count = 300;
    public float pistonMinY = -1.3f;
    public float pistonMaxY = 2.3f;
    public Transform containerArea;

    [HideInInspector] public float currentTemperature = 273f;
    [HideInInspector] public int startMoleculeCount = 0;

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
        molecules.Clear();

        for (int i = 0; i < count; i++)
        {
            Vector2 spawnPos = new Vector2(
                Random.Range(containerArea.position.x - containerArea.localScale.x / 2f + 0.2f,
                             containerArea.position.x + containerArea.localScale.x / 2f - 0.2f),
                pistonMinY - 0.3f // inside chamber
            );

            GameObject mol = Instantiate(moleculePrefab, spawnPos, Quaternion.identity, containerArea);
            var particle = mol.GetComponent<MoleculeParticle>();
            particle.InitializeVelocity(initialTemp);
            molecules.Add(particle);
        }

        startMoleculeCount = molecules.Count;
    }

    public void ApplyTemperature(float temp)
    {
        foreach (var mol in molecules)
        {
            if (mol != null)
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

    public int GetActiveMoleculeCount()
    {
        // Exclude nulls for destroyed molecules
        molecules.RemoveAll(m => m == null);
        return molecules.Count;
    }

    public int TotalSpawnedCount()
    {
        // Count includes active and removed
        return molecules.Count;
    }

    public void AddNewMolecules(int numberToAdd, float temp)
    {
        for (int i = 0; i < numberToAdd; i++)
        {
            Vector2 spawnPos = new Vector2(
                Random.Range(containerArea.position.x - containerArea.localScale.x / 2f + 0.2f,
                             containerArea.position.x + containerArea.localScale.x / 2f - 0.2f),
                pistonMinY - 0.5f
            );

            GameObject mol = Instantiate(moleculePrefab, spawnPos, Quaternion.identity, containerArea);
            var particle = mol.GetComponent<MoleculeParticle>();
            particle.InitializeVelocity(temp);
            molecules.Add(particle);
        }
    }

    public void RemoveMolecule(MoleculeParticle mol)
    {
        if (mol != null)
            molecules.Remove(mol);
    }

    private void DebugAverageSpeed()
    {
        if (molecules == null || molecules.Count == 0) return;

        float totalSpeed = 0f;
        int count = 0;

        foreach (var mol in molecules)
        {
            if (mol == null) continue;
            Rigidbody2D rb = mol.GetComponent<Rigidbody2D>();
            totalSpeed += rb.linearVelocity.magnitude;
            count++;
        }

        if (count > 0)
        {
            float avgSpeed = totalSpeed / count;
            Debug.Log($"[Molecule Debug] Average Speed: {avgSpeed:F2}");
        }
    }
}
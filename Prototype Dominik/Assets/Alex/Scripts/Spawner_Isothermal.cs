using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider))]

public class Spawner_Isothermal : MonoBehaviour
{
    [Header("Prefabs & Counts")]
    public GameObject moleculePrefab;
    public int maxMoleculeCount = 300;

    [Header("Chamber limits")]
    public float pistonMinY = -1.3f;
    public float pistonMaxY = 2.3f;
    public float wallMargin = 0.2f;

    [Header("Runtime")]
    [HideInInspector] public float currentTemperature = 273f;

    private readonly List<MoleculeParticle> molecules = new();
    private BoxCollider volume;



    void Awake() => volume = GetComponent<BoxCollider>();
    void Start() => SpawnMolecules(maxMoleculeCount, currentTemperature);

    public void SpawnMolecules(int count, float temperatureK)
    {
        ClearAll();
        for (int i = 0; i < count; i++)
            molecules.Add(InstantiateOne(temperatureK));
        Debug.Log($"Spawning {count} molecules at T = {currentTemperature}");
    }
    MoleculeParticle InstantiateOne(float temperatureK)
    {
        Vector3 pos = RandomPointInside(volume, wallMargin);
        GameObject go = Instantiate(moleculePrefab, pos, Random.rotation, transform);
        var mol = go.GetComponent<MoleculeParticle>();
        mol.InitializeVelocity(temperatureK);
        return mol;
    }

    public void UpdateConditions(float temperatureK, float normalizedVolume, float normalizedMoles)
    {
        currentTemperature = temperatureK;

        float volumeFactor = Mathf.Clamp01(1.0f - normalizedVolume);
        float moleFactor = Mathf.Clamp01(normalizedMoles);

        int targetCount = Mathf.RoundToInt(maxMoleculeCount * moleFactor * (0.5f + volumeFactor));

        AdjustMoleculeCount(targetCount);
        foreach (var m in molecules)
            if (m != null) m.AdjustSpeed(temperatureK);
    }
    void AdjustMoleculeCount(int targetCount)
    {
        molecules.RemoveAll(m => m == null);

        int current = molecules.Count;

        if (current < targetCount)
        {
            for (int i = 0; i < targetCount - current; i++)
                molecules.Add(InstantiateOne(currentTemperature));
        }
        else if (current > targetCount)
        {
            int excess = current - targetCount;
            for (int i = 0; i < molecules.Count && excess > 0; i++)
            {
                if (molecules[i] != null)
                {
                    Destroy(molecules[i].gameObject);
                    excess--;
                }
            }
            molecules.RemoveAll(m => m == null);
        }
    }

    void ClearAll()
    {
        foreach (var m in molecules)
            if (m) Destroy(m.gameObject);
        molecules.Clear();
    }

    public void RemoveMolecule(MoleculeParticle m)
    {
        molecules.Remove(m);
    }

    public int ActiveCount()
    {
        molecules.RemoveAll(m => m == null);
        return molecules.Count;
    }

    private static Vector3 RandomPointInside(BoxCollider box, float margin)
    {
        Vector3 localPos = new Vector3(
            Random.Range(-0.5f + margin, 0.5f - margin),
            Random.Range(-0.5f + margin, 0.5f - margin),
            Random.Range(-0.5f + margin, 0.5f - margin)
        );
        Vector3 scaledPos = Vector3.Scale(localPos, box.size);
        return box.transform.TransformPoint(box.center + scaledPos);
    }
}

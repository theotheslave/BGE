using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider))]
public class Spawner : MonoBehaviour
{
    [Header("Prefabs & Counts")]
    public GameObject moleculePrefab;
    public int maxMoleculeCount = 300;

    [Header("Chamber limits")]
    //public float pistonMinY = -1.3f;
    //public float pistonMaxY = 2.3f;
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
    }

    public void UpdateConditions(float temperatureK, float normalizedVolume, float moleFraction)
    {
        currentTemperature = temperatureK;
        foreach (var m in molecules)
            if (m) m.AdjustSpeed(temperatureK);

        int desiredCount = Mathf.Clamp(Mathf.RoundToInt(maxMoleculeCount * moleFraction), 1, maxMoleculeCount);
        for (int i = 0; i < molecules.Count; i++)
        {
            if (molecules[i])
                molecules[i].gameObject.SetActive(i < desiredCount);
        }

        float scale = Mathf.Lerp(1.5f, 0.5f, normalizedVolume);
        transform.localScale = new Vector3(scale, scale, scale);
    }

    public int ActiveCount()
    {
        molecules.RemoveAll(m => m == null);
        return molecules.Count;
    }

    public void RemoveMolecule(MoleculeParticle m)
    {
        if (molecules.Contains(m))
        {
            molecules.Remove(m);
            Destroy(m.gameObject);
        }
    }

    private MoleculeParticle InstantiateOne(float temperatureK)
    {
        Vector3 pos = RandomPointInside(volume, wallMargin);
        GameObject go = Instantiate(moleculePrefab, pos, Random.rotation, transform);
        var mol = go.GetComponent<MoleculeParticle>();
        mol.InitializeVelocity(temperatureK);
        return mol;
    }

    private void ClearAll()
    {
        foreach (var m in molecules)
            if (m) Destroy(m.gameObject);
        molecules.Clear();
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

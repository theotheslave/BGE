using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider))]
public class Spawner : MonoBehaviour
{
    [Header("Prefabs & Counts")]
    public GameObject moleculePrefab;
    public int startCount = 300;

    [Header("Chamber limits")]
    public float pistonMinY = -1.3f;
    public float pistonMaxY = 2.3f;
    public float wallMargin = 0.2f;           

    [Header("Runtime")]
    [HideInInspector] public float currentTemperature = 273f;

    private readonly List<MoleculeParticle> molecules = new();
    private BoxCollider volume;              

   

    void Awake() => volume = GetComponent<BoxCollider>();
    void Start() => SpawnMolecules(startCount, currentTemperature);

   

    public void SpawnMolecules(int count, float temperatureK)
    {
        ClearAll();

        for (int i = 0; i < count; i++)
            molecules.Add(InstantiateOne(temperatureK));
    }

    public void AddNewMolecules(int count, float temperatureK)
    {
        for (int i = 0; i < count; i++)
            molecules.Add(InstantiateOne(temperatureK));
    }

    public void ApplyTemperature(float temperatureK)
    {
        currentTemperature = temperatureK;
        foreach (var m in molecules)
            if (m) m.AdjustSpeed(temperatureK);
    }

    public int ActiveCount()
    {
        molecules.RemoveAll(m => m == null);
        return molecules.Count;
    }

    public void RemoveMolecule(MoleculeParticle m) => molecules.Remove(m);

  

    MoleculeParticle InstantiateOne(float temperatureK)
    {
        Vector3 pos = RandomPointInside(volume, wallMargin);
        GameObject go = Instantiate(moleculePrefab, pos, Random.rotation, transform);
        var mol = go.GetComponent<MoleculeParticle>();
        mol.InitializeVelocity(temperatureK);
        return mol;
    }
    void OnDrawGizmosSelected()
    {
        if (volume == null) volume = GetComponent<BoxCollider>();
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(volume.bounds.center, volume.bounds.size);
    }

    void ClearAll()
    {
        foreach (var m in molecules)
            if (m) Destroy(m.gameObject);
        molecules.Clear();
    }

    static Vector3 RandomPointInside(BoxCollider box, float margin)
    {
        Bounds bounds = box.bounds;
        Vector3 min = bounds.min + Vector3.one * margin;
        Vector3 max = bounds.max - Vector3.one * margin;

        return new Vector3(
            Random.Range(min.x, max.x),
            Random.Range(min.y, max.y),
            Random.Range(min.z, max.z)
        );
    }
}

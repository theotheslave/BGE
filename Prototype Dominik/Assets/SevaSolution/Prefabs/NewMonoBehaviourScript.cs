using System.Collections.Generic;
using UnityEngine;

public class PrefabSpawner : MonoBehaviour
{
    [Header("Prefab to spawn")]
    [Tooltip("Drag the prefab you want to spawn here.")]
    public GameObject prefabToSpawn;

    [Header("Spawn Settings")]
    [Tooltip("Optional parent under which all spawned prefabs will be organized.")]
    public Transform parentTransform;

    // Keeps track of all instances this spawner has created
    private readonly List<GameObject> _spawnedInstances = new List<GameObject>();

    /// <summary>
    /// Call this from your Button's OnClick.
    /// It will destroy all previously spawned instances, then instantiate a fresh copy.
    /// </summary>
    public void SpawnAndClear()
    {
        // 1) Destroy all existing instances
        foreach (var instance in _spawnedInstances)
        {
            if (instance != null)
                Destroy(instance);
        }
        _spawnedInstances.Clear();

        // 2) Instantiate the new prefab
        if (prefabToSpawn == null)
        {
            UnityEngine.Debug.LogError("PrefabSpawner: prefabToSpawn is not assigned!");
            return;
        }

        GameObject newInstance;
        if (parentTransform != null)
            newInstance = Instantiate(prefabToSpawn, parentTransform);
        else
            newInstance = Instantiate(prefabToSpawn);

        // 3) Keep track of it
        _spawnedInstances.Add(newInstance);
    }
}

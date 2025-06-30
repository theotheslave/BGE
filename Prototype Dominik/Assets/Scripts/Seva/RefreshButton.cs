using UnityEngine;

public class RefreshButton : MonoBehaviour
{
    public GameObject prefabToSpawn;     // Префаб, который нужно создать
    public Transform parentTransform;    // Родитель, под которым будет экземпляр

    private GameObject currentInstance;  // Храним текущий экземпляр

    void Start()
    {
        currentInstance = Instantiate(prefabToSpawn, parentTransform);
    }

    public void SpawnPrefab()
    {
        // Удаляем предыдущий экземпляр, если он существует
        if (currentInstance != null)
        {
            Destroy(currentInstance);
        }

        // Создаём новый экземпляр
        currentInstance = Instantiate(prefabToSpawn, parentTransform);
    }
}

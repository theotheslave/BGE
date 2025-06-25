using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    [Tooltip("Delay in seconds before this object destroys itself")]
    [SerializeField] private float delay = 1f;

    void Start()
    {
        // Schedule self-destruction
        Destroy(gameObject, delay);
    }
}

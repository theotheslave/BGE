using UnityEngine;
public class Test : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"TriggerTest saw: {other.name}");
    }
}
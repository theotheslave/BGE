using UnityEngine;
[RequireComponent(typeof(Collider))]
public class OutlineGoggles : MonoBehaviour
{
    private Outline outline;

    void Awake()
    {
        outline = GetComponent<Outline>();
        if (outline == null)
        {
            outline = gameObject.AddComponent<Outline>();
            outline.OutlineColor = Color.cyan;
            outline.OutlineWidth = 6f;
        }

        outline.enabled = false;
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.transform == transform)
            {
                if (!outline.enabled) outline.enabled = true;
                return;
            }
        }

        if (outline.enabled) outline.enabled = false;
    }
}
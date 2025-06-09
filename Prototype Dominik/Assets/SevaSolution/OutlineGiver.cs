using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class HoverOutline2D : MonoBehaviour
{
    [Tooltip("The Outline component that draws an outline when enabled")]
    public Outline outline;

    Collider2D col;

    void Awake()
    {
        // cache collider and start with no outline
        col = GetComponent<Collider2D>();
        if (outline != null) outline.enabled = false;
    }

    void Update()
    {
        // Convert mouse position to world point
        Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Check if we're hovering over our own collider
        bool isHovering = col.OverlapPoint(mouseWorld);

        // Enable/disable the outline accordingly
        if (outline != null)
            outline.enabled = isHovering;
    }
}

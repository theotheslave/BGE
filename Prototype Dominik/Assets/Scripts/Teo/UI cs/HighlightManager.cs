using UnityEngine;

public class HighlightManager : MonoBehaviour
{
    [SerializeField] private LayerMask interactableLayer;

    private Outline currentOutline;

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, interactableLayer))
        {
            Outline newOutline = hit.transform.GetComponent<Outline>();

            if (newOutline != null && newOutline != currentOutline)
            {
                ClearHighlight();

                currentOutline = newOutline;
                currentOutline.enabled = true;
            }
        }
        else
        {
            ClearHighlight();
        }
    }

    void ClearHighlight()
    {
        if (currentOutline != null)
        {
            currentOutline.enabled = false;
            currentOutline = null;
        }
    }
}

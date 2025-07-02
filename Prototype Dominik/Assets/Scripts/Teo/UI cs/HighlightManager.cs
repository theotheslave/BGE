//using UnityEngine;

//public class HighlightManager : MonoBehaviour
//{
//    [SerializeField] private LayerMask interactableLayer;

//    private Outline currentOutline;

//    void Update()
//    {
//        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

//        if (Physics.Raycast(ray, out RaycastHit hit, 100f, interactableLayer))
//        {
//            if (hit.transform.TryGetComponent(out Outline hitOutline))
//            {
//                if (hit.transform.TryGetComponent(out SelectableObject selectable) &&
//                    hit.transform == SelectableObject.CurrentSelectionTransform)
//                {
//                    return; // Let SelectableObject handle it
//                }

//                if (currentOutline != hitOutline)
//                {
//                    ClearHighlight();
//                    currentOutline = hitOutline;
//                    currentOutline.enabled = true;
//                }
//            }
//            else
//            {
//                ClearHighlight();
//            }
//        }
//        else
//        {
//            ClearHighlight();
//        }
//    }

//    void ClearHighlight()
//    {
//        if (currentOutline != null)
//        {
//            currentOutline.enabled = false;
//            currentOutline = null;
//        }
//    }
//}

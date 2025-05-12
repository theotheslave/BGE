using UnityEngine;
using TMPro;
using System.Linq;

public class LearnedFormulasUI : MonoBehaviour
{
    public GameObject formulaEntryPrefab; // Prefab with TMP label inside
    public Transform contentParent;       // ScrollView Content object

    public void RefreshList()
    {
        if (formulaEntryPrefab == null || contentParent == null)
        {
            Debug.LogError("LearnedFormulasUI: prefab or content is missing.");
            return;
        }

        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        foreach (string formulaID in FormulaUnlockManager.Instance.GetUnlockedFormulas().OrderBy(id => id))
        {
            GameObject entry = Instantiate(formulaEntryPrefab, contentParent);

            var label = entry.GetComponentInChildren<TextMeshProUGUI>();
            if (label != null)
                label.text = formulaID;
            else
                Debug.LogWarning("Prefab is missing TMP label.");

            var button = entry.GetComponent<UnityEngine.UI.Button>();
            if (button != null)
            {
                string idCopy = formulaID;
                button.onClick.AddListener(() => UIManager.Instance?.ShowFormulaDetails(idCopy));
            }
        }
    }
}
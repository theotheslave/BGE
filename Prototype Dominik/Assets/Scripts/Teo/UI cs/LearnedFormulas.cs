using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class LearnedFormulasUI : MonoBehaviour
{
    public GameObject formulaEntryPrefab;   // Must have Button + TMP Text inside
    public Transform contentParent;         // Content under Scroll View

    void OnEnable() => RefreshList();

    public void RefreshList()
    {
        Debug.Log("Refreshing learned formulas list...");

        if (formulaEntryPrefab == null)
        {
            Debug.LogError("❌ formulaEntryPrefab is null!");
            return;
        }

        if (contentParent == null)
        {
            Debug.LogError("❌ contentParent is null!");
            return;
        }

        Debug.Log("✅ Both references are assigned. Continuing...");

        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        foreach (var formulaID in FormulaUnlockManager.Instance.GetUnlockedFormulas().OrderBy(id => id))
        {
            Debug.Log("Instantiating entry for: " + formulaID);
            GameObject entry = Instantiate(formulaEntryPrefab, contentParent);

            var label = entry.GetComponentInChildren<TextMeshProUGUI>();
            if (label != null) label.text = formulaID;
            else Debug.LogError("❌ TMP label not found in prefab!");
        }
    }
}
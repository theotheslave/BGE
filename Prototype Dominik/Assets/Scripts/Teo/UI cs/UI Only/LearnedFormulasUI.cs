using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class LearnedFormulasUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject formulaEntryPrefab; // Prefab with FormulaCardView, DraggableCard, Image
    public Transform contentParent;       // The Content object of your ScrollView

    void OnEnable()
    {
        RefreshList();
    }

    public void RefreshList()
    {
        if (formulaEntryPrefab == null || contentParent == null)
        {
            Debug.LogError("LearnedFormulasUI: prefab or contentParent is not assigned.");
            return;
        }

        // Clear old entries
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        // Load and display each unlocked formula
        var unlocked = FormulaUnlockManager.Instance.GetUnlockedFormulas().OrderBy(id => id);
        Debug.Log("Refreshing learned formulas list: " + string.Join(", ", unlocked));

        foreach (string formulaID in unlocked)
        {
            FormulaCard card = Resources.Load<FormulaCard>($"FormulaCards/{formulaID}");
            if (card == null)
            {
                Debug.LogWarning($"Could not load FormulaCard for ID: {formulaID}");
                continue;
            }

            GameObject entry = Instantiate(formulaEntryPrefab, contentParent);

            var view = entry.GetComponent<FormulaCardView>();
            if (view != null)
            {
                view.cardData = card;
                entry.GetComponent<Image>().sprite = card.formulaSprite;
                entry.name = $"Card_{card.formulaID}";
            }
            else
            {
                Debug.LogWarning("Prefab is missing FormulaCardView component.");
            }
        }
    }
}

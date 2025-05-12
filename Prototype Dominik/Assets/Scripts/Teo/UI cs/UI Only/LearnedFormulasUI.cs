using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class LearnedFormulasUI : MonoBehaviour
{
    public GameObject formulaEntryPrefab; // Prefab must have FormulaCardView, DraggableCard, Image
    public Transform contentParent;       // ScrollView Content object

    public void RefreshList()
    {
        Debug.Log("Refreshing learned formulas list...");

        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        var unlocked = FormulaUnlockManager.Instance.GetUnlockedFormulas().ToList();
        Debug.Log("Unlocked: " + string.Join(", ", unlocked));

        foreach (string formulaID in unlocked)
        {
            Debug.Log($"Trying to load FormulaCard: {formulaID}");

            FormulaCard card = Resources.Load<FormulaCard>($"FormulaCards/{formulaID}");
            if (card == null)
            {
                Debug.LogError($"[LOAD FAIL] FormulaCard not found at FormulaCards/{formulaID}");
                continue;
            }

            GameObject entry = Instantiate(formulaEntryPrefab, contentParent);
            var view = entry.GetComponent<FormulaCardView>();
            if (view == null)
            {
                Debug.LogError("[VIEW FAIL] FormulaCardView component missing on prefab.");
                continue;
            }

            view.cardData = card;
            entry.GetComponent<Image>().sprite = card.formulaSprite;
            entry.name = $"Card_{card.formulaID}";
        }
    }
}
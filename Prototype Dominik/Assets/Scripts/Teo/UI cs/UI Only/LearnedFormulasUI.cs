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

            var view = entry.GetComponent<FormulaCardView>();
            if (view != null)
            {
                FormulaCard card = Resources.Load<FormulaCard>($"FormulaCards/{formulaID}");
                if (card != null)
                {
                    view.cardData = card;
                    entry.GetComponent<Image>().sprite = card.formulaSprite;
                    entry.name = $"Card_{card.formulaID}";
                }
                else
                {
                    Debug.LogWarning($"Could not load FormulaCard for ID: {formulaID}");
                }
            }
            else
            {
                Debug.LogWarning("Prefab is missing FormulaCardView component.");
            }
        }
    }
}
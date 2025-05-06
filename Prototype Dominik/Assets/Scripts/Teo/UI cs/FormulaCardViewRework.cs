using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(DraggableCardRework))]
public class FormulaCardViewRerwork : MonoBehaviour
{
    public FormulaCardRework cardData;
    private Image cardImage;

    void Awake() => cardImage = GetComponent<Image>();

    void Start() => ApplyCardData();

    public void ApplyCardData()
    {
        if (cardData != null && cardImage != null)
        {
            cardImage.sprite = cardData.formulaSprite;
            gameObject.name = $"Card_{cardData.formulaID}";
        }
    }
}
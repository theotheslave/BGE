using UnityEngine;
using UnityEngine.UI;

public class FormulaCardView : MonoBehaviour
{
    public FormulaCard cardData; // Drag your asset here in the Inspector
    public Image cardImage;      // UI Image to show the sprite

    void Start()
    {
        if (cardData != null && cardImage != null)
        {
            cardImage.sprite = cardData.formulaSprite;
        }
    }
}
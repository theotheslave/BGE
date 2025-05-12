using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class FormulaCardView : MonoBehaviour
{
    public FormulaCard cardData;

    void Start()
    {
        GetComponent<Image>().sprite = cardData.formulaSprite;
        name = "Card_" + cardData.formulaID;
    }
}

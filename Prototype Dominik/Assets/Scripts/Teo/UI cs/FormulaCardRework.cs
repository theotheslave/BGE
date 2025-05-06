using UnityEngine;

public enum CardType { Attack, Defense, Universal }

[CreateAssetMenu(fileName = "NewFormulaCard", menuName = "ThermoBattle/FormulaCard")]
public class FormulaCardRework : ScriptableObject
{
    public string formulaID;
    public Sprite formulaSprite;
    public int value;
    public CardType cardType;
}
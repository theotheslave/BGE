using UnityEngine;

public enum CardType { Attack, Defense, Universal }

[CreateAssetMenu(fileName = "NewFormulaCard", menuName = "ThermoBattle/FormulaCard")]
public class FormulaCard : ScriptableObject
{
    public string formulaID;
    public Sprite formulaSprite;
    public string correctSlotID;
    public int value;
    public CardType cardType;
}
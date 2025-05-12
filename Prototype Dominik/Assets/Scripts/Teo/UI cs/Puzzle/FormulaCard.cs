using UnityEngine;

[CreateAssetMenu(fileName = "NewFormulaCard", menuName = "Game/FormulaCard")]
public class FormulaCard : ScriptableObject
{
    public string formulaID;
    public string correctSlotID;
    public Sprite formulaSprite;
    public string description;
}
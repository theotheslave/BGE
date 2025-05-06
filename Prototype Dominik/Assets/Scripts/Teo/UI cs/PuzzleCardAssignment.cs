using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class PuzzleCardAssignment
{
    public FormulaCardRework card;
    public string targetSlotID;
}

[CreateAssetMenu(menuName = "ThermoBattle/Formula Puzzle Definition")]
public class FormulaPuzzleDefinition : ScriptableObject
{
    public string puzzleID;
    public List<PuzzleCardAssignment> cardAssignments = new();
}
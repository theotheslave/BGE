using UnityEngine;
using UnityEngine.UI;

public class TestButton : MonoBehaviour
{
    public string debugFormulaID = "Puzzle_Debug";
    public LearnedFormulasUI learnedUI;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            Debug.Log("Fake puzzle complete! Unlocking: " + debugFormulaID);

            FormulaUnlockManager.Instance.UnlockFormula(debugFormulaID);

            if (learnedUI != null)
            {
                learnedUI.RefreshList();
            }
        });
    }
}
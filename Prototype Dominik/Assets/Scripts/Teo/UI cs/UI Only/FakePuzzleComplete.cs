using UnityEngine;
using UnityEngine.UI;

public class FakePuzzleComplete : MonoBehaviour
{
    public string debugFormulaID = "Test_Formula_001";
    public LearnedFormulasUI learnedUI;

    void Start()
    {
        Button btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(() =>
            {
                FormulaUnlockManager.Instance.UnlockFormula(debugFormulaID);
                learnedUI?.RefreshList();
                UIManager.Instance?.ShowPuzzleFeedback("Unlocked: " + debugFormulaID);
            });
        }
    }
}

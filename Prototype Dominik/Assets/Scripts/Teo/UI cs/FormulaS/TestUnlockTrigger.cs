using UnityEngine;
using UnityEngine.UI;

public class TestUnlockTrigger : MonoBehaviour
{
    public string testFormulaID = "Test_Formula_001";
    public LearnedFormulasUI learnedFormulasUI;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            FormulaUnlockManager.Instance.UnlockFormula(testFormulaID);
            Debug.Log("Unlocked: " + testFormulaID);

            if (learnedFormulasUI != null)
            {
                learnedFormulasUI.RefreshList();
            }
        });
    }
}
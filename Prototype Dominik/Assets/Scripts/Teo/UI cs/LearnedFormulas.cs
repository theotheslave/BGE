using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class LearnedFormulasUI : MonoBehaviour
{
    public GameObject formulaEntryPrefab;
    public Transform contentParent;

    void OnEnable() => RefreshList();

    public void RefreshList()
    {
        foreach (Transform child in contentParent) Destroy(child.gameObject);
        foreach (var formulaID in FormulaUnlockManager.Instance.GetUnlockedFormulas().OrderBy(id => id))
        {
            GameObject entry = Instantiate(formulaEntryPrefab, contentParent);
            entry.GetComponentInChildren<Text>().text = formulaID;
            entry.GetComponent<Button>().onClick.AddListener(() => Debug.Log("Selected: " + formulaID));
        }
    }
}
using UnityEngine;
using System.Collections.Generic;

public class FormulaUnlockManager : MonoBehaviour
{
    public static FormulaUnlockManager Instance { get; private set; }
    private HashSet<string> unlockedFormulas = new();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void UnlockFormula(string formulaID)
    {
        if (unlockedFormulas.Add(formulaID))
        {
            Debug.Log("Unlocked formula: " + formulaID);
        }
    }
}
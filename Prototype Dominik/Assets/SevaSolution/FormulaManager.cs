using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FormulaManager : MonoBehaviour
{
    // Singleton instance
    public static FormulaManager Instance { get; private set; }

    [Header("Assign the four slot objects here (each has a DroppableTextTarget)")]
    public DroppableTextTarget slotP1;
    public DroppableTextTarget slotV1;
    public DroppableTextTarget slotP2;
    public DroppableTextTarget slotV2;

    [Header("Prefabs for displaying the computed result")]
    [Tooltip("A simple UI Text prefab. At runtime, we will set its .text to the variable name (e.g. \"P1\").")]
    public GameObject variableLabelPrefab;

    [Tooltip("A UI Text or Image prefab showing \"=\". This is the equals‐sign that goes in between.")]
    public GameObject equalsSignPrefab;

    [Tooltip("A simple UI Text prefab. At runtime, we will set its .text to the computed numeric result.")]
    public GameObject resultTextPrefab;

    [Header("Where should the trio (var = result) appear?")]
    [Tooltip("Any RectTransform (e.g. an empty GameObject with a HorizontalLayoutGroup) under which we'll parent the new elements.")]
    public RectTransform resultArea;

    // Internal storage: each variable name -> its current text (letter or number)
    private Dictionary<string, string> _slotTexts = new Dictionary<string, string>()
    {
        { "P1", null },
        { "V1", null },
        { "P2", null },
        { "V2", null }
    };

    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("FormulaManager: Another instance detected; destroying this one.");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        Debug.Log("FormulaManager: Awake() - Singleton instance set.");

        // Quick null-check on assigned slots
        if (slotP1 == null || slotV1 == null || slotP2 == null || slotV2 == null)
        {
            Debug.LogError("FormulaManager: One or more slot references (P1, V1, P2, V2) are not assigned in the Inspector.");
        }
        else
        {
            Debug.Log("FormulaManager: All four slot references are assigned.");
        }

        // Quick null-check on prefabs and resultArea
        if (variableLabelPrefab == null) Debug.LogError("FormulaManager: variableLabelPrefab is not assigned.");
        if (equalsSignPrefab == null) Debug.LogError("FormulaManager: equalsSignPrefab is not assigned.");
        if (resultTextPrefab == null) Debug.LogError("FormulaManager: resultTextPrefab is not assigned.");
        if (resultArea == null) Debug.LogError("FormulaManager: resultArea is not assigned.");
    }

    /// <summary>
    /// Called by each DroppableTextTarget when a slot's text changes.
    /// </summary>
    public void OnSlotUpdated(string variableName, string newText)
    {
        Debug.Log($"FormulaManager: OnSlotUpdated called for {variableName} with newText = \"{newText}\"");

        // Update internal dictionary
        if (_slotTexts.ContainsKey(variableName))
        {
            _slotTexts[variableName] = newText;
        }
        else
        {
            Debug.LogError($"FormulaManager: OnSlotUpdated called with unknown variableName \"{variableName}\"");
            return;
        }

        // Log current dictionary state
        Debug.Log("FormulaManager: Current slot values:");
        foreach (var kvp in _slotTexts)
        {
            Debug.Log($"  {kvp.Key} => \"{kvp.Value}\"");
        }

        // Attempt to solve
        TrySolveIfReady();
    }

    /// <summary>
    /// Checks how many slots are numeric vs. non‐numeric. If exactly one is missing and three are numeric,
    /// compute and display the result (but do not overwrite the original slot).
    /// </summary>
    private void TrySolveIfReady()
    {
        Debug.Log("FormulaManager: TrySolveIfReady() called.");

        // 1) Attempt to parse each filled string into a float
        var numericValues = new Dictionary<string, float>();
        var nonNumericSlots = new List<string>();

        foreach (var kvp in _slotTexts)
        {
            string key = kvp.Key;
            string textValue = kvp.Value;

            if (string.IsNullOrEmpty(textValue))
            {
                nonNumericSlots.Add(key);
                Debug.Log($"  [{key}] is empty or null => non‐numeric");
            }
            else
            {
                if (float.TryParse(textValue, out float parsed))
                {
                    numericValues[key] = parsed;
                    Debug.Log($"  [{key}] parsed as {parsed}");
                }
                else
                {
                    nonNumericSlots.Add(key);
                    Debug.Log($"  [{key}] \"{textValue}\" is not a valid float => non‐numeric");
                }
            }
        }

        Debug.Log($"FormulaManager: numeric count = {numericValues.Count}, non‐numeric count = {nonNumericSlots.Count}");

        // We need exactly 3 numeric and 1 non‐numeric to proceed
        if (nonNumericSlots.Count == 1 && numericValues.Count == 3)
        {
            string missingVar = nonNumericSlots[0];
            Debug.Log($"FormulaManager: Exactly one missing slot: {missingVar}");

            // Compute the missing variable
            float resultValue = ComputeMissingVariable(missingVar, numericValues);
            Debug.Log($"FormulaManager: Computed {missingVar} = {resultValue}");

            // Display the result in the ResultArea
            CreateResultLine(missingVar, resultValue);

            // NOTE: We intentionally do NOT overwrite the missing slot’s Text.
            // The original letter remains visible.
        }
        else
        {
            Debug.Log("FormulaManager: Not ready to solve yet (need exactly 3 numeric and 1 non‐numeric).");
        }
    }

    /// <summary>
    /// Computes the value of the missing variable from P1·V1 = P2·V2.
    /// </summary>
    private float ComputeMissingVariable(string missingVar, Dictionary<string, float> numericValues)
    {
        numericValues.TryGetValue("P1", out float p1);
        numericValues.TryGetValue("V1", out float v1);
        numericValues.TryGetValue("P2", out float p2);
        numericValues.TryGetValue("V2", out float v2);

        Debug.Log($"ComputeMissingVariable: Received numeric values: P1={p1}, V1={v1}, P2={p2}, V2={v2}");

        switch (missingVar)
        {
            case "P1":
                float calcP1 = (p2 * v2) / v1;
                Debug.Log($"ComputeMissingVariable: P1 = (P2·V2)/V1 = ({p2}*{v2})/{v1} = {calcP1}");
                return calcP1;

            case "V1":
                float calcV1 = (p2 * v2) / p1;
                Debug.Log($"ComputeMissingVariable: V1 = (P2·V2)/P1 = ({p2}*{v2})/{p1} = {calcV1}");
                return calcV1;

            case "P2":
                float calcP2 = (p1 * v1) / v2;
                Debug.Log($"ComputeMissingVariable: P2 = (P1·V1)/V2 = ({p1}*{v1})/{v2} = {calcP2}");
                return calcP2;

            case "V2":
                float calcV2 = (p1 * v1) / p2;
                Debug.Log($"ComputeMissingVariable: V2 = (P1·V1)/P2 = ({p1}*{v1})/{p2} = {calcV2}");
                return calcV2;

            default:
                Debug.LogError($"ComputeMissingVariable: Unknown key \"{missingVar}\"");
                return 0f;
        }
    }

    /// <summary>
    /// Instantiates three UI elements under resultArea:
    /// [ variable label ], [ equals sign ], [ numeric result ].
    /// </summary>
    private void CreateResultLine(string missingVar, float resultValue)
    {
        Debug.Log($"CreateResultLine: Attempting to display: {missingVar} = {resultValue}");

        if (resultArea == null)
        {
            Debug.LogError("CreateResultLine: resultArea is not assigned in the Inspector!");
            return;
        }
        if (variableLabelPrefab == null || equalsSignPrefab == null || resultTextPrefab == null)
        {
            Debug.LogError("CreateResultLine: One or more prefabs (label, equals, resultText) are not assigned.");
            return;
        }

        // Instantiate variable label
        GameObject varLabelGO = Instantiate(variableLabelPrefab, resultArea);
        Text varLabelText = varLabelGO.GetComponent<Text>();
        if (varLabelText != null)
        {
            varLabelText.text = missingVar;
            Debug.Log($"CreateResultLine: Instantiated variable label and set text to \"{missingVar}\"");
        }
        else
        {
            Debug.LogWarning("CreateResultLine: variableLabelPrefab has no Text component; cannot set variable name.");
        }

        // Instantiate equals sign
        GameObject eqGO = Instantiate(equalsSignPrefab, resultArea);
        Debug.Log("CreateResultLine: Instantiated equals sign prefab.");

        // Instantiate result text
        GameObject resultGO = Instantiate(resultTextPrefab, resultArea);
        Text resultText = resultGO.GetComponent<Text>();
        if (resultText != null)
        {
            resultText.text = resultValue.ToString();
            Debug.Log($"CreateResultLine: Instantiated result text and set text to \"{resultValue}\"");
        }
        else
        {
            Debug.LogWarning("CreateResultLine: resultTextPrefab has no Text component; cannot set numeric value.");
        }
    }

    /// <summary>
    /// Helper to find the DroppableTextTarget component by variable name.
    /// </summary>
    private DroppableTextTarget GetDroppedTargetByName(string varName)
    {
        switch (varName)
        {
            case "P1": return slotP1;
            case "V1": return slotV1;
            case "P2": return slotP2;
            case "V2": return slotV2;
            default: return null;
        }
    }
}

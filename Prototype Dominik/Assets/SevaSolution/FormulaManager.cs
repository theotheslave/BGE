using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class FormulaManager : MonoBehaviour
{
    public static FormulaManager Instance { get; private set; }

    [Header("PV slots (P₁·V₁ = P₂·V₂)")]
    public DroppableTextTarget PV_P1, PV_V1, PV_P2, PV_V2;

    [Header("PT slots (P₁/T₁ = P₂/T₂)")]
    public DroppableTextTarget PT_P1, PT_T1, PT_P2, PT_T2;

    [Header("VT slots (V₁/T₁ = V₂/T₂)")]
    public DroppableTextTarget VT_V1, VT_T1, VT_V2, VT_T2;

    [Header("IG slots (P·V = n·R·T)")]
    public DroppableTextTarget IG_P, IG_V, IG_n, IG_T, IG_R;

    [Header("Result Areas (one per formula)")]
    public RectTransform resultAreaPV;
    public RectTransform resultAreaPT;
    public RectTransform resultAreaVT;
    public RectTransform resultAreaIG;

    [Header("Result‐line Prefabs")]
    public GameObject variableLabelPrefab;
    public GameObject equalsSignPrefab;
    public GameObject resultTextPrefab;

    // parsed numbers + suffixes
    private readonly Dictionary<string, float> _nums = new Dictionary<string, float>();
    private readonly Dictionary<string, string> _suffix = new Dictionary<string, string>();

    // track solved
    private bool _solvedPV, _solvedPT, _solvedVT, _solvedIG;

    // default units
    private readonly Dictionary<string, string> _unitPV = new Dictionary<string, string>
    {
        {"P1","Pa"}, {"V1","L"}, {"P2","Pa"}, {"V2","L"}
    };
    private readonly Dictionary<string, string> _unitPT = new Dictionary<string, string>
    {
        {"P1","Pa"}, {"T1","K"}, {"P2","Pa"}, {"T2","K"}
    };
    private readonly Dictionary<string, string> _unitVT = new Dictionary<string, string>
    {
        {"V1","L"}, {"T1","K"}, {"V2","L"}, {"T2","K"}
    };
    private readonly Dictionary<string, string> _unitIG = new Dictionary<string, string>
    {
        {"P","Pa"}, {"V","L"}, {"n","mol"}, {"T","K"}, {"R", "(Pa*L)/mol*K"}
    };


    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    /// <summary>
    /// Called by any DroppableTextTarget on drop.
    /// </summary>
    public void OnSlotUpdated(string varName, string droppedText)
    {
        // parse numeric prefix + suffix
        var m = Regex.Match(droppedText.Trim(), @"^([-+]?\d*\.?\d+)(.*)$");
        if (m.Success)
        {
            _nums[varName] = float.Parse(m.Groups[1].Value);
            _suffix[varName] = m.Groups[2].Value;
        }
        else
        {
            _nums.Remove(varName);
            _suffix.Remove(varName);
        }

        TrySolvePV();
        TrySolvePT();
        TrySolveVT();
        TrySolveIG();
    }

    private void TrySolvePV()
    {
        if (_solvedPV) return;
        var keys = new[] { "PV_P1", "PV_V1", "PV_P2", "PV_V2" };
        if (CountFilled(keys) != 3) return;
        string missing = FindMissing(keys);
        float p1 = _nums.GetValueOrDefault("PV_P1"), v1 = _nums.GetValueOrDefault("PV_V1");
        float p2 = _nums.GetValueOrDefault("PV_P2"), v2 = _nums.GetValueOrDefault("PV_V2");
        float res = missing switch
        {
            "PV_P1" => p2 * v2 / v1,
            "PV_V1" => p2 * v2 / p1,
            "PV_P2" => p1 * v1 / v2,
            _ => p1 * v1 / p2
        };
        CreateResultLine(missing.Substring(3), res, resultAreaPV, _unitPV);
        _solvedPV = true;
    }

    private void TrySolvePT()
    {
        if (_solvedPT) return;
        var keys = new[] { "PT_P1", "PT_T1", "PT_P2", "PT_T2" };
        if (CountFilled(keys) != 3) return;
        string missing = FindMissing(keys);
        float p1 = _nums.GetValueOrDefault("PT_P1"), t1 = _nums.GetValueOrDefault("PT_T1");
        float p2 = _nums.GetValueOrDefault("PT_P2"), t2 = _nums.GetValueOrDefault("PT_T2");
        float res = missing switch
        {
            "PT_P1" => p2 * t1 / t2,
            "PT_T1" => p1 * t2 / p2,
            "PT_P2" => p1 * t2 / t1,
            _ => p2 * t1 / p1
        };
        CreateResultLine(missing.Substring(3), res, resultAreaPT, _unitPT);
        _solvedPT = true;
    }

    private void TrySolveVT()
    {
        if (_solvedVT) return;
        var keys = new[] { "VT_V1", "VT_T1", "VT_V2", "VT_T2" };
        if (CountFilled(keys) != 3) return;
        string missing = FindMissing(keys);
        float v1 = _nums.GetValueOrDefault("VT_V1"), t1 = _nums.GetValueOrDefault("VT_T1");
        float v2 = _nums.GetValueOrDefault("VT_V2"), t2 = _nums.GetValueOrDefault("VT_T2");
        float res = missing switch
        {
            "VT_V1" => v2 * t1 / t2,
            "VT_T1" => v1 * t2 / v2,
            "VT_V2" => v1 * t2 / t1,
            "VT_T2" => v2 * t1 / v1
        };
        CreateResultLine(missing.Substring(3), res, resultAreaVT, _unitVT);
        _solvedVT = true;
    }

    private void TrySolveIG()
    {
        if (_solvedIG) return;
        var keys = new[] { "IG_P", "IG_V", "IG_n", "IG_T", "IG_R" };
        if (CountFilled(keys) != 4) return;
        string missing = FindMissing(keys);
        float P = _nums.GetValueOrDefault("IG_P"), V = _nums.GetValueOrDefault("IG_V");
        float n = _nums.GetValueOrDefault("IG_n"), T = _nums.GetValueOrDefault("IG_T");
        float R = _nums.GetValueOrDefault("IG_R");
        float res = missing switch
        {
            "IG_P" => n * R * T / V,
            "IG_V" => n * R * T / P,
            "IG_n" => P * V / (R * T),
            "IG_R" => P * V / (n * T),
            "IG_T" => P * V / (n * R),       
            _ => P * V / (n * R)
        };
        CreateResultLine(missing.Substring(3), res, resultAreaIG, _unitIG);
        _solvedIG = true;
    }

    private int CountFilled(string[] keys)
    {
        int c = 0;
        foreach (var k in keys) if (_nums.ContainsKey(k)) c++;
        return c;
    }

    private string FindMissing(string[] keys)
        => Array.Find(keys, k => !_nums.ContainsKey(k));

    private void CreateResultLine(string varName, float value, RectTransform area, Dictionary<string, string> unitMap)
    {
        // variable label
        var lblGO = Instantiate(variableLabelPrefab, area);
        var lbl = lblGO.GetComponent<Text>();
        if (lbl != null) lbl.text = varName;

        // equals
        Instantiate(equalsSignPrefab, area);

        // result
        var resGO = Instantiate(resultTextPrefab, area);
        var res = resGO.GetComponent<Text>();
        if (res != null)
        {
            string suf = _suffix.GetValueOrDefault(varName,
                         unitMap.GetValueOrDefault(varName, ""));
            res.text = value.ToString("G9") + suf;
        }
    }
}

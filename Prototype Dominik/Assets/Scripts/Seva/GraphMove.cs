using UnityEngine;
using System.Collections.Generic;

public class SpriteOnCurve : MonoBehaviour
{
    [Range(0.02240041f, 0.06462584f)]
    public float rawInput = 0.02240041f; // Real-world input value (e.g., volume)

    public List<Transform> controlPoints;  // Points defining the curve

    private IsothermalMinigame miniGame;

    // Define the input range
    private const float minInput = 0.02240041f;
    private const float maxInput = 0.06462584f;

    void Start()
    {
        miniGame = GameObject.Find("PuzzleManager").GetComponent<IsothermalMinigame>();
    }

    void Update()
    {
        if (miniGame != null)
        {
            rawInput = miniGame.volume;
        }

        float progress = Mathf.InverseLerp(minInput, maxInput, rawInput); // maps rawInput to 0–1

        if (controlPoints.Count >= 2)
        {
            transform.position = CalculateBezierPoint(progress, controlPoints);
        }
    }

    Vector3 CalculateBezierPoint(float t, List<Transform> points)
    {
        while (points.Count > 1)
        {
            List<Transform> newPoints = new List<Transform>();
            for (int i = 0; i < points.Count - 1; i++)
            {
                Vector3 p0 = points[i].position;
                Vector3 p1 = points[i + 1].position;
                Vector3 lerp = Vector3.Lerp(p0, p1, t);
                GameObject temp = new GameObject("temp");
                temp.transform.position = lerp;
                newPoints.Add(temp.transform);
            }
            points = newPoints;
        }
        return points[0].position;
    }
}
using UnityEngine;
using UnityEngine.UI;  // 🔄 Add this line for the Slider component
using System.Collections.Generic;
using System.Diagnostics;

public class SpriteOnCurveIsochoric : MonoBehaviour
{
    [Range(0f, 1f)]
    public float rawInputIsochoric = 0f; // Real-world input value (e.g., volume)

    public List<Transform> IsochoricControlPoints;  // Points defining the curve

    private Slider slider;

    // Define the input range
    private const float minInput = 0f;
    private const float maxInput = 1f;

    void Start()
    {
        // ✅ Find the Slider (make sure it's active in the scene)
        slider = GameObject.Find("Slider").GetComponent<Slider>();

        // 🔄 Optional: Add a warning if the slider is not found
        if (slider == null)
        {
        }
    }

    void Update()
    {
        // 🛠️ Make sure slider is found before using it
        if (slider != null)
        {
            rawInputIsochoric = slider.value;
        }

        float progress = Mathf.InverseLerp(minInput, maxInput, rawInputIsochoric); // maps rawInputIsochoric to 0-1

        if (IsochoricControlPoints.Count >= 2)
        {
            transform.position = CalculateBezierPoint(progress, IsochoricControlPoints);
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
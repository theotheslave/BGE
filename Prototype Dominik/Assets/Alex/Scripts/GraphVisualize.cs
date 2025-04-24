using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class GraphVisualize : MonoBehaviour
{
    public float graphWidth = 4f;
    public float graphHeight = 2f;
    public int maxPoints = 100;

    public float tempMin = 273f;
    public float tempMax = 800f;
    public float volumeMax = 0.04f;

    private LineRenderer lineRenderer;
    private List<Vector2> dataPoints = new();

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    public void AddPoint(float temp, float volume)
    {
        if (dataPoints.Count >= maxPoints)
            dataPoints.RemoveAt(0); 

        dataPoints.Add(new Vector2(temp, volume));
        UpdateLine();
    }

    void UpdateLine()
    {
        lineRenderer.positionCount = dataPoints.Count;
        for (int i = 0; i < dataPoints.Count; i++)
        {
            Vector2 p = dataPoints[i];

            float normX = Mathf.InverseLerp(tempMin, tempMax, p.x);
            float normY = Mathf.InverseLerp(0f, volumeMax, p.y);

            Vector3 pos = new Vector3(normX * graphWidth, normY * graphHeight, 0);
            lineRenderer.SetPosition(i, pos);
        }
    }

    public void Clear()
    {
        dataPoints.Clear();
        lineRenderer.positionCount = 0;
    }
}

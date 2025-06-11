using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ClickSpeedGraph : MonoBehaviour
{
    private StatsBase statsBase;
    public int maxPoints = 100;
    public float graphHeight;
    public float graphWidth;
    public float maxSpeedValue = 4f;
    
    private LineRenderer lineRenderer;
    private Queue<float> speedValues = new Queue<float>();

    private void Start()
    {
        statsBase = GameManager.instance.GetStatsBase;
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = maxPoints;
        lineRenderer.widthMultiplier = 1f;
        
        // graph initialization with zeros
        for (int i = 0; i < maxPoints; i++)
        {
            speedValues.Enqueue(0);
        }
    }

    private void Update()
    {
        // add new speed value to the graph
        float speed = Mathf.Clamp(statsBase.clickSpeed, 0f, maxSpeedValue);
        speedValues.Enqueue(speed);
        
        // remove the oldest value from the graph
        if (speedValues.Count > maxPoints)
            speedValues.Dequeue();
        
        // transform values into the points
        Vector3[] positions = new Vector3[maxPoints];
        int i = 0;
        foreach (float value in speedValues)
        {
            float x = (float)i / (maxPoints - 1) * graphWidth;
            float y = value / maxSpeedValue * graphHeight;
            positions[i] = new Vector3(x, y, 0f);
            i++;
        }
        
        lineRenderer.positionCount = positions.Length;
        lineRenderer.SetPositions(positions);
    }
}

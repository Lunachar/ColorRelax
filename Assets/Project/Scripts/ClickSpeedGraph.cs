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
    public float maxSpeedValue;
    
    private LineRenderer lineRenderer;
    private Queue<float> speedValues = new Queue<float>();
    
    private float timeOffset = 0f;

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
        timeOffset += Time.deltaTime * 4f; // скорость "движения" волны по графику

        float amplitude = Mathf.Clamp(statsBase.clickSpeed * 3, 0f, maxSpeedValue); // амплитуда волны
        float frequency = 2f; // частота волны

        Vector3[] positions = new Vector3[maxPoints];
        for (int i = 0; i < maxPoints; i++)
        {
            float x = (float)i / (maxPoints - 1) * graphWidth;
            float phase = (i * frequency / maxPoints) * Mathf.PI * 2f;
            float y = Mathf.Sin(phase + timeOffset) * amplitude;
            y = Mathf.Clamp(y, -graphHeight, graphHeight); // ограничим график по высоте
            positions[i] = new Vector3(x, y + graphHeight / 2f, 0f); // сдвиг по y чтобы не уходило в минус
        }

        lineRenderer.positionCount = positions.Length;
        lineRenderer.SetPositions(positions);
    }
}

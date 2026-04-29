using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ClickSpeedGraph : MonoBehaviour
{
    [SerializeField] private int maxPoints = 120;
    [SerializeField] private float graphHeightViewport = 0.18f;
    [SerializeField] private float maxSpeedValue = 10f;
    [SerializeField] private float sampleInterval = 0.025f;
    [SerializeField] private float calmNoise = 0.035f;
    [SerializeField] private float spikeSharpness = 18f;
    [SerializeField] private float verticalCenterViewport = 0.16f;
    [SerializeField] private float lineWidth = 0.035f;

    private readonly Queue<float> samples = new Queue<float>();
    private LineRenderer lineRenderer;
    private float sampleTimer;
    private float smoothedIntensity;
    private float phase;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.useWorldSpace = true;
        lineRenderer.positionCount = maxPoints;
        lineRenderer.widthMultiplier = lineWidth;
        lineRenderer.sortingOrder = 100;
        lineRenderer.startColor = new Color(0.45f, 1f, 0.2f, 0.95f);
        lineRenderer.endColor = new Color(0.45f, 1f, 0.2f, 0.95f);

        samples.Clear();
        for (int i = 0; i < maxPoints; i++)
        {
            samples.Enqueue(0f);
        }

        Redraw();
    }

    private void Update()
    {
        float clickSpeed = ScoreManager.instance != null ? ScoreManager.instance.clickSpeed : 0f;
        float targetIntensity = Mathf.InverseLerp(0f, maxSpeedValue, clickSpeed);
        smoothedIntensity = Mathf.Lerp(smoothedIntensity, targetIntensity, Time.deltaTime * 10f);
        phase += Time.deltaTime * Mathf.Lerp(4f, 26f, smoothedIntensity);

        sampleTimer += Time.deltaTime;
        while (sampleTimer >= sampleInterval)
        {
            sampleTimer -= sampleInterval;
            PushSample(CreateHeartbeatSample());
        }

        Redraw();
    }

    private float CreateHeartbeatSample()
    {
        float heartbeat = Mathf.Pow(Mathf.Max(0f, Mathf.Sin(phase)), spikeSharpness);
        float recoil = -0.35f * Mathf.Pow(Mathf.Max(0f, Mathf.Sin(phase - 0.45f)), 8f);
        float wave = Mathf.Sin(phase * 0.7f) * calmNoise;
        float jitter = Random.Range(-calmNoise, calmNoise) * smoothedIntensity;

        return Mathf.Clamp((heartbeat + recoil) * smoothedIntensity + wave + jitter, -1f, 1f);
    }

    private void PushSample(float value)
    {
        if (samples.Count >= maxPoints)
        {
            samples.Dequeue();
        }

        samples.Enqueue(value);
    }

    private void Redraw()
    {
        Camera camera = GameManager.instance != null ? GameManager.instance.GmainCamera : Camera.main;
        if (camera == null)
        {
            RedrawLocal();
            return;
        }

        Vector3[] positions = new Vector3[maxPoints];
        float depth = Mathf.Abs(transform.position.z - camera.transform.position.z);
        if (depth < camera.nearClipPlane)
        {
            depth = camera.nearClipPlane + 1f;
        }

        int index = 0;

        foreach (float sample in samples)
        {
            float x = (float)index / (maxPoints - 1);
            float y = Mathf.Clamp01(verticalCenterViewport + sample * graphHeightViewport * 0.5f);
            positions[index] = camera.ViewportToWorldPoint(new Vector3(x, y, depth));
            index++;
        }

        lineRenderer.positionCount = positions.Length;
        lineRenderer.SetPositions(positions);
    }

    private void RedrawLocal()
    {
        Vector3[] positions = new Vector3[maxPoints];
        int index = 0;

        foreach (float sample in samples)
        {
            float x = Mathf.Lerp(-5f, 5f, (float)index / (maxPoints - 1));
            positions[index] = new Vector3(x, sample, 0f);
            index++;
        }

        lineRenderer.positionCount = positions.Length;
        lineRenderer.SetPositions(positions);
    }
}

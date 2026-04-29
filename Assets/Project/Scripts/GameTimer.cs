using TMPro;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timeField;
    [SerializeField] private Transform startPosition;
    [SerializeField] private Transform endPosition;
    [SerializeField] private float gameDurationSeconds = 60f;
    [SerializeField] private float warningTimeSeconds = 10f;

    private float startTime;
    private bool isGameOver = false;
    private Color defaultTimeColor;

    private void Start()
    {
        Time.timeScale = 1f;
        startTime = Time.time;

        if (timeField == null)
        {
            Debug.LogError("[GameTimer] Time field is not assigned.");
            enabled = false;
            return;
        }

        defaultTimeColor = timeField.color;
        ConfigureTimeText();

        if (startPosition != null)
        {
            timeField.transform.position = startPosition.position;
        }
    }

    private void Update()
    {
        if (isGameOver)
        {
            return;
        }

        float elapsedTime = Time.time - startTime;
        float remainingTime = Mathf.Max(0, gameDurationSeconds - elapsedTime);

        timeField.text = Mathf.CeilToInt(remainingTime).ToString("00");

        float t = Mathf.Clamp01(elapsedTime / gameDurationSeconds);
        if (startPosition != null && endPosition != null)
        {
            timeField.transform.position = Vector3.Lerp(
                startPosition.position,
                endPosition.position,
                t
            );
        }

        if (remainingTime <= warningTimeSeconds)
        {
            float ping = Mathf.PingPong(Time.time * 5f, 1f);
            timeField.color = Color.Lerp(defaultTimeColor, Color.red, ping);
        }
        else
        {
            timeField.color = defaultTimeColor;
        }

        if (remainingTime <= 0f)
        {
            isGameOver = true;
            OnTimerEnd();
        }
    }

    private void OnTimerEnd()
    {
        Time.timeScale = 0f;
        Debug.Log("[GameTimer] Game over. Time is up.");

        int finalScore = GameManager.instance.GetStatsBase.totalScore;
        if (LeaderboardManager.instance.IsHighScore(finalScore))
        {
            GameManager.instance.GetUiManager.ShowNameInputUI();
        }
        else
        {
            GameManager.instance.GetUiManager.ShowLeaderboardUI();
        }
    }

    private void ConfigureTimeText()
    {
        timeField.fontSize = Mathf.Max(timeField.fontSize, 72f);
        timeField.fontStyle = FontStyles.Bold;
        timeField.alignment = TextAlignmentOptions.Center;
        timeField.enableWordWrapping = false;
        timeField.raycastTarget = false;

        RectTransform rectTransform = timeField.rectTransform;
        rectTransform.sizeDelta = new Vector2(Mathf.Max(rectTransform.sizeDelta.x, 180f), Mathf.Max(rectTransform.sizeDelta.y, 90f));
    }
}

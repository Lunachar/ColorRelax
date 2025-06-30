using TMPro;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timeField;
    [SerializeField] private Transform startPosition;
    [SerializeField] private Transform endPosition;
    [SerializeField] private float gameTime = 8300f; // in milliseconds

    private float startTime;
    private bool isGameOver = false;

    private void Start()
    {
        startTime = Time.time;
        if (timeField != null && startPosition != null)
            timeField.transform.position = startPosition.position;
    }

    private void Update()
    {
        if (isGameOver) return;

        float elapsedTime = (Time.time - startTime) * 1000f; // convert to ms
        float remainingTime = Mathf.Max(0, gameTime - elapsedTime);

        // Обновление текста
        timeField.text = $"{remainingTime:0000}";

        // Перемещение текста
        float t = Mathf.Clamp01(elapsedTime / gameTime);
        if (timeField != null && startPosition != null && endPosition != null)
        {
            timeField.transform.position = Vector3.Lerp(
                startPosition.position,
                endPosition.position,
                t
            );
        }

        // Мигание красным, если меньше 1 секунды
        if (remainingTime < 10000f)
        {
            float ping = Mathf.PingPong(Time.time * 5f, 1f);
            timeField.color = Color.Lerp(Color.white, Color.red, ping);
        }

        // Завершение игры
        if (remainingTime <= 0f)
        {
            isGameOver = true;
            OnTimerEnd();
        }
    }

    private void OnTimerEnd()
    {
        Time.timeScale = 0f;
        Debug.Log("⏰ Game Over. Time's up!");

        // Проверка попадания в таблицу
        int finalScore = GameManager.instance.GetStatsBase.totalScore;
        if (LeaderboardManager.instance.IsHighScore(finalScore))
        {
            // Показать UI ввода имени
            GameManager.instance.GetUiManager.ShowNameInputUI();
        }
        else
        {
            GameManager.instance.GetUiManager.ShowLeaderboardUI();
        }
    }
}
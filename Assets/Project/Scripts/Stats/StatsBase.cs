using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "StatsBase", menuName = "Stats/StatsBase")]
public class StatsBase : ScriptableObject
{
    public int totalScore;
    public int buttonClickCount;
    public float clickSpeed;
    public float clickSpeedDecayRate = 1f;

    public IntGameEvent onClickUpdated; // on click event
    public UnityEvent<int> onScoreUpdated = new UnityEvent<int>();
    public UnityEvent<int> onClickCountUpdated = new UnityEvent<int>();

    public void SaveToJson()
    {
        string json = JsonUtility.ToJson(this);
        System.IO.File.WriteAllText(GetJsonPath(), json);
    }

    public void LoadFromJson()
    {
        string path = GetJsonPath();
        if (System.IO.File.Exists(path))
        {
            JsonUtility.FromJsonOverwrite(System.IO.File.ReadAllText(path), this);
        }
    }

    private string GetJsonPath()
    {
        return System.IO.Path.Combine(Application.persistentDataPath, "stats_base.json");
    }

    public void ResetStats()
    {
        buttonClickCount = 0;
        totalScore = 0;
        clickSpeed = 0;
        SaveToJson();
    }

    public void RegisterClick(float speedMultiplier)
    {
        buttonClickCount++;

        // calculate score for one click
        int baseScore = 1;
        float exponent = buttonClickCount / 5f;
        float expMultiplier = Mathf.Pow(2f, exponent);
        float rawScore = baseScore * expMultiplier * speedMultiplier;
        int scoreToAdd = Mathf.RoundToInt(rawScore);

        totalScore += scoreToAdd;

        // events
        onClickUpdated.Invoke(buttonClickCount);
        onScoreUpdated.Invoke(totalScore);

        // UI
        GameManager.instance.GetBonusHistory.AddBonusEntry(scoreToAdd);
        GameManager.instance.GetUiManager.PlayScoreAnimation();
        GameManager.instance.GetUiManager.PlayClickAnimation();

        SaveToJson();
    }

    public void TotalScore(int value)
    {
        int currentAdd = value;
        totalScore += currentAdd;
        GameManager.instance.GetBonusHistory.AddBonusEntry(currentAdd);
        GameManager.instance.GetUiManager.PlayScoreAnimation();

        SaveToJson();
    }


    public void SetClickSpeed(float buttonClickSpeed)
    {
        if (buttonClickSpeed <= 0f)
        {
            clickSpeed = 0;
            return;
        }
        float rawSpeed = 1f / buttonClickSpeed;
        clickSpeed = Mathf.Clamp(rawSpeed, 0f, 10f);
    }

    public void DecayClickSpeed()
    {
        clickSpeed = Mathf.Lerp(clickSpeed, 0f, clickSpeedDecayRate * Time.deltaTime);
    }

    public void AddButtonClickCount(int buttonClickCount)
    {
        this.buttonClickCount += buttonClickCount;
    }

    public int GetButtonClickCount()
    {
        return buttonClickCount;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StatsBase", menuName = "Stats/StatsBase")]
public class StatsBase : ScriptableObject
{
    public int buttonClickCount;
    public int totalScore;
    public IntGameEvent onClickUpdated; // on click event

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
        SaveToJson();
    }

    public void CalculateScore(int speedMultiplier)
    {
        int baseScore = 1;
        int multiplier = Mathf.FloorToInt(buttonClickCount / 5);
        
        int scoreToAdd = Mathf.RoundToInt(baseScore *(multiplier + 1) * speedMultiplier);
        totalScore += scoreToAdd;
        GameManager.instance.GetUiManager.PlayScoreAnimation();
    }

    public void CalculateButtonClickCount()
    {
        buttonClickCount++;
        GameManager.instance.GetUiManager.PlayScoreAnimation();
        onClickUpdated.Invoke(buttonClickCount);
        SaveToJson();
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StatsBase", menuName = "Stats/StatsBase")]
public class StatsBase : ScriptableObject
{
    public int totalScore;
    public int buttonClickCount;
    public float clickSpeed;
    public float clickSpeedDecayRate = 1f;
    
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
        clickSpeed = 0;
        SaveToJson();
    }

    public void CalculateScore(float speedMultiplier)
    {
        int baseScore = 1;
        
        // exponent is doubling every 5 clicks
        float exponent = buttonClickCount / 5;
        float expMultiplier = Mathf.Pow(2f, exponent);
        
        // final additive
        float rawScore = baseScore * expMultiplier * speedMultiplier;
        int scoreToAdd = Mathf.RoundToInt(rawScore);
        
        totalScore += scoreToAdd;
        GameManager.instance.GetUiManager.PlayScoreAnimation();
        
        SaveToJson();
        
    }

    public void CalculateButtonClickCount()
    {
        buttonClickCount++;
        GameManager.instance.GetUiManager.PlayClickAnimation();
        onClickUpdated.Invoke(buttonClickCount);
        SaveToJson();
    }

    public void SetClickSpeed(float buttonClickSpeed)
    {
        float rawSpeed = 1f / buttonClickSpeed;
        clickSpeed = Mathf.Clamp(rawSpeed, 0f, 10f);
    }

    public void DecayClickSpeed()
    {
        clickSpeed = Mathf.MoveTowards(clickSpeed, 0f, clickSpeedDecayRate * Time.deltaTime);
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
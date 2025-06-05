using UnityEngine;
using UnityEngine.Serialization;

public class AchievementTracker : MonoBehaviour
{
    [SerializeField] private StatsBase statsBase;
    [SerializeField] private IntGameEvent onAchievementUnlocked;

    // progression settings
    [Header("Progression Settings")]
    [SerializeField] private int baseThreshold = 10;
    [SerializeField] private int baseReward = 10;
    [SerializeField] private float multiplier = 2f;
    
    private void OnEnable()
    {
        if(statsBase != null)
        {
            statsBase.onClickUpdated.AddListener(CheckAllAchievements);
        }
       
    }

    private void OnDisable()
    {
        if (statsBase != null)
        {
            statsBase.onClickUpdated.RemoveListener(CheckAllAchievements);
        }
    }

    public void CheckAllAchievements(int totalClicks)
    {
        // calculate current achievement level
        int level = Mathf.FloorToInt(Mathf.Log(totalClicks / baseThreshold, multiplier)) + 1;
        
        if(totalClicks >= GetThresholdForLevel(level))
        {
            int reward = CalculateReward(level);
            statsBase.totalScore += reward;
            
            Debug.Log($"Got Achievement for level {level}! Reward: {reward}");
            onAchievementUnlocked.Invoke(level);
        }
    }

    private int CalculateReward(int level)
    {
        return Mathf.RoundToInt(baseReward * Mathf.Pow(2, level));
    }

    private int GetThresholdForLevel(int level)
    {
        return baseThreshold * Mathf.RoundToInt(Mathf.Pow(multiplier, level) - 1);
    }

    private void Start()
    {
        statsBase.LoadFromJson();
        statsBase.onClickUpdated.AddListener(CheckAllAchievements);
    }

    private void OnApplicationQuit() => statsBase.SaveToJson();
}

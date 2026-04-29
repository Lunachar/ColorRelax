using System;
using UnityEngine;
using UnityEngine.Events;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    [SerializeField] private StatsBase statsBase;
    [SerializeField] private BonusHistory bonusHistory;
    [SerializeField] private int maxScorePerClick = 50000000;
    [SerializeField] private int maxScorePerMatch = 250000000;

    public UnityEvent<int> onScoreUpdated = new UnityEvent<int>();
    public UnityEvent<int> onClickUpdated = new UnityEvent<int>();

    public float clickSpeed { get; private set; }
    public int LastScoreAdded { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RegisterClick(float speedMultiplier)
    {
        statsBase.buttonClickCount++;
        onClickUpdated.Invoke(statsBase.buttonClickCount);

        clickSpeed = speedMultiplier;

        double clickGrowth = Math.Pow(1.115d, statsBase.buttonClickCount);
        double speedGrowth = 1d + speedMultiplier * 0.65d + speedMultiplier * speedMultiplier * 0.09d;
        double scoreGrowth = 1d + Math.Log10(statsBase.totalScore + 10d) * 0.75d
            + Math.Pow(Math.Max(statsBase.totalScore, 0) / 1000d, 0.45d);
        double rawScore = clickGrowth * speedGrowth * scoreGrowth;
        double clampedScore = Math.Min(Math.Max(Math.Round(rawScore), 1d), maxScorePerClick);
        int scoreToAdd = (int)clampedScore;

        AddScore(scoreToAdd, string.Empty);
    }

    public void AddBonus(int amount, string reason)
    {
        AddScore(amount, reason);
    }

    public void AddMatchBonus()
    {
        double scoreMomentum = Math.Max(statsBase.totalScore, 10) * 0.12d;
        double speedMomentum = 1d + clickSpeed * 0.45d + clickSpeed * clickSpeed * 0.12d;
        double lastClickMomentum = Math.Max(LastScoreAdded, 1) * 12d;
        double rawBonus = Math.Max(lastClickMomentum, scoreMomentum * speedMomentum);
        double clampedBonus = Math.Min(Math.Max(Math.Round(rawBonus), 50d), maxScorePerMatch);

        AddScore((int)clampedBonus, "match");
    }

    private void AddScore(int amount, string reason)
    {
        if (amount <= 0)
        {
            return;
        }

        statsBase.totalScore = SaturatingAdd(statsBase.totalScore, amount);
        LastScoreAdded = amount;
        statsBase.SaveToJson();

        onScoreUpdated.Invoke(statsBase.totalScore);
        bonusHistory.AddBonusEntry(amount, reason);

        Debug.Log($"[ScoreManager] +{amount} for {reason}");
    }

    private int SaturatingAdd(int currentValue, int amount)
    {
        long result = (long)currentValue + amount;
        return result > int.MaxValue ? int.MaxValue : (int)result;
    }
}

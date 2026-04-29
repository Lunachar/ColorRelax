using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementTracker : MonoBehaviour
{
    private StatsBase statsBase;
    private HashSet<int> unlockedScoreMilestones = new HashSet<int>();
    private HashSet<int> unlockedClickMilestones = new HashSet<int>();

    private int[] clickMilestones =
        { 7, 11, 23, 55, 77, 100, 111, 133, 155, 177, 200, 222, 255, 277, 300, 333, 400, 444 };

    private int[] scoreMilestones = { 100, 500, 1000, 2500, 5000, 10000, 25000, 50000, 100000, 250000, 500000, 1000000 };
    private int[] speedMilestones = { 2, 5, 8, 10 };

    private int matchCombo = 0;

    private float clickTimer;
    private float timeBetweenClicks = 0.5f;

    private void Start()
    {
        statsBase = GameManager.instance.GetStatsBase;

        var scoreManager = ScoreManager.instance;

        if (scoreManager != null)
        {
            scoreManager.onScoreUpdated.AddListener(CheckScoreMilestones);
            scoreManager.onClickUpdated.AddListener(OnClickUpdated);
        }
    }


    private void Update()
    {
        clickTimer += Time.deltaTime;
    }

    private void OnClickUpdated(int totalClicks)
    {
        CheckClickMilestones(totalClicks);
        CheckScoreMilestones(statsBase.totalScore);
        CheckClickSpeed();
        CheckCombo();
        clickTimer = 0;
    }

    private void CheckClickMilestones(int clicks)
    {
        foreach (int milestone in clickMilestones)
        {
            if (clicks >= milestone && !unlockedClickMilestones.Contains(milestone))
            {
                GrantBonus(Mathf.Max(25, milestone * 3), $"+{milestone} clicks", "click streak");
                unlockedClickMilestones.Add(milestone);
            }
        }
    }

    private void CheckScoreMilestones(int score)
    {
        foreach (int milestone in scoreMilestones)
        {
            if (score >= milestone && !unlockedScoreMilestones.Contains(milestone))
            {
                GrantBonus(Mathf.Max(100, milestone / 4), $"+{milestone} round number", "round numb");
                unlockedScoreMilestones.Add(milestone);
            }
        }
    }

    private void CheckClickSpeed()
    {
        if (clickTimer < timeBetweenClicks)
        {
            statsBase.clickSpeed += 1f;

            foreach (int milestone in speedMilestones)
            {
                if ((int)statsBase.clickSpeed == milestone)
                {
                    GrantBonus(milestone * 75, $"x{milestone} speed", $"x{milestone} speed");
                    break;
                }
            }
        }
        else
        {
            statsBase.clickSpeed = 0;
        }
    }

    private void CheckCombo()
    {
        if (GameManager.instance.GetButtonBehaviour.GetCurrentButtonColor() ==
            GameManager.instance.GetBackgroundBehaviour.GetCurrentBackgroundColor())
        {
            matchCombo++;
            if (matchCombo > 3)
            {
                GrantBonus(matchCombo * 150, $"Combo x{matchCombo}!", $"combo x{matchCombo}");
                matchCombo = 0;
            }
        }
        else
        {
            matchCombo = 0;
        }
    }

    private void GrantBonus(int value, string message, string historyLabel)
    {
        
        StartCoroutine(Wait(value, historyLabel));
        GameManager.instance.GetUiManager.ShowPopup(message);
    }

    private IEnumerator Wait(int value, string historyLabel)
    {
            yield return new WaitForSeconds(1);
            ScoreManager.instance.AddBonus(value, historyLabel);
    }
}

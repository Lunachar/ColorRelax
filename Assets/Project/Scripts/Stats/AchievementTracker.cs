using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class AchievementTracker : MonoBehaviour
{
    private StatsBase statsBase;
    private HashSet<int> unlockedScoreMilestones = new HashSet<int>();
    private HashSet<int> unlockedClickMilestones = new HashSet<int>();

    private int[] clickMilestones =
        { 7, 11, 23, 55, 77, 100, 111, 133, 155, 177, 200, 222, 255, 277, 300, 333, 400, 444 };

    private int[] scoreMilestones = { 10, 50, 100, 200, 300, 400, 500, 600, 700, 800, 900, 1000 };
    private int[] speedMilestones = { 2, 5, 8, 10 };

    private int lastClickMatch = 0;
    private int lastMilestoneChecked = 0;
    private int matchCombo = 0;

    private float clickTimer;
    private float timeBetweenClicks = 0.5f;

    private void Start()
    {
        statsBase = GameManager.instance.GetStatsBase;

        if (statsBase != null)
        {
            statsBase.onScoreUpdated.AddListener(CheckScoreMilestones);
            statsBase.onClickCountUpdated.AddListener(CheckScoreMilestones);
            statsBase.onClickUpdated.AddListener(OnClickUpdated);
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
                GrantBonus(10, $"Matched {milestone} clicks!");
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
                GrantBonus(20, $"Matched {milestone} points!");
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
                    GrantBonus(15, $"Clicking at {milestone} clicks/second!");
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
                GrantBonus(50, $"Combo x{matchCombo}!");
                matchCombo = 0;
            }
        }
        else
        {
            matchCombo = 0;
        }
    }

    private void GrantBonus(int value, string message)
    {
        
        StartCoroutine(Wait(value));
        GameManager.instance.GetUiManager.ShowPopup(message);
    }

    private IEnumerator Wait(int value)
    {
            yield return new WaitForSeconds(1);
            statsBase.TotalScore(value);
    }
}
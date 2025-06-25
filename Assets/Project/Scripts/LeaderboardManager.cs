using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[Serializable]
public class LeaderboardEntry
{
    public string name;
    public int score;
}

[Serializable]
public class LeaderboardData
{
    public List<LeaderboardEntry> entries = new List<LeaderboardEntry>();
}

public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager instance;

    private string fileName = "leaderboard.json";
    private int maxEntries = 7;
    private LeaderboardData leaderboardData = new LeaderboardData();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            LoadLeaderboard();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public List<LeaderboardEntry> GetEntries()
    {
        return leaderboardData.entries;
    }

    public bool IsHighScore(int score)
    {
        if (leaderboardData.entries.Count < maxEntries)
            return true;

        return score > leaderboardData.entries[leaderboardData.entries.Count - 1].score;
    }

    public void AddEntry(string playerName, int score)
    {
        var entry = new LeaderboardEntry { name = playerName, score = score };
        leaderboardData.entries.Add(entry);

        // сортировка по убыванию
        leaderboardData.entries = leaderboardData.entries
            .OrderByDescending(e => e.score)
            .Take(maxEntries)
            .ToList();

        SaveLeaderboard();
    }

    private void SaveLeaderboard()
    {
        string json = JsonUtility.ToJson(leaderboardData, true);
        File.WriteAllText(GetFilePath(), json);
    }

    private void LoadLeaderboard()
    {
        string path = GetFilePath();

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            leaderboardData = JsonUtility.FromJson<LeaderboardData>(json);
        }
        else
        {
            leaderboardData = new LeaderboardData();
        }
    }

    private string GetFilePath()
    {
        return Path.Combine(Application.persistentDataPath, fileName);
    }
}
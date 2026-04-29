using UnityEngine;

[CreateAssetMenu(fileName = "StatsBase", menuName = "Stats/StatsBase")]
public class StatsBase : ScriptableObject
{
    public int totalScore;
    public int buttonClickCount;
    public float clickSpeed;
    public float clickSpeedDecayRate = 1f;

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

    public void ResetStats()
    {
        buttonClickCount = 0;
        totalScore = 0;
        clickSpeed = 0;
        SaveToJson();
    }

    private string GetJsonPath()
    {
        return System.IO.Path.Combine(Application.persistentDataPath, "stats_base.json");
    }
}

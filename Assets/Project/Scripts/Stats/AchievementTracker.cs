using UnityEngine;

public class AchievementTracker : MonoBehaviour
{
    public ClickStats clickStats;

    private void OnEnable()
    {
       
    }

    private void OnDisable()
    {
        
    }

    private void CheckStarClicks(int totalClicks)
    {
        if (totalClicks == 10)
        {
            Debug.Log("Got Star Clicks Achievement!");
        }
    }

    private void Start() => clickStats.LoadFromJson();
    private void OnApplicationQuit() => clickStats.SaveToJson();
}

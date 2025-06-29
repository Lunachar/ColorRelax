public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    public int totalScore { get; private set; }
    public int clickCount { get; private set; }

    [SerializeField] private BonusHistory bonusHistory;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void RegisterClick(float speedMultiplier)
    {
        clickCount++;

        int baseScore = 1;
        float exponent = clickCount / 5f;
        float expMultiplier = Mathf.Pow(2f, exponent);
        float rawScore = baseScore * expMultiplier * speedMultiplier;
        int scoreToAdd = Mathf.RoundToInt(rawScore);

        AddScore(scoreToAdd, "Click");
    }

    public void AddBonus(int amount, string reason)
    {
        AddScore(amount, reason);
    }

    private void AddScore(int amount, string reason)
    {
        totalScore += amount;

        // Запускаем вывод в UI через очередь
        bonusHistory.EnqueueBonus(amount);

        // События для других систем
        GameManager.instance.GetUiManager.UpdateScoreText(totalScore, clickCount);
        Debug.Log($"[ScoreManager] +{amount} for {reason}");
    }
}
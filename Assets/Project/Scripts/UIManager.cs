using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI clickConterText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI currentClickSpeed;
    //[SerializeField] private float clickSpeedDecayRate = 0.5f;
    private StatsBase statsBase;

    private Color defaultColor;


    private void Start()
    {
        statsBase = GameManager.instance.GetStatsBase;
        defaultColor = scoreText.color;
    }

    private void Update()
    {
        clickConterText.text = statsBase.buttonClickCount.ToString();
        scoreText.text = statsBase.totalScore.ToString();

        float colorLerp = Mathf.Clamp01(statsBase.totalScore / 1000f);
        scoreText.color = Color.Lerp(defaultColor, Color.yellow, colorLerp);

        statsBase.DecayClickSpeed();
        UpdateClickSpeedText();
    }

    public void PlayScoreAnimation()
    {
        scoreText.transform.DOPunchScale(Vector3.one * 0.2f, 0.3f);
        scoreText.DOColor(Color.green, 0.2f).OnComplete(() =>
            scoreText.DOColor(scoreText.color, 0.2f));
    }

    public void PlayClickAnimation()
    {
        clickConterText.transform.DOPunchScale(Vector3.one * 0.2f, 0.3f);
        clickConterText.DOColor(Color.green, 0.2f).OnComplete(() =>
            clickConterText.DOColor(scoreText.color, 0.2f));
    }

    public void UpdateClickSpeedText()
    {
        float clickSpeed = GameManager.instance.GetStatsBase.clickSpeed;
        currentClickSpeed.text = $"{clickSpeed:0.00}";
        
        float intensity = Mathf.InverseLerp(0, 10, clickSpeed);
        
        currentClickSpeed.color = Color.Lerp(Color.gray, Color.green, intensity);
        
        currentClickSpeed.transform.DOScale(Vector3.one * (1f + 0.3f * intensity), 0.2f).SetEase(Ease.OutQuad);
    }
}
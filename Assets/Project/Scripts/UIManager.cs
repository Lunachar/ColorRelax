using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI clickConterText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI currentClickSpeed;
    
    [SerializeField] private CanvasGroup popupPanel;
    [SerializeField] private TextMeshProUGUI popupText;
    [SerializeField] private float popupDuration = 1.5f;

[SerializeField] private NameInputUI nameInputUI;
    [SerializeField] private ExitMenu exitMenu;
    
    private Tween currentPopupTween;
    
    private Tween clickSpeedScaleTween;
    
    private StatsBase statsBase;

    private Color defaultColor;
    


    private void Start()
    {
        statsBase = GameManager.instance.GetStatsBase;
        defaultColor = scoreText.color;
    }

    private void Update()
    {
        if (scoreText.transform.localScale != Vector3.one)
        {
            Debug.LogWarning("⚠️ scoreText scale changed: " + scoreText.transform.localScale);
        }

        clickConterText.text = statsBase.buttonClickCount.ToString();
        scoreText.text = statsBase.totalScore.ToString();

        float colorLerp = Mathf.Clamp01(statsBase.totalScore / 1000f);
        scoreText.color = Color.Lerp(defaultColor, Color.yellow, colorLerp);

        statsBase.DecayClickSpeed();
        UpdateClickSpeedText();
    }

    public void PlayScoreAnimation()
    {
        scoreText.transform.DOKill(true);
        scoreText.transform.DOPunchScale(Vector3.one * 0.2f, 0.3f);
        scoreText.DOColor(Color.green, 0.2f).OnComplete(() =>
            scoreText.DOColor(scoreText.color, 0.2f));
    }

    public void PlayClickAnimation()
    {
        clickConterText.transform.DOKill(true);
        clickConterText.transform.DOPunchScale(Vector3.one * 0.2f, 0.3f);
        clickConterText.DOColor(Color.green, 0.2f).OnComplete(() =>
            clickConterText.DOColor(scoreText.color, 0.2f));
    }

    public void UpdateClickSpeedText()
    {
        float clickSpeed = statsBase.clickSpeed;

        if (currentClickSpeed == null) 
        {
            Debug.LogError("currentClickSpeed == NULL!");
            return;
        }
        currentClickSpeed.text = $"{clickSpeed:0.00}";
        
        float intensity = Mathf.InverseLerp(0, 10, clickSpeed);
        
        currentClickSpeed.color = Color.Lerp(Color.gray, Color.green, intensity);
        
        clickSpeedScaleTween?.Kill(); // stop previous anim
        currentClickSpeed.transform.localScale = Vector3.one; // reset scale
        
        currentClickSpeed.transform.DOScale(Vector3.one * (1f + 0.3f * intensity), 0.2f).SetEase(Ease.OutQuad);
    }

    public void ShowPopup(string message)
    {
        currentPopupTween?.Kill();
        
        popupText.text = message;
        popupPanel.alpha = 0;
        popupPanel.gameObject.SetActive(true);
        
        currentPopupTween = DOTween.Sequence()
            .Append(popupPanel.DOFade(1, 0.3f))
            .AppendInterval(popupDuration)
            .Append(popupPanel.DOFade(0, 0.3f))
            .OnComplete(() => popupPanel.gameObject.SetActive(false));
    }

  public void ShowNameInputUI()
    {
        nameInputUI.Show();
    }

    public void ShowLeaderboardUI()
    {
        exitMenu.ShowLeaderboard();
    }
}
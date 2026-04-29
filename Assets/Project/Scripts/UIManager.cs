using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI clickConterText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI currentClickSpeed;

    [SerializeField] private CanvasGroup popupPanel;
    [SerializeField] private TextMeshProUGUI popupText;
    [SerializeField] private float popupDuration = 1.5f;

    [SerializeField] private NameInputUI nameInputUI;
    [SerializeField] private LeaderboardUI leaderboardUI;

    private Tween currentPopupTween;
    private Tween clickSpeedScaleTween;
    private Tween scoreColorTween;
    private Tween clickColorTween;

    private StatsBase statsBase;
    private Color defaultScoreColor;
    private Color currentScoreColor;
    private Color defaultClickColor;
    private float lastDisplayedClickSpeed = -1f;

    private void Start()
    {
        statsBase = GameManager.instance.GetStatsBase;
        defaultScoreColor = scoreText.color;
        defaultClickColor = clickConterText.color;

        ScoreManager.instance.onScoreUpdated.AddListener(UpdateScoreText);
        ScoreManager.instance.onClickUpdated.AddListener(UpdateClickCountText);

        UpdateScoreText(statsBase.totalScore);
        UpdateClickCountText(statsBase.buttonClickCount);
        UpdateClickSpeedText(true);
        StyleButtons();
    }

    private void Update()
    {
        UpdateClickSpeedText(false);
    }

    private void UpdateScoreText(int newScore)
    {
        scoreText.text = newScore.ToString();

        float colorLerp = Mathf.Clamp01(newScore / 1000f);
        currentScoreColor = Color.Lerp(defaultScoreColor, Color.yellow, colorLerp);
        scoreText.color = currentScoreColor;

        PlayScoreAnimation();
    }

    private void UpdateClickCountText(int newClickCount)
    {
        clickConterText.text = newClickCount.ToString();
        PlayClickAnimation();
    }

    public void PlayScoreAnimation()
    {
        scoreText.transform.DOKill(true);
        scoreColorTween?.Kill();
        scoreText.transform.DOPunchScale(Vector3.one * 0.2f, 0.3f);
        scoreColorTween = DOTween.Sequence()
            .Append(scoreText.DOColor(Color.green, 0.2f))
            .Append(scoreText.DOColor(currentScoreColor, 0.2f));
    }

    public void PlayClickAnimation()
    {
        clickConterText.transform.DOKill(true);
        clickColorTween?.Kill();
        clickConterText.transform.DOPunchScale(Vector3.one * 0.2f, 0.3f);
        clickColorTween = DOTween.Sequence()
            .Append(clickConterText.DOColor(Color.green, 0.2f))
            .Append(clickConterText.DOColor(defaultClickColor, 0.2f));
    }

    public void UpdateClickSpeedText(bool force)
    {
        float clickSpeed = ScoreManager.instance.clickSpeed;

        if (currentClickSpeed == null)
        {
            Debug.LogError("currentClickSpeed == NULL!");
            return;
        }

        if (!force && Mathf.Approximately(clickSpeed, lastDisplayedClickSpeed))
        {
            return;
        }

        lastDisplayedClickSpeed = clickSpeed;
        currentClickSpeed.text = $"{clickSpeed:0.00}";

        float intensity = Mathf.InverseLerp(0, 10, clickSpeed);
        currentClickSpeed.color = Color.Lerp(Color.gray, Color.green, intensity);

        clickSpeedScaleTween?.Kill();
        clickSpeedScaleTween = currentClickSpeed.transform
            .DOScale(Vector3.one * (1f + 0.3f * intensity), 0.2f)
            .SetEase(Ease.OutQuad);
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
        if (leaderboardUI == null)
        {
            leaderboardUI = FindObjectOfType<LeaderboardUI>();
        }

        if (leaderboardUI == null)
        {
            leaderboardUI = new GameObject("LeaderboardUI").AddComponent<LeaderboardUI>();
        }

        leaderboardUI.Show(statsBase.totalScore);
    }

    private void StyleButtons()
    {
        Button[] buttons = FindObjectsOfType<Button>(true);
        Button gameplayButton = GameManager.instance.GetButtonBehaviour != null
            ? GameManager.instance.GetButtonBehaviour.colorButton
            : null;

        foreach (Button button in buttons)
        {
            if (button == gameplayButton)
            {
                continue;
            }

            if (button.GetComponent<PrettyButton>() == null)
            {
                button.gameObject.AddComponent<PrettyButton>();
            }
        }
    }
}

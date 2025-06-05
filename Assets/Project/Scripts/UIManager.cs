using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI clickConterText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private ClickStats clickStats;

    private Color defaultColor;


    private void Start()
    {
        defaultColor = scoreText.color;
    }

    private void Update()
    {
        clickConterText.text = clickStats.buttonClickCount.ToString();
        scoreText.text = clickStats.totalScore.ToString();
        
        float colorLerp = Mathf.Clamp01(clickStats.totalScore / 1000f);
        scoreText.color = Color.Lerp(defaultColor, Color.yellow, colorLerp);
    }

    public void PlayScoreAnimation()
    {
        scoreText.transform.DOPunchScale(Vector3.one * 0.2f, 0.3f);
        scoreText.DOColor(Color.green, 0.2f).OnComplete(() =>
            scoreText.DOColor(scoreText.color, 0.2f));
    }
}

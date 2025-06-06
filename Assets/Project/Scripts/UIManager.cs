using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI clickConterText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [FormerlySerializedAs("clickStats")] [SerializeField] private StatsBase statsBase;

    private Color defaultColor;


    private void Start()
    {
        defaultColor = scoreText.color;
    }

    private void Update()
    {
        clickConterText.text = statsBase.buttonClickCount.ToString();
        scoreText.text = statsBase.totalScore.ToString();
        
        float colorLerp = Mathf.Clamp01(statsBase.totalScore / 1000f);
        scoreText.color = Color.Lerp(defaultColor, Color.yellow, colorLerp);
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
}

using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;
using Button = UnityEngine.UI.Button;

public class ButtonBehaviour : MonoBehaviour
{
    private Color currentButtonColor;

    public Button colorButton;
    public List<Color> colors;
    public float speedMultiplier;

    private float lastClickTime;
    private float clickInterval;

    private void Start()
    {
        colors = GameManager.instance.GetColorDatabase.availibleColors;
        colorButton.onClick.AddListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        if (Time.timeScale < 0.1f)
        {
            return;
        }

        clickInterval = Time.time - lastClickTime;
        lastClickTime = Time.time;

        speedMultiplier = Mathf.Clamp(1f / clickInterval, 0.01f, 10f);

        ScoreManager.instance.RegisterClick(speedMultiplier);

        if (GameManager.instance.GetSoundDatabase.buttonClickSound.Count > 0)
        {
            AudioClip clickSound = GameManager.instance.GetSoundDatabase
                .buttonClickSound[Random.Range(0, GameManager.instance.GetSoundDatabase.buttonClickSound.Count)];
            GameManager.instance.GsoundEffects.PlayOneShot(clickSound);
        }

        GameManager.instance.GetBackgroundBehaviour.ChangeBackgroundColor();
        ChangeButtonColorAndPosition();
    }

    public void ChangeButtonColorAndPosition()
    {
        currentButtonColor = colors[Random.Range(0, colors.Count)];
        colorButton.image.color = currentButtonColor;

        RectTransform buttonRect = colorButton.GetComponent<RectTransform>();
        float x = Random.Range(-300f, 300f);
        float y = Random.Range(-400f, 450f);

        buttonRect.DOAnchorPos(new Vector2(x, y), 0.5f).SetEase(Ease.OutBack);
        colorButton.image.DOFade(0, 0);
        colorButton.image.DOFade(1, 0.5f);

        GameManager.instance.CheckMatch();
    }

    public Color GetCurrentButtonColor() => currentButtonColor;
}

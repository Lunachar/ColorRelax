using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;
using Button = UnityEngine.UI.Button;

public class ButtonBehaviour : MonoBehaviour
{
    private Color currentButtonColor;
    public Button colorButton;
    
    public List<Color> colors;
    
    // button pressing speed variables
    private float lastClickTime;
    private float clickInterval;
    public float speedMultiplier;

    private void Start()
    {
        colors = GameManager.instance.GetColorDatabase.availibleColors;
        colorButton.onClick.AddListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        if(Time.timeScale < 0.1f) return;
        
        // interval between button clicks
        clickInterval = Time.time - lastClickTime;
        lastClickTime = Time.time;
        GameManager.instance.GetStatsBase.SetClickSpeed(clickInterval);

        // the higher the clicks, the higher the multiplier (not less than 1)
        speedMultiplier = Mathf.Clamp(1f / clickInterval, 0.01f, 10f);
        
        GameManager.instance.GetStatsBase.RegisterClick(speedMultiplier);
        
        if (GameManager.instance.GetSoundDatabase.buttonClickSound.Count > 0)
        {
            AudioClip clickSound = GameManager.instance.GetSoundDatabase
                .buttonClickSound[Random.Range(0, GameManager.instance.GetSoundDatabase.buttonClickSound.Count)];
            GameManager.instance.GsoundEffects.PlayOneShot(clickSound);
        }
        
        ChangeButtonColorAndPosition();
        GameManager.instance.GetStatsBase.SaveToJson();
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

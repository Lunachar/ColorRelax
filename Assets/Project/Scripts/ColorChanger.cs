using System.Collections.Generic;
using UnityEngine;
using Button = UnityEngine.UI.Button;
using Random = UnityEngine.Random;
using DG.Tweening;

public class ColorChanger : MonoBehaviour
{
    public Camera mainCamera;
    public Button colorButton;
    public ParticleSystem fireworks;
    public AudioSource backgroundMusic;
    public AudioSource soundEffects;

    public List<Color> colors => colorDatabase.availibleColors;
    
    public ColorDatabase colorDatabase;
    public SoundDatabase soundDatabase;

    public ClickStats stats;
    
    private Color currentBackgroundColor;
    private Color currentButtonColor;
    
    // button pressing speed variables
    private float lastClickTime;
    private float clickInterval;
    public float speedMultiplier = 1f;

    private void Start()
    {
        backgroundMusic.Play();
        ChangeColors();
        
        colorButton.onClick.AddListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        if(Time.timeScale < 0.1f) return;
        
        // interval between button clicks
        clickInterval = Time.time - lastClickTime;
        lastClickTime = Time.time;

        // the higher the clicks, the higher the multiplier (not less than 1)
        speedMultiplier = Mathf.Clamp(1f / clickInterval, 1f, 1);
        
        stats.buttonClickCount++;

        stats.CalculateScore((int)speedMultiplier);
        
        if (soundDatabase.buttonClickSound.Count > 0)
        {
            AudioClip clickSound = soundDatabase
                .buttonClickSound[Random.Range(0, soundDatabase.buttonClickSound.Count)];
            soundEffects.PlayOneShot(clickSound);
        }
        
        ChangeButtonColorAndPosition();
        stats.SaveToJson();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ChangeBackgroundColor();
        }
    }

    private void ChangeBackgroundColor()
    {
        currentBackgroundColor = colors[Random.Range(0, colors.Count)];
        mainCamera.backgroundColor = currentBackgroundColor;
        CheckMatch();
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

        CheckMatch();
    }

    private void CheckMatch()
    {
        if (currentBackgroundColor == currentButtonColor)
        {
            fireworks.Play();
            transform.DOPunchScale(Vector3.one * 0.2f, 0.3f);
            Camera.main.DOShakePosition(0.2f, 0.1f);

            if (soundDatabase.matchSound != null)
            {
                soundEffects.PlayOneShot(soundDatabase.matchSound);
            }
        }
    }

    private void ChangeColors()
        {
            ChangeBackgroundColor();
            ChangeButtonColorAndPosition();
        }
}
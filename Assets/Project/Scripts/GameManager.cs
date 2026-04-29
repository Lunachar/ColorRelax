using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    [Header("ALL SYSTEM REFERENCES")]
    [SerializeField] private UIManager uiManager;
    [SerializeField] private StatsBase statsBase;
    [SerializeField] private ButtonBehaviour buttonBehaviour;
    [SerializeField] private BackgroundBehaviour backgroundBehaviour;
    [SerializeField] private BonusHistory bonusHistory;
    
    [SerializeField] private Camera mainCamera;
    [SerializeField] private ParticleSystem fireworks;
    [SerializeField] private AudioSource backgroundMusic;
    [SerializeField] private AudioSource soundEffects;

    
    [SerializeField] private ColorDatabase colorDatabase;
    [SerializeField] private SoundDatabase soundDatabase;

    private readonly List<ParticleSystem> matchEffects = new List<ParticleSystem>();
    private bool wasMatchedLastCheck;
    private bool matchScoringEnabled;

    private void Awake()
    {
        transform.SetParent(null);
        if (instance == null)
        {
            instance = this;
            statsBase.LoadFromJson();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        CacheMatchEffects();
        ConfigureMatchEffects();
        backgroundMusic.Play();
        ChangeColors();
        wasMatchedLastCheck = false;
        matchScoringEnabled = true;
    }

    public void CheckMatch()
    {
        bool isMatch = backgroundBehaviour.GetCurrentBackgroundColor() == buttonBehaviour.GetCurrentButtonColor();
        if (!isMatch)
        {
            wasMatchedLastCheck = false;
            return;
        }

        if (matchScoringEnabled)
        {
            PlayMatchEffects();
        }

        if (matchScoringEnabled && !wasMatchedLastCheck && ScoreManager.instance != null)
        {
            ScoreManager.instance.AddMatchBonus();
        }

        wasMatchedLastCheck = true;

        if (soundDatabase.matchSound != null)
        {
            soundEffects.PlayOneShot(soundDatabase.matchSound);
        }
    }

    private void ChangeColors()
    {
        backgroundBehaviour.ChangeBackgroundColor();
        buttonBehaviour.ChangeButtonColorAndPosition();
    }

    private void CacheMatchEffects()
    {
        matchEffects.Clear();
        AddMatchEffect(fireworks);

        AddMatchEffect(GameObject.Find("ColorMatchFX")?.GetComponent<ParticleSystem>());
        AddMatchEffect(GameObject.Find("ConffettiFX")?.GetComponent<ParticleSystem>());
    }

    private void AddMatchEffect(ParticleSystem effect)
    {
        if (effect != null && !matchEffects.Contains(effect))
        {
            matchEffects.Add(effect);
        }
    }

    private void ConfigureMatchEffects()
    {
        foreach (ParticleSystem effect in matchEffects)
        {
            var main = effect.main;
            main.loop = false;
            main.playOnAwake = false;
            main.duration = 0.8f;
            main.startLifetime = new ParticleSystem.MinMaxCurve(0.55f, 1.35f);
            main.startSpeed = new ParticleSystem.MinMaxCurve(4f, 11f);
            main.startSize = new ParticleSystem.MinMaxCurve(0.18f, 0.55f);
            main.gravityModifier = new ParticleSystem.MinMaxCurve(0.05f, 0.35f);
            main.maxParticles = 500;

            var emission = effect.emission;
            emission.rateOverTime = 0f;

            var shape = effect.shape;
            shape.enabled = true;
            shape.shapeType = ParticleSystemShapeType.Circle;
            shape.radius = 0.55f;
            shape.arc = 360f;

            var colorOverLifetime = effect.colorOverLifetime;
            colorOverLifetime.enabled = true;
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new[]
                {
                    new GradientColorKey(Color.white, 0f),
                    new GradientColorKey(Color.yellow, 0.35f),
                    new GradientColorKey(Color.cyan, 0.7f),
                    new GradientColorKey(Color.magenta, 1f)
                },
                new[]
                {
                    new GradientAlphaKey(1f, 0f),
                    new GradientAlphaKey(1f, 0.5f),
                    new GradientAlphaKey(0f, 1f)
                });
            colorOverLifetime.color = gradient;

            ParticleSystemRenderer renderer = effect.GetComponent<ParticleSystemRenderer>();
            if (renderer != null)
            {
                renderer.sortingOrder = 200;
            }
        }
    }

    private void PlayMatchEffects()
    {
        Vector3 effectPosition = buttonBehaviour.colorButton != null
            ? buttonBehaviour.colorButton.transform.position
            : mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 5f));

        foreach (ParticleSystem effect in matchEffects)
        {
            effect.transform.position = effectPosition;
            effect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            effect.Emit(90);
            effect.Play();
        }

        transform.DOKill();
        mainCamera.transform.DOKill();
        transform.DOPunchScale(Vector3.one * 0.28f, 0.35f, 10, 0.8f);
        mainCamera.transform.DOShakePosition(0.35f, 0.75f, 18, 90f);
        mainCamera.transform.DOShakeRotation(0.25f, 2.5f, 12, 80f);
    }

    public StatsBase GetStatsBase => statsBase;
    public UIManager GetUiManager => uiManager;
    public ButtonBehaviour GetButtonBehaviour => buttonBehaviour;
    public BackgroundBehaviour GetBackgroundBehaviour => backgroundBehaviour;
    public SoundDatabase GetSoundDatabase => soundDatabase;
    public ColorDatabase GetColorDatabase => colorDatabase;
    public AudioSource GsoundEffects => soundEffects;
    public AudioSource GbackgroundMusic => backgroundMusic;
    public Camera GmainCamera => mainCamera;
    public BonusHistory GetBonusHistory => bonusHistory;
}

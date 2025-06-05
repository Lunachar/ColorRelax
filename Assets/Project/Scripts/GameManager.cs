using UnityEngine;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    [Header("ALL SYSTEM REFERENCES")]
    [SerializeField] private UIManager uiManager;
    [SerializeField] private StatsBase statsBase;
    [SerializeField] private ButtonBehaviour buttonBehaviour;
    [SerializeField] private BackgroundBehaviour backgroundBehaviour;
    
    [SerializeField] private Camera mainCamera;
    [SerializeField] private ParticleSystem fireworks;
    [SerializeField] private AudioSource backgroundMusic;
    [SerializeField] private AudioSource soundEffects;

    
    [SerializeField] private ColorDatabase colorDatabase;
    [SerializeField] private SoundDatabase soundDatabase;
 

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        backgroundMusic.Play();
        ChangeColors();
    }

    public void CheckMatch()
    {
        if (backgroundBehaviour.GetCurrentBackgroundColor() == buttonBehaviour.GetCurrentButtonColor())
        {
            fireworks.Play();
            transform.DOPunchScale(Vector3.one * 0.2f, 0.3f);
            mainCamera.DOShakePosition(0.3f, 0.5f);

            if (soundDatabase.matchSound != null)
            {
                soundEffects.PlayOneShot(soundDatabase.matchSound);
            }
        }
    }

    private void ChangeColors()
        {
            backgroundBehaviour.ChangeBackgroundColor();
            buttonBehaviour.ChangeButtonColorAndPosition();
        }
    public StatsBase GetStatsBase => statsBase;
    public UIManager GetUiManager => uiManager;
    public ButtonBehaviour GetButtonBehaviour => buttonBehaviour;
    public SoundDatabase GetSoundDatabase => soundDatabase;
    public ColorDatabase GetColorDatabase => colorDatabase;
    public AudioSource GsoundEffects => soundEffects;
    public AudioSource GbackgroundMusic => backgroundMusic;
    public Camera GmainCamera => mainCamera;
}
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NameInputUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private Button submitButton;
    [SerializeField] private GameObject panel;

    private void Awake()
    {
        submitButton.onClick.AddListener(SaveNameAndAddToLeaderboard);
        transform.localScale = Vector3.one;
        panel.SetActive(false);
    }

    public void Show()
    {
        Time.timeScale = 0f;
        transform.localScale = Vector3.one;
        panel.SetActive(true);
        nameInputField.text = "";
        nameInputField.characterLimit = 7;
        nameInputField.ActivateInputField();
    }

    private void SaveNameAndAddToLeaderboard()
    {
        string playerName = nameInputField.text.Trim();
        if (string.IsNullOrEmpty(playerName)) return;

        int score = GameManager.instance.GetStatsBase.totalScore;
        LeaderboardManager.instance.AddEntry(playerName, score);

        panel.SetActive(false);
        GameManager.instance.GetUiManager.ShowLeaderboardUI();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ExitManager : MonoBehaviour
{
    [SerializeField] private GameObject exitConformationPanel;
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;
    [SerializeField] private ClickStats clickStats;

    private void Start()
    {
        exitConformationPanel.SetActive(false);

        yesButton.onClick.AddListener(ConfirmExit);
        noButton.onClick.AddListener(CancelExit);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleExitPanel();
        }
    }

    private void ToggleExitPanel()
    {
        bool isActive = !exitConformationPanel.activeSelf;
        exitConformationPanel.SetActive(isActive);

        if (isActive)
        {
            Time.timeScale = 0;
            FindObjectOfType<EventSystem>().enabled = true;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    public void ConfirmExit()
    {
        clickStats.ResetStats();
        clickStats.SaveToJson();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void CancelExit()
    {
       ToggleExitPanel();
    }

}

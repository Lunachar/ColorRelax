using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ExitManager : MonoBehaviour
{
    [SerializeField] private GameObject exitConfirmationPanel;
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;
    [FormerlySerializedAs("clickStats")] [SerializeField] private StatsBase statsBase;

    private void Start()
    {
        exitConfirmationPanel.SetActive(false);

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
        bool isActive = !exitConfirmationPanel.activeSelf;
        exitConfirmationPanel.SetActive(isActive);

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
        statsBase.ResetStats();
        statsBase.SaveToJson();

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

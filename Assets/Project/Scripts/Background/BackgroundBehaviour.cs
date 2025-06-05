using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class BackgroundBehaviour : MonoBehaviour
{
    
    private Color currentBackgroundColor;
    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.instance;
    }
    
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ChangeBackgroundColor();
        }
    }

    public void ChangeBackgroundColor()
    {
        currentBackgroundColor = gameManager.GetColorDatabase
            .availibleColors[Random.Range(0, gameManager.GetColorDatabase.availibleColors.Count)];
        gameManager.GmainCamera.backgroundColor = currentBackgroundColor;
        gameManager.CheckMatch();
    }
    public Color GetCurrentBackgroundColor() => currentBackgroundColor;
}

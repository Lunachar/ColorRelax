using UnityEngine;
using UnityEngine.EventSystems;
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
        if (Input.GetMouseButtonDown(0) && !IsPointerOverUi())
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

    private bool IsPointerOverUi()
    {
        if (EventSystem.current == null)
        {
            return false;
        }

        if (Input.touchCount > 0)
        {
            return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
        }

        return EventSystem.current.IsPointerOverGameObject();
    }
}

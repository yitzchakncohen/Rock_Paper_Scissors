using System;
using System.Collections;
using System.Collections.Generic;
using RockPaperScissors;
using UnityEngine;

public class GameMenu : MonoBehaviour
{
    [SerializeField] private GameObject gameMenuPanel;
    [SerializeField] private GameObject gameOverMenuPanel;

    private void Start() 
    {
        GameplayManager.OnGameOver += GameplayManager_OnGameOver;
    }

    private void OnDestroy() 
    {
        GameplayManager.OnGameOver -= GameplayManager_OnGameOver;        
    }

    public void OpenGameMenu()
    {
        gameMenuPanel.SetActive(true);
    }

    public void CloseGameMenu()
    {
        gameMenuPanel.SetActive(false);
    }

    private void OpenGameOverMenu()
    {
        gameOverMenuPanel.SetActive(true);
    }

    private void CloseGameOverMenu()
    {
        gameOverMenuPanel.SetActive(false);
    }

    public void GoToMainMenu()
    {
        ApplicationManager.Instance.ReturnToMenu();
    }

    private void GameplayManager_OnGameOver()
    {
        OpenGameOverMenu();
    }
}

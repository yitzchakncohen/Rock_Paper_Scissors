using System;
using System.Collections;
using System.Collections.Generic;
using RockPaperScissors;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameMenu : MonoBehaviour
{
    public static event Action OnStartGameButtonPress;

    [SerializeField] private GameObject gameMenuPanel;
    [SerializeField] private GameObject gameOverMenuPanel;
    [SerializeField] private TextMeshProUGUI gameOverScoreText;
    [SerializeField] private Button[] MainMenuButtons;
    [SerializeField] private Button NewGameButton;


    private void Start() 
    {
        GameplayManager.OnGameOver += GameplayManager_OnGameOver;
        foreach (Button button in MainMenuButtons)
        {
            button.onClick.AddListener(GoToMainMenu);
        }
        NewGameButton.onClick.AddListener(StartGame);

        gameOverMenuPanel.SetActive(false);
        gameMenuPanel.SetActive(false);
    }

    private void OnDestroy() 
    {
        foreach (Button button in MainMenuButtons)
        {
            button.onClick.RemoveAllListeners();
        }
        NewGameButton.onClick.RemoveAllListeners();
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

    private void OpenGameOverMenu(int score)
    {
        gameOverMenuPanel.SetActive(true);
        gameOverScoreText.text = "Score: " + score.ToString();
    }

    private void CloseGameOverMenu()
    {
        gameOverMenuPanel.SetActive(false);
    }

    private void GoToMainMenu()
    {
        ApplicationManager.Instance.ReturnToMenu();
    }

    private void StartGame()
    {
        OnStartGameButtonPress?.Invoke();
    }

    private void GameplayManager_OnGameOver(int score)
    {
        OpenGameOverMenu(score);
    }

}

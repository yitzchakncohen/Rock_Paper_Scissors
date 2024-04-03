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

    [SerializeField] private GameObject HUDPanel;
    [SerializeField] private GameObject gameMenuPanel;
    [SerializeField] private GameObject gameOverMenuPanel;
    [SerializeField] private TextMeshProUGUI gameOverScoreValueText;
    [SerializeField] private TextMeshProUGUI gameOverHighScoreValueText;
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
        HUDPanel.SetActive(true);
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
        AudioManager.Instance.PlayMenuNavigationSound();
    }

    public void CloseGameMenu()
    {
        gameMenuPanel.SetActive(false);
        AudioManager.Instance.PlayMenuNavigationSound();
    }

    private void OpenGameOverMenu(int score, int highscore)
    {
        HUDPanel.SetActive(false);
        gameOverMenuPanel.SetActive(true);
        gameOverScoreValueText.text = score.ToString();
        gameOverHighScoreValueText.text = highscore.ToString();
    }

    private void CloseGameOverMenu()
    {
        gameOverMenuPanel.SetActive(false);
    }

    private void GoToMainMenu()
    {
        ApplicationManager.Instance.ReturnToMenu();
        AudioManager.Instance.PlayMenuNavigationSound();
    }

    private void StartGame()
    {
        OnStartGameButtonPress?.Invoke();
        AudioManager.Instance.PlayMenuNavigationSound();
    }

    private void GameplayManager_OnGameOver(object sender, GameplayManager.OnGameOverEventArgs e)
    {
        OpenGameOverMenu(e.Score, e.Highscore);
    }

}

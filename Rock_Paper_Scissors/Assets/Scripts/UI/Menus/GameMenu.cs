using System;
using DG.Tweening;
using RockPaperScissors.UI.Components;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RockPaperScissors.UI.Menus
{
    public class GameMenu : MonoBehaviour
    {
        [SerializeField] private CanvasGroup HUDPanel;
        [SerializeField] private ModalWindow gameMenuPanel;
        [SerializeField] private GameOverMenu gameOverMenuPanel;
        [SerializeField] private ModalWindow howToPlayModal;
        [SerializeField] private Button[] mainMenuButtons;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button closeMenuButton;
        [SerializeField] private Button endGameMenuButton;
        [SerializeField] private Button helpButton;
        [SerializeField] private AdModal adModal;
        [SerializeField] private ModalWindow settingsModal;
        [SerializeField] private ModalWindow gameModeModal;

        private void Start() 
        {
            GameplayManager.OnGameOver += GameplayManager_OnGameOver;
            GameplayManager.OnLevelCompleted += GameplayManager_OnLevelCompleted;
            GameOverMenu.OnNextLevelButtonPress += GameOverMenu_OnNextLevelButtonPress;
            GameOverMenu.OnRestartLevelButtonPress += GameOverMenu_OnRestartLevelButtonPress;
            GameOverMenu.OnStartEndlessGameButtonPress += GameOverMenu_OnStartEndlessGameButton;
            foreach (Button button in mainMenuButtons)
            {
                button.onClick.AddListener(GoToMainMenu);
            }
            settingsButton.onClick.AddListener(OpenSettingsMenu);
            closeMenuButton.onClick.AddListener(CloseGameMenu);
            helpButton.onClick.AddListener(OpenHowToPlayMenu);
            endGameMenuButton.onClick.AddListener(EndGame);

            // Setup UI
            gameOverMenuPanel.gameObject.SetActive(false);
            gameMenuPanel.gameObject.SetActive(false);
            HUDPanel.gameObject.SetActive(true);
            adModal.gameObject.SetActive(false);
            settingsModal.gameObject.SetActive(false);
            howToPlayModal.gameObject.SetActive(false);
        }

        private void OnDestroy() 
        {
            foreach (Button button in mainMenuButtons)
            {
                button.onClick.RemoveAllListeners();
            }
            settingsButton.onClick.RemoveAllListeners();
            closeMenuButton.onClick.RemoveAllListeners();   
            helpButton.onClick.RemoveAllListeners();
            endGameMenuButton.onClick.RemoveAllListeners();
            GameplayManager.OnGameOver -= GameplayManager_OnGameOver;    
            GameplayManager.OnLevelCompleted -= GameplayManager_OnLevelCompleted;
            GameOverMenu.OnNextLevelButtonPress -= GameOverMenu_OnNextLevelButtonPress;
            GameOverMenu.OnRestartLevelButtonPress -= GameOverMenu_OnRestartLevelButtonPress;
            GameOverMenu.OnStartEndlessGameButtonPress -= GameOverMenu_OnStartEndlessGameButton;   
        }

        public void OpenGameMenu()
        {
            if(!gameMenuPanel.gameObject.activeSelf)
            {
                gameMenuPanel.Open();
            }
        }

        public void CloseGameMenu()
        {
            if(gameMenuPanel.gameObject.activeSelf)
            {
                gameMenuPanel.Close();
            }
        }

        public void OpenSettingsMenu()
        {
            if(!settingsModal.gameObject.activeSelf)
            {
                settingsModal.Open();
            }
        }

        private void OpenHowToPlayMenu()
        {
            if(!howToPlayModal.gameObject.activeSelf)
            {
                howToPlayModal.Open();
            }
        }

        public void CloseSettingsMenu()
        {
            if(settingsModal.gameObject.activeSelf)
            {
                settingsModal.Close();
            }
        }

        public void OpenGameOverMenu(int score, int highscore, GameMode gameMode, LevelData levelData, bool winCondition = false)
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(HUDPanel.DOFade(0f, 1f));
            sequence.AppendCallback(()=> {
                HUDPanel.gameObject.SetActive(false);
                gameOverMenuPanel.Open(score, highscore, gameMode, levelData, winCondition);
            });
        }

        private void GoToMainMenu()
        {
            ApplicationManager.Instance.ReturnToMenu();
            AudioManager.Instance.PlayMenuNavigationSound();
        }

        private void EndGame()
        {
            FindObjectOfType<GameplayManager>().GameOver();
        }

        private void GameplayManager_OnGameOver(object sender, GameplayManager.OnGameOverEventArgs e)
        {
            adModal.SetupTimer();
            gameMenuPanel.Close();
            OpenGameOverMenu(e.Score, e.Highscore, e.GameMode, e.LevelData, e.WinCondition);
        }

        private void GameplayManager_OnLevelCompleted(object sender, GameplayManager.OnGameOverEventArgs e)
        {
            adModal.SetupTimer();
            gameMenuPanel.Close();
            OpenGameOverMenu(e.Score, e.Highscore, e.GameMode, e.LevelData, e.WinCondition);
        }

        private void GameOverMenu_OnStartEndlessGameButton()
        {
            adModal.Open();
        }

        private void GameOverMenu_OnRestartLevelButtonPress()
        {
            adModal.Open();
        }

        private void GameOverMenu_OnNextLevelButtonPress()
        {
            adModal.Open();
        }
    }
}

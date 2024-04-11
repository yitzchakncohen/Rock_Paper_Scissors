using System;
using System.IO;
using RockPaperScissors.SaveSystem;
using RockPaperScissors.UI.Components;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RockPaperScissors.UI.Menus
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private ModalWindow settingsModal;
        [SerializeField] private ModalWindow howToPlayModal;
        [SerializeField] private GameModeMenu gameModeMenu;
        private ModalWindow gameModeModal;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button startButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button howToPlayButton;
        [SerializeField] private TextMeshProUGUI highscoreText;
        public static event Action OnStartGameButtonPress;
        public static event Action OnContinueGameButtonPress;

        private void Start() 
        {
            startButton.onClick.AddListener(StartGame);

            // Only enable the continue button if there is a saved game.
            if(File.Exists(Application.persistentDataPath + SaveManager.SAVE_DIRECTORY + SaveManager.SAVE_FILE_NAME))
            {
                continueButton.interactable = true;
                continueButton.onClick.AddListener(ContinueGame);
            }
            else
            {
                continueButton.interactable = false;
            }
            
            int highscore = PlayerPrefs.GetInt(ApplicationManager.HIGH_SCORE_STRING, -1);
            if(highscore != -1)
            {
                highscoreText.text = "HIGHSCORE: " + highscore; 
            }
            else
            {
                highscoreText.gameObject.SetActive(false);
            }

            settingsButton.onClick.AddListener(OpenSettingsMenu);
            settingsModal.gameObject.SetActive(false);
            howToPlayButton.onClick.AddListener(OpenHowToPlayMenu);
            howToPlayModal.gameObject.SetActive(false);

            gameModeModal = gameModeMenu.GetComponent<ModalWindow>();
            gameModeMenu.OnHighScoreModeButtonPress += GameModeMenu_OnHighScoreModeButtonPress;
            gameModeMenu.OnLevelModeButtonPress += GameModeMenu_OnLevelModeButtonPress;
            gameModeMenu.gameObject.SetActive(false);
        }

        private void OnDestroy() 
        {
            continueButton.onClick.RemoveAllListeners();
            startButton.onClick.RemoveAllListeners();
            settingsButton.onClick.RemoveAllListeners();
            gameModeMenu.OnHighScoreModeButtonPress -= GameModeMenu_OnHighScoreModeButtonPress;
            gameModeMenu.OnLevelModeButtonPress -= GameModeMenu_OnLevelModeButtonPress;
        }

        private void StartGame()
        {
            AudioManager.Instance.PlayMenuNavigationSound();
            gameModeModal.Open();
        }

        private void ContinueGame()
        {
            AudioManager.Instance.PlayMenuNavigationSound();
            OnContinueGameButtonPress?.Invoke();
        }

        private void OpenSettingsMenu()
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

        private void GameModeMenu_OnLevelModeButtonPress()
        {
            throw new NotImplementedException();
        }

        private void GameModeMenu_OnHighScoreModeButtonPress()
        {
            OnStartGameButtonPress?.Invoke();
        }
    }
}

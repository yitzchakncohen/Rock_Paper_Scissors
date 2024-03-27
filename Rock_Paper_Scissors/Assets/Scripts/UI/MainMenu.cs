using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace RockPaperScissors.UI
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private Button continueButton;
        [SerializeField] private Button startButton;
        public static event Action OnStartGameButtonPress;
        public static event Action OnContinueGameButtonPress;

        private void Start() 
        {
            startButton.onClick.AddListener(StartGame);

            // Only enable the continue button if there is a saved game.
            if(File.Exists(Application.persistentDataPath + ApplicationManager.SAVE_DIRECTORY + ApplicationManager.SAVE_FILE_NAME))
            {
                continueButton.interactable = true;
                continueButton.onClick.AddListener(ContinueGame);
            }
            else
            {
                continueButton.interactable = false;
            }
        }

        private void OnDestroy() 
        {
            continueButton.onClick.RemoveAllListeners();
            startButton.onClick.RemoveAllListeners();
        }

        private void StartGame()
        {
            OnStartGameButtonPress?.Invoke();
        }

        private void ContinueGame()
        {
            OnContinueGameButtonPress?.Invoke();
        }
    }
}

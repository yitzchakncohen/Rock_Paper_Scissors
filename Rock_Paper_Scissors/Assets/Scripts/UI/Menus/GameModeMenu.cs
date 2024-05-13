using System;
using System.Collections;
using System.Collections.Generic;
using RockPaperScissors.UI.Components;
using UnityEngine;
using UnityEngine.UI;

namespace RockPaperScissors.UI.Menus
{
    [RequireComponent(typeof(ModalWindow))]
    public class GameModeMenu : MonoBehaviour
    {
        public event Action OnLevelModeButtonPress;
        public event Action OnHighScoreModeButtonPress;
        [SerializeField] private Button highScoreModeButton;
        [SerializeField] private Button levelModeButton;
        [SerializeField] private Button closeButton;

        private ModalWindow modalWindow;

        private void Awake()
        {
            modalWindow = GetComponent<ModalWindow>();
            // Level Game Mode Disabled
            levelModeButton.interactable = false;
        }

        private void OnEnable() 
        {
            highScoreModeButton.onClick.AddListener(HighScoreModeButtonPress);
            levelModeButton.onClick.AddListener(LevelModeButtonPress);
            closeButton.onClick.AddListener(CloseButtonPress);
        }

        private void OnDisable() 
        {
            highScoreModeButton.onClick.RemoveAllListeners();
            levelModeButton.onClick.RemoveAllListeners();   
        }

        private void LevelModeButtonPress()
        {
            OnLevelModeButtonPress?.Invoke();
            AudioManager.Instance.PlayMenuNavigationSound();
        }

        private void HighScoreModeButtonPress()
        {
            OnHighScoreModeButtonPress?.Invoke();
            AudioManager.Instance.PlayMenuNavigationSound();
        }  

        private void CloseButtonPress()
        {
            modalWindow.Close();
            AudioManager.Instance.PlayMenuNavigationSound();
        }      
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using RockPaperScissors.UI.Components;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RockPaperScissors.UI.Menus
{
    [RequireComponent(typeof(ModalWindow))]
    public class HowToPlayMenu : MonoBehaviour
    {
        public event Action OnPlayButtonPress;
        [SerializeField] private GameObject[] howToPlayPanels;
        [SerializeField] private Button backButton;
        [SerializeField] private Button nextButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private Button playButton;
        [SerializeField] private TextMeshProUGUI pageNumberText;
        private ModalWindow modalWindow;
        private int currentPage = 0;

        private void Awake() 
        {
            modalWindow = GetComponent<ModalWindow>();   
        }

        private void OnEnable() 
        {
            backButton.onClick.AddListener(BackButtonOnClick);
            nextButton.onClick.AddListener(NextButtonOnClick);
            closeButton.onClick.AddListener(CloseButtonOnClick);
            playButton.onClick.AddListener(PlayButtonOnClick);
            UpdateButtonInteractability();
            // Open first page
            CloseAllHowToPlayPanels();
            currentPage = 0;
            howToPlayPanels[currentPage].SetActive(true); 
            UpdateButtonInteractability();
        }

        private void OnDisable() 
        {
            backButton.onClick.RemoveAllListeners();
            nextButton.onClick.RemoveAllListeners();
            closeButton.onClick.RemoveAllListeners();
            playButton.onClick.RemoveAllListeners();
        }

        private void UpdateButtonInteractability()
        {
            if(currentPage == 0)
            {
                backButton.interactable = false;
                nextButton.gameObject.SetActive(true);
                playButton.gameObject.SetActive(false);  
                nextButton.interactable = true;
            }
            else if(currentPage == howToPlayPanels.Length - 1)
            {
                nextButton.interactable = false;
                nextButton.gameObject.SetActive(false);
                playButton.gameObject.SetActive(true);  
                backButton.interactable = true;
            }
            else
            {
                nextButton.interactable = true;
                nextButton.gameObject.SetActive(true);
                playButton.gameObject.SetActive(false);  
                backButton.interactable = true;
            }

            pageNumberText.text = $"{currentPage + 1} of {howToPlayPanels.Length}";
        }

        private void CloseButtonOnClick()
        {
            modalWindow.Close();
        }

        private void PlayButtonOnClick()
        {
            OnPlayButtonPress?.Invoke();
            CloseButtonOnClick();
        }

        private void NextButtonOnClick()
        {
            currentPage = Math.Clamp(currentPage+1, 0, howToPlayPanels.Length-1);
            CloseAllHowToPlayPanels();
            howToPlayPanels[currentPage].SetActive(true);
            UpdateButtonInteractability();
        }

        private void BackButtonOnClick()
        {
            currentPage = Math.Clamp(currentPage-1, 0, howToPlayPanels.Length-1);
            CloseAllHowToPlayPanels();
            howToPlayPanels[currentPage].SetActive(true);
            UpdateButtonInteractability();
        }

        private void CloseAllHowToPlayPanels()
        {
            Array.ForEach(howToPlayPanels, panel => panel.SetActive(false));    
        }
    }    
}

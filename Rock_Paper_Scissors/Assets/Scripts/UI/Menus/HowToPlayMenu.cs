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
        [SerializeField] private GameObject[] howToPlayPanels;
        [SerializeField] private Button backButton;
        [SerializeField] private Button nextButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private TextMeshProUGUI pageNumberText;
        private ModalWindow modalWindow;
        private int currentPage = 0;

        private void Awake() 
        {
            modalWindow = GetComponent<ModalWindow>();   
        }

        private void OnEnable() 
        {
            backButton.onClick.AddListener(OnBackButtonPress);
            nextButton.onClick.AddListener(OnNextButtonPress);
            closeButton.onClick.AddListener(OnCloseButtonPress);
            UpdateButtonInteractability();
        }

        private void OnDisable() 
        {
            backButton.onClick.RemoveAllListeners();
            nextButton.onClick.RemoveAllListeners();
            closeButton.onClick.RemoveAllListeners();
        }

        private void UpdateButtonInteractability()
        {
            if(currentPage == 0)
            {
                backButton.interactable = false;
                nextButton.interactable = true;
            }
            else if(currentPage == howToPlayPanels.Length - 1)
            {
                nextButton.interactable = false;
                backButton.interactable = true;
            }
            else
            {
                nextButton.interactable = true;
                backButton.interactable = true;
            }

            pageNumberText.text = $"{currentPage + 1} of {howToPlayPanels.Length}";
        }

        private void OnCloseButtonPress()
        {
            modalWindow.Close();
        }

        private void OnNextButtonPress()
        {
            currentPage = Math.Clamp(currentPage+1, 0, howToPlayPanels.Length-1);
            CloseAllHowToPlayPanels();
            howToPlayPanels[currentPage].SetActive(true);
            UpdateButtonInteractability();
        }

        private void OnBackButtonPress()
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

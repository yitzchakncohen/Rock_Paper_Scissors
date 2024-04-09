using System;
using System.Collections;
using System.Collections.Generic;
using RockPaperScissors.SaveSystem;
using UnityEngine;
using UnityEngine.UI;

namespace RockPaperScissors.UI
{
    public class SaveButton : MonoBehaviour
    {
        public static event Action OnSaveButtonPress;
        private Button button;

        private void Awake() 
        {  
            button = GetComponent<Button>();
        }

        private void OnEnable() 
        {
            SaveManager.OnSaveCompleted += SaveManager_OnSaveCompleted;
        }

        private void OnDisable() 
        {
            SaveManager.OnSaveCompleted -= SaveManager_OnSaveCompleted;
        }

        public void SaveButtonPress()
        {
            OnSaveButtonPress?.Invoke();
            button.interactable = false;
        }

        private void SavingComplete()
        {
            button.interactable = true;
        }

        private void SaveManager_OnSaveCompleted()
        {
            SavingComplete();
        }
    }
}

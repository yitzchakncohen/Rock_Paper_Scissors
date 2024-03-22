using System;
using System.Collections;
using System.Collections.Generic;
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

        public void SaveButtonPress()
        {
            OnSaveButtonPress?.Invoke();
            button.interactable = false;
        }

        public void SavingComplete()
        {
            button.interactable = true;
        }
    }
}

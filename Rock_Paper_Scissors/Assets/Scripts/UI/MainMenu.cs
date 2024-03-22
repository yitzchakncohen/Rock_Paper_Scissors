using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RockPaperScissors.UI
{
    public class MainMenu : MonoBehaviour
    {
        public static event Action OnStartGameButtonPress;
        public static event Action OnContinueGameButtonPress;

        public void StartGame()
        {
            OnStartGameButtonPress?.Invoke();
        }

        public void ContinueGame()
        {
            OnContinueGameButtonPress?.Invoke();
        }
    }
}

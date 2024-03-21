using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RockPaperScissors.UI
{
    public class MainMenu : MonoBehaviour
    {
        public void StartGame()
        {
            ApplicationManager.Instance.StartNewGame();
        }

        public void ContinueGame()
        {
            ApplicationManager.Instance.ContinueGame();
        }
    }
}

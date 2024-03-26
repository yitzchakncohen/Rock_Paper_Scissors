using System.Collections;
using System.Collections.Generic;
using RockPaperScissors;
using UnityEngine;

public class GameMenu : MonoBehaviour
{
    [SerializeField] private GameObject gameMenuPanel;

    public void OpenGameMenu()
    {
        gameMenuPanel.SetActive(true);
    }

    public void CloseGameMenu()
    {
        gameMenuPanel.SetActive(false);
    }

    public void GoToMainMenu()
    {
        ApplicationManager.Instance.ReturnToMenu();
    }
}

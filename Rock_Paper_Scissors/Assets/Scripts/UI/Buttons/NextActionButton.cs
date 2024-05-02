using System;
using System.Collections;
using System.Collections.Generic;
using RockPaperScissors.SaveSystem;
using UnityEngine;
using UnityEngine.UI;

namespace RockPaperScissors.UI.Buttons
{
    public class NextActionButton : MonoBehaviour
    {
        private ActionHandler actionHandler;
        private Button button;

        private void Start() 
        {
            actionHandler = FindObjectOfType<ActionHandler>();
            TurnManager.OnNextTurn += TurnManager_OnNextTurn;
            WaveManager.OnWaveCompleted += WaveManager_OnWaveCompleted;
            WaveManager.OnWaveStarted += WaveManager_OnWaveStarted;
            SaveManager.OnLoadCompleted += SaveManager_OnLoadCompleted;
            button = GetComponent<Button>();
            button.interactable = false;
            button.onClick.AddListener(() => actionHandler.SelectNextAvaliableUnit());
        }

        private void OnDestroy() 
        {
            TurnManager.OnNextTurn -= TurnManager_OnNextTurn;
            WaveManager.OnWaveCompleted -= WaveManager_OnWaveCompleted;
            WaveManager.OnWaveStarted -= WaveManager_OnWaveStarted;
            SaveManager.OnLoadCompleted -= SaveManager_OnLoadCompleted;
            button.onClick.RemoveAllListeners();
        }

        private void TurnManager_OnNextTurn(object sender, TurnManager.OnNextTurnEventArgs e)
        {
            if(e.IsPlayersTurn)
            {
                button.interactable = true;
            }
            else
            {
                button.interactable = false;
            }            
        }

        private void WaveManager_OnWaveCompleted()
        {
            button.interactable = true;
        }

        private void WaveManager_OnWaveStarted()
        {
            button.interactable = false;
        }

        private void SaveManager_OnLoadCompleted()
        {
            button.interactable = true;
        }
    }
}

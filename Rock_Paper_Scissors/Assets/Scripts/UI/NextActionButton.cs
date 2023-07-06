using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RockPaperScissors.UI
{
    public class NextActionButton : MonoBehaviour
    {
        private Button button;

        private void Start() 
        {
            TurnManager.OnNextTurn += TurnManager_OnNextTurn;
            WaveManager.OnWaveCompleted += WaveManager_OnWaveCompleted;
            WaveManager.OnWaveStarted += WaveManager_OnWaveStarted;
            button = GetComponent<Button>();
            button.interactable = false;
        }

        private void OnDestroy() 
        {
            TurnManager.OnNextTurn -= TurnManager_OnNextTurn;
            WaveManager.OnWaveCompleted -= WaveManager_OnWaveCompleted;
            WaveManager.OnWaveStarted -= WaveManager_OnWaveStarted;
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
    }
}

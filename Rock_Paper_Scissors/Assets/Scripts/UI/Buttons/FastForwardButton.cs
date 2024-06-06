using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RockPaperScissors.UI.Buttons
{
    public class FastForwardButton : MonoBehaviour
    {
        [SerializeField] private GameObject buttonHighlight;
        private bool fastForwarding = false;

        private void Start() 
        {
            TurnManager.OnNextTurn += TurnManager_OnNextTurn;
            WaveManager.OnWaveCompleted += WaveManager_OnWaveCompleted;
        }

        private void OnDestroy() 
        {
            TurnManager.OnNextTurn -= TurnManager_OnNextTurn;
            WaveManager.OnWaveCompleted -= WaveManager_OnWaveCompleted;
        }

        private void WaveManager_OnWaveCompleted()
        {
            DisableFastForwarding();
        }

        private void TurnManager_OnNextTurn(object sender, TurnManager.OnNextTurnEventArgs e)
        {
            DisableFastForwarding();
        }

        public void ToggleFastForward()
        {
            if(fastForwarding)
            {
                DisableFastForwarding();
            }
            else
            {
                EnableFastForwarding();
            }
        }

        private void EnableFastForwarding()
        {
            TimeScaleManager.SpeedUpTimeScale();
            buttonHighlight.SetActive(true);
            fastForwarding = true;
        }

        private void DisableFastForwarding()
        {
            TimeScaleManager.ReturnToNormalSpeed();
            buttonHighlight.SetActive(false);
            fastForwarding = false;
        }
    }
}

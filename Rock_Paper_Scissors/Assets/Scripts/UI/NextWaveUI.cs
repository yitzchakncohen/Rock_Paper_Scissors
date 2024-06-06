using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace RockPaperScissors.UI
{
    public class NextWaveUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nextWaveText;
        [SerializeField] private GameObject nextWaveHighlight;

        private void OnEnable() 
        {
            WaveManager.OnTurnsUntilNextWaveUpdated += WaveManager_OnTurnsUntilNextWaveUpdated;
        }

        private void OnDisable() 
        {
            WaveManager.OnTurnsUntilNextWaveUpdated -= WaveManager_OnTurnsUntilNextWaveUpdated;
        }

        private void WaveManager_OnTurnsUntilNextWaveUpdated(int turnsUntilNextWave)
        {
            if(turnsUntilNextWave == 0)
            {
                nextWaveText.text = "-";
            }
            else
            {
                nextWaveText.text = turnsUntilNextWave.ToString();
            }
            if(turnsUntilNextWave == 1)
            {
                nextWaveHighlight.SetActive(true);
            }
            else
            {
                nextWaveHighlight.SetActive(false);
            }
        }
    }    
}

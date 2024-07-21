using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using RockPaperScissors.SaveSystem;

namespace RockPaperScissors.UI
{
    public class NextWaveUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nextWaveText;
        [SerializeField] private GameObject nextWaveHighlight;

        private void Awake()
        {
            SaveManager_OnLoadCompleted();
            SaveManager.OnLoadCompleted += SaveManager_OnLoadCompleted;
        }

        private void SaveManager_OnLoadCompleted()
        {
            bool active = FindObjectOfType<GameplayManager>().GameMode == GameMode.Endless;
            gameObject.SetActive(active);
        }

        private void OnDestroy() 
        {
            SaveManager.OnLoadCompleted -= SaveManager_OnLoadCompleted;
        }

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

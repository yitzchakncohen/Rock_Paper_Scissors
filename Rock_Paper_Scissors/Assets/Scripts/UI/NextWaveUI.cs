using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NextWaveUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nextWaveText;
    
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
        nextWaveText.text = turnsUntilNextWave.ToString();
    }
}

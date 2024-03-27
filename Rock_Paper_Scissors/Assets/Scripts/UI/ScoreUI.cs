using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace RockPaperScissors.UI
{
    public class ScoreUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI scoreText;

        private void OnEnable() 
        {
            GameplayManager.OnScoreChange += GameplayManager_OnScoreChange;
            scoreText.text = "0";
        }

        private void OnDisable() 
        {
            GameplayManager.OnScoreChange -= GameplayManager_OnScoreChange;
        }

        private void GameplayManager_OnScoreChange(int score)
        {
            scoreText.text = score.ToString();
        }
    }    
}

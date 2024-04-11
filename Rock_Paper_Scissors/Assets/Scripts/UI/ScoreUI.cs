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
        private float scoreUpdateTime = 0.3f;

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
            StartCoroutine(ScoreUpdateRoutine(score));
        }

        private IEnumerator ScoreUpdateRoutine(int score)
    {
        float startingCurrency = int.Parse(scoreText.text);

        while(startingCurrency <= score-1)
        {
            startingCurrency = Mathf.Lerp(startingCurrency, score, Time.deltaTime/scoreUpdateTime);
            scoreText.text = startingCurrency.ToString();
            // Debug.Log(startingCurrency);
            yield return null;
        }
        scoreText.text = score.ToString();
    }
    }    
}

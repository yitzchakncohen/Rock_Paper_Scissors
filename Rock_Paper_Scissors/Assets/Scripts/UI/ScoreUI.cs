using System;
using System.Collections;
using System.Collections.Generic;
using RockPaperScissors.SaveSystem;
using TMPro;
using UnityEngine;

namespace RockPaperScissors.UI
{
    public class ScoreUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI scoreText;
        private float scoreUpdateTime = 0.3f;

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
        float startingScore = int.Parse(scoreText.text);

        while(startingScore <= score-1)
        {
            startingScore = Mathf.Lerp(startingScore, score, Time.deltaTime/scoreUpdateTime);
            scoreText.text = startingScore.ToString("N0");
            yield return null;
        }
        scoreText.text = score.ToString();
    }
    }    
}

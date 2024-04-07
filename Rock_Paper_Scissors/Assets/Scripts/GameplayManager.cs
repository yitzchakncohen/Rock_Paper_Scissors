using System;
using System.Collections;
using System.Collections.Generic;
using RockPaperScissors.SaveSystem;
using RockPaperScissors.Units;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

namespace RockPaperScissors
{
    /// <summary>
    /// <c>GameplayManager</c> manages the game flow, including identifying end game states. 
    /// </summary>
    public class GameplayManager : MonoBehaviour, ISaveInterface<SaveGameplayManagerData>
    {
        public class OnGameOverEventArgs : EventArgs
        {
            public int Score;
            public int Highscore;
        }
        public static event EventHandler<OnGameOverEventArgs> OnGameOver;
        public static event Action<int> OnScoreChange;
        public static event Action<int> OnNewHighscore;
        private int score = 0;
        
        private void Awake() 
        {
            UnitHealth.OnDeath += UnitHealth_OnDeath;
        }
        private void OnDestroy() 
        {
            UnitHealth.OnDeath -= UnitHealth_OnDeath;
        }

        private void UnitHealth_OnDeath(object sender, Unit attacker)
        {
            if(attacker == null)
            {
                // Self Destruct
                return;
            }

            UnitHealth unitHealth = sender as UnitHealth;
            // Game ends when the pillow fort is destroyed.
            if(unitHealth != null && unitHealth.Unit.GetUnitClass() == UnitClass.PillowFort)
            {
                GameOver();
            }

            // Score points for defeating enemies.
            if (attacker.IsFriendly)
            {
                score += 10;
                OnScoreChange?.Invoke(score);
            }
        }

        [ContextMenu("Game Over")]
        public void GameOver()
        {
            int highscore = PlayerPrefs.GetInt(ApplicationManager.HIGH_SCORE_STRING, -1);
            if(highscore < score)
            {
                PlayerPrefs.SetInt(ApplicationManager.HIGH_SCORE_STRING, score);
                highscore = score;
                OnNewHighscore?.Invoke(score);
            }
            OnGameOverEventArgs onGameOverEventArgs = new OnGameOverEventArgs
            {
                Score = score, 
                Highscore = highscore
            };
            AudioManager.Instance.PlayGameOverSound();
            OnGameOver?.Invoke(this, onGameOverEventArgs);
        }

        public SaveGameplayManagerData Save()
        {
            return new SaveGameplayManagerData
            {
                Score = score
            };
        }

        public void Load(SaveGameplayManagerData loadData)
        {
            score = loadData.Score;
            OnScoreChange?.Invoke(score);
        }
    }
}

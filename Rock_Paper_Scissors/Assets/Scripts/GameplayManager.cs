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
        public static event Action OnGameOver;
        public static event Action<int> OnScoreChange;
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

            // Game ends when the pillow fort is destroyed.
            if(attacker.GetUnitClass() == UnitClass.PillowFort)
            {
                OnGameOver?.Invoke();
            }

            // Score points for defeating enemies.
            if(attacker.IsFriendly())
            {
                score += 10;
                OnScoreChange?.Invoke(score);
            }
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

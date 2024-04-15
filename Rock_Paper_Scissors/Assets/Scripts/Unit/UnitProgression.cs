using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RockPaperScissors.Units
{
    public class UnitProgression : MonoBehaviour
    {
        public event Action OnLevelUp;
        public event Action OnGainXP;
        private UnitAnimator unitAnimator;
        private int level = 1;
        private int xp = 0;

        public int Level => level;
        public int XP {get => xp; set {xp = value;}}

        private void Start() 
        {
            unitAnimator = GetComponentInChildren<UnitAnimator>();
            UnitHealth.OnDeath += Health_OnDeath;
        }

        private void Health_OnDeath(object sender, Unit attacker)
        {
            if(attacker == null)
            {
                // Self Destruct
                return;
            }
            
            if(attacker.UnitProgression == this)
            {
                Unit unitDefeated = (sender as UnitHealth).GetComponent<Unit>();
                GainXP(unitDefeated.UnitDefeatedXPReward, attacker.XPToLevelUp);
            }
        }

        private void GainXP(int amount, int levelUpXPRequired)
        {
            xp += amount;
            OnGainXP?.Invoke();
            CheckForLevelUp(levelUpXPRequired);
        }

        private void CheckForLevelUp(int levelUpXPRequired)
        {
            if(xp == levelUpXPRequired)
            {
                level = Math.Clamp(level + 1, 1, 3);
                OnLevelUp?.Invoke();
                StartCoroutine(unitAnimator.AnimateLevelUp(level));
                AudioManager.Instance.PlayUnitLevelUpSound();
            }
        }

        public void SetLevel(int newLevel)
        {
            int oldLevel = level;
            level = newLevel;
            level = Math.Clamp(level, 1, 3);
            if(level > oldLevel)
            {
                OnLevelUp?.Invoke();
                StartCoroutine(unitAnimator.AnimateLevelUp(level));
                AudioManager.Instance.PlayUnitLevelUpSound();
            }
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RockPaperScissors.Units
{
    public class UnitProgression : MonoBehaviour
    {
        public event Action OnLevelUp;
        private UnitAnimator unitAnimator;
        private int level = 1;
        private int xp = 0;

        private void Start() 
        {
            unitAnimator = GetComponent<UnitAnimator>();
            UnitHealth.OnDeath += Health_OnDeath;
        }

        private void Health_OnDeath(object sender, Unit attacker)
        {
            if(attacker.GetUnitProgression() == this)
            {
                GainXP(100);
            }
        }

        private void GainXP(int amount)
        {
            xp += amount;
            CheckForLevelUp();
        }

        private void CheckForLevelUp()
        {
            if(xp % 100 == 0)
            {
                level = Math.Clamp(level + 1, 1, 3);
                OnLevelUp?.Invoke();
                StartCoroutine(unitAnimator.AnimateLevelUp(level));
            }
        }

        public int GetLevel()
        {
            return level;
        }
    }
}

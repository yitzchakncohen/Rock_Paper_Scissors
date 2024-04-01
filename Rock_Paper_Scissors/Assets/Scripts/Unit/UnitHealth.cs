using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RockPaperScissors.Units
{
    public class UnitHealth : MonoBehaviour
    {
        public static event EventHandler<Unit> OnDeath;
        public event Action OnHealthChanged;
        private Unit unit;
        private UnitProgression unitProgression;
        private UnitAnimator[] unitAnimators;
        [SerializeField] private int health = -1;
        private float deathAnimationTime = 0.6f;

        private void Awake() 
        {
            unit = GetComponent<Unit>();
            unitAnimators = GetComponentsInChildren<UnitAnimator>(true);
        }
        
        private void Start() 
        {
            unitProgression = unit.GetUnitProgression();
            unitProgression.OnLevelUp += UnitProgression_OnLevelUp;
            if(health == -1)
            {
                SetHealth(unit.GetMaximumHealth());
            } 
        }

        private void OnDestroy() 
        {
            unitProgression.OnLevelUp -= UnitProgression_OnLevelUp;
        }

        private void UnitProgression_OnLevelUp()
        {
            SetHealth(unit.GetMaximumHealth());
        }

        public void Damage(int damageAmount, Unit attacker)
        {
            SetHealth(health - damageAmount);
            CheckForDeath(attacker);
        }

        public void CheckForDeath(Unit attacker)
        {
            if(IsDead())
            {
                OnDeath?.Invoke(this, attacker);
                StartCoroutine(OnDeathRoutine());
            }
        }

        private IEnumerator OnDeathRoutine()
        {
            Coroutine coroutine = null;
            foreach (UnitAnimator unitAnimator in unitAnimators)
            {
                if(unitAnimator != null && unitAnimator.gameObject.activeSelf)
                {
                    if(coroutine != null)
                    {
                        coroutine = unitAnimator.StartCoroutine(unitAnimator.DeathAnimationRoutine(deathAnimationTime));
                        yield return coroutine;         
                    }
                    else
                    {
                        unitAnimator.StartCoroutine(unitAnimator.DeathAnimationRoutine(deathAnimationTime));
                    }
                }
                else
                {
                    yield return null;
                }          
            }
            Destroy(gameObject);
        }

        public int GetHealth()
        {
            return health;
        }
        public void SetHealth(int health)
        {
            this.health = health;
            OnHealthChanged?.Invoke();
        }

        public bool IsDead()
        {
            return health <= 0;
        }

        public float GetNormalizedHealth()
        {
            return (float)health / (float)unit.GetMaximumHealth();
        }

        public Unit GetUnit()
        {
            return unit;
        }
    }
}

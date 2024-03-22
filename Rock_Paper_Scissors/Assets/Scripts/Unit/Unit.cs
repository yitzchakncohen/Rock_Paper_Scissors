using System;
using System.Collections;
using System.Collections.Generic;
using RockPaperScissors.Grids;
using RockPaperScissors.SaveSystem;
using UnityEngine;

namespace RockPaperScissors.Units
{
    public class Unit : MonoBehaviour, ISaveInterface<SaveUnitData>
    {
        public static event EventHandler OnUnitSpawn;
        [SerializeField] private UnitAnimator unitAnimator;
        [SerializeField] private UnitShaderController unitShaderController;
        [SerializeField] private bool isFriendly = true;
        [SerializeField] private UnitData unitData;
        private UnitAction[] unitActions;
        private UnitProgression unitProgression;
        private UnitHealth health;

        private void Awake() 
        {
            unitActions = GetComponents<UnitAction>();
            health = GetComponent<UnitHealth>();
            unitProgression = GetComponent<UnitProgression>();
            if(unitAnimator != null)
            {
                if(unitData.spriteLibrary != null)
                {
                    unitAnimator.SetSpriteLibraryAsset(unitData.spriteLibrary);
                }
                else
                {
                    Debug.Log($"Unit {gameObject.name}, is missing a sprite library");
                }
            }
        }

        private void Start() 
        {
            ActionHandler.OnUnitSelected += ActionHandler_OnUnitSelected;
            OnUnitSpawn?.Invoke(this, EventArgs.Empty);
            unitShaderController.SetupSprite(unitData.unitThumbnail);
        }

        private void OnDestroy() 
        {
            ActionHandler.OnUnitSelected -= ActionHandler_OnUnitSelected;
        }

        private void ActionHandler_OnUnitSelected(object sender, Unit selectedUnit)
        {
            if(selectedUnit == this)
            {
                unitShaderController.SetOutlineOn();
            }
            else
            {
                unitShaderController.SetOutlineOff();
            }
        }

        public UnitAction[] GetUnitActions()
        {
            return unitActions;
        }

        public bool IsFriendly()
        {
            return isFriendly;
        }

        public bool IsMoveable()
        {
            return GetComponent<UnitMovement>();
        }

        public void Damage(int damageAmount, Unit attacker)
        {
            health.Damage(damageAmount, attacker);
        }

        public float GetNormalizedHealth() => health.GetNormalizedHealth();

        public int GetMaximumHealth()
        {
            return unitData.maximumHealth[unitProgression.GetLevel() - 1];
        }

        public int GetCost()
        {
            return unitData.unitCost;
        }

        public Sprite GetUnitThumbnail()
        {
            return unitData.unitThumbnail;
        }

        public int GetAttackRange()
        {
            return unitData.attackRange;
        }

        internal int GetMoveDistance()
        {
            return unitData.moveDistance;
        }

        public UnitClass GetUnitClass()
        {
            return unitData.unitClass;
        }

        public UnitProgression GetUnitProgression()
        {
            return unitProgression;
        }

        public int GetLevel()
        {
            if(unitProgression != null)
            {
                return unitProgression.GetLevel();
            }
            else
            {
                return -1;
            }
        }

        public int GetHealth()
        {
            return health.GetHealth();
        }

        public bool IsDead()
        {
            return health.IsDead();
        }

        public UnitAnimator GetUnitAnimator()
        {
            return unitAnimator;
        }

        public int GetBaseAttack()
        {
            return unitData.attackDamage[0];
        }

        public int GetBaseDefense()
        {
            return unitData.defense[0];
        }

        public int GetModifiedAttack()
        {
            return unitData.attackDamage[unitProgression.GetLevel() - 1];
        }

        public int GetModifiedDefense()
        {
            return unitData.defense[unitProgression.GetLevel() - 1];
        }

        public int GetUnitDefeatedReward()
        {
            return unitData.unitDefeatedReward;
        }

        public SaveUnitData Save()
        {
            
            SaveUnitData saveUnitData = new SaveUnitData
            {
                UnitClass = unitData.unitClass,
                UnitLevel = GetLevel(),
                UnitHealth = GetHealth(),
                UnitXP = unitProgression.GetXP(),
                IsFriendly = isFriendly,
                FacingDirection = unitAnimator.GetCurrentDirection()
            };

            foreach (UnitAction unitAction in GetUnitActions())
            {
                unitAction.SaveAction(saveUnitData);
            }

            return saveUnitData;
        }

        public void Load(SaveUnitData loadData)
        {
            unitProgression.SetLevel(loadData.UnitLevel);
            health.SetHealth(loadData.UnitHealth);
            unitProgression.SetXP(loadData.UnitXP);
            isFriendly = loadData.IsFriendly;
            foreach (UnitAction unitAction in GetUnitActions())
            {
                unitAction.LoadAction(loadData);
            }
            unitAnimator.SetFacingDirection(loadData.FacingDirection, unitProgression.GetLevel());
            StartCoroutine(UpdateAnimatorRoutine());
        }

        private IEnumerator UpdateAnimatorRoutine()
        {
            yield return new WaitForEndOfFrame();
            unitAnimator.UpdateSprite();
        }
    }
}

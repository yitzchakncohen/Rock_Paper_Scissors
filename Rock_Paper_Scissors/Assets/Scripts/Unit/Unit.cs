using System;
using System.Collections;
using System.Collections.Generic;
using RockPaperScissors.Grids;
using RockPaperScissors.SaveSystem;
using UnityEngine;

namespace RockPaperScissors.Units
{
    public class Unit : MonoBehaviour, ISaveInterface<SaveUnitData>, IGridOccupantInterface
    {
        public UnitAnimator UnitAnimator => unitAnimator;
        public ParticleSystem HitFX => unitData.HitFX;
        public Sprite UnitThumbnail => unitData.unitThumbnail;
        public UnitProgression UnitProgression => unitProgression;
        public UnitClass Class => unitData.unitClass;
        public UnitAction[] UnitActions => unitActions;
        public float NormalizedHealth => health.NormalizedHealth;
        public int Cost => unitData.unitCost;
        public int MoveDistance => unitData.moveDistance;
        public int AttackRange => unitData.attackRange;
        public int UnitDefeatedReward => unitData.unitDefeatedReward;
        public int Health => health.Health; 
        public int AttackDamage => unitData.attackDamage[unitProgression.Level - 1];
        public int Defense => unitData.defense[unitProgression.Level - 1];
        public bool IsDead => health.IsDead;
        public bool IsFriendly { get => isFriendly; set{} }
        public bool IsMoveable{get => UnitClass.Moveable.HasFlag(Class); set{}} 
        public bool IsBuilding{get => UnitClass.Building.HasFlag(Class); set{}} 
        public bool IsTrap{get => UnitClass.Trap.HasFlag(Class); set{}} 


        
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

        public void Damage(int damageAmount, Unit attacker)
        {
            health.Damage(damageAmount, attacker);
            ParticleSystem hitFX = Instantiate(HitFX, transform.position, Quaternion.identity);
            Destroy(hitFX.gameObject, 1f);
        }

        public void DestroyUnit()
        {
            health.Damage(health.Health, null);
        }

        public int GetMaximumHealth()
        {
            return unitData.maximumHealth[unitProgression.Level - 1];
        }     

        public int GetLevel()
        {
            if(unitProgression != null)
            {
                return unitProgression.Level;
            }
            else
            {
                return -1;
            }
        }        

        public int GetTotalActionPointsRemaining()
        {
            if(IsTrap)
            {
                return 0;
            }

            int actionPoints = 0;
            foreach (UnitAction unitAction in unitActions)
            {
                actionPoints += unitAction.ActionPointsRemaining;
            }
            return actionPoints;
        }

        public void SetTrappedTurnsRemaining(int trappedTurnsRemaining)
        {
            foreach (UnitAction action in unitActions)
            {
                action.SetTrappedTurnsRemaining(trappedTurnsRemaining);
            }
        }

        public SaveUnitData Save()
        {
            SaveUnitData saveUnitData = new SaveUnitData
            {
                UnitClass = unitData.unitClass,
                UnitLevel = GetLevel(),
                UnitHealth = Health,
                UnitXP = unitProgression.XP,
                IsFriendly = isFriendly,
                FacingDirection = unitAnimator.GetCurrentDirection()
            };

            foreach (UnitAction unitAction in UnitActions)
            {
                saveUnitData = unitAction.SaveAction(saveUnitData);
            }

            return saveUnitData;
        }

        public void Load(SaveUnitData loadData)
        {
            unitProgression.SetLevel(loadData.UnitLevel);
            health.SetHealth(loadData.UnitHealth);
            unitProgression.XP = loadData.UnitXP;
            isFriendly = loadData.IsFriendly;
            foreach (UnitAction unitAction in UnitActions)
            {
                unitAction.LoadAction(loadData);
            }
            unitAnimator.SetFacingDirection(loadData.FacingDirection, unitProgression.Level);
            StartCoroutine(UpdateAnimatorRoutine());
        }

        private IEnumerator UpdateAnimatorRoutine()
        {
            yield return new WaitForEndOfFrame();
            unitAnimator.UpdateSprite();
        }

        public bool CanWalkOnGridOccupant(IGridOccupantInterface gridOccupantInterface)
        {
            Unit gridOccupant = (Unit)gridOccupantInterface;
            if(gridOccupantInterface != null && IsFriendly == gridOccupant.IsFriendly)
            {
                // Which types of buildings can you walk over?
                if(gridOccupant.Class == UnitClass.PillowOutpost)
                {
                    return true;
                }
                if(gridOccupant.Class == UnitClass.TrampolineTrap)
                {
                    return true;
                }
                if(gridOccupant.Class == UnitClass.GlueTrap)
                {
                    return gridOccupant.isFriendly != IsFriendly;
                }
                // Can't walk over any units right now
                return false;
            }
            return true;
        }
    }
}

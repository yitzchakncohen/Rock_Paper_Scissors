using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RockPaperScissors.Grids;
using RockPaperScissors.SaveSystem;
using UnityEngine;

namespace RockPaperScissors.Units
{
    public class UnitAttack : UnitAction
    {
        [SerializeField] private UnitAnimator unitAnimator;
        [SerializeField] private AnimationCurve attackAnimationCurve;
        private float cameraSnapDelay = 0.2f;
        private Unit unit;
        private Unit target;
        private GridManager gridManager;
        private float timer;
        private float attackAnimationTime = 0.6f;
        private Vector3 attackStartPosition;
        private Vector3 attackTargetPosition;
        private bool attacking;
        private int unitAttackActionBaseValue = 100;
        private int classAdvantageMultiplier = 10;


        private void Awake() 
        {
            unit = GetComponent<Unit>();
        }

        protected override void Start() 
        {
            base.Start();
            IsCancellableAction = false;
            gridManager = FindObjectOfType<GridManager>();
        }

        private void Update() 
        {
            if(!attacking)
            {
                return;
            }

            AnimateAttack(attackTargetPosition - transform.position);

            // Check the attack timer to ensure the animation is complete, then complete the attack.
            timer += Time.deltaTime;
            if(timer >= attackAnimationTime)
            {
                StartCoroutine(CompleteAttack());
            }
        }

        private void StartAttack(Unit unitToAttack, Action onActionComplete)
        {
            timer = 0f;
            attackStartPosition = transform.position;
            attackTargetPosition = unitToAttack.transform.position;
            target = unitToAttack;
            attacking = true;
            ActionStart(onActionComplete);
        }

        private IEnumerator CompleteAttack()
        {
            GridObject gridObjectAttacking = gridManager.GetGridObjectFromWorldPosition(target.transform.position);
            bool isTargetInTower = gridObjectAttacking.GetOccupantBuilding() != null;
            int damageAmount = CombatModifiers.GetDamage(unit, target, isTargetInTower);
            target.Damage(damageAmount, unit);
            actionPointsRemaining -= 1;
            transform.position = attackStartPosition;
            attacking = false;

            // Wait to complete the action until the camera has snapped to the new location.
            yield return new WaitForSeconds(cameraSnapDelay);
            ActionComplete();
        }

        private void AnimateAttack(Vector2 attackDirection)
        {
            int level = unit.GetUnitProgression().GetLevel();
            float normalizedAnimationTime = timer/attackAnimationTime;
            // Animate the attack by moving the unit towards the unit it is attacking. 
            transform.position = attackStartPosition + (attackTargetPosition - attackStartPosition)*attackAnimationCurve.Evaluate(normalizedAnimationTime);

            // Update the sprite via the animator.
            if(attackDirection.x > 0 && attackDirection.y > 0)
            {
                unitAnimator.MoveUpRight(level);
            }
            else if(attackDirection.x > 0 && attackDirection.y < 0)
            {
                unitAnimator.MoveDownRight(level);
            }
            else if(attackDirection.x > 0 && attackDirection.y == 0)
            {
                unitAnimator.MoveRight(level);
            }
            if(attackDirection.x < 0 && attackDirection.y > 0)
            {
                unitAnimator.MoveUpLeft(level);
            }
            else if(attackDirection.x < 0 && attackDirection.y < 0)
            {
                unitAnimator.MoveDownLeft(level);
            }
            else if(attackDirection.x < 0 && attackDirection.y == 0)
            {
                unitAnimator.MoveLeft(level);
            }
        }

        public bool TryAttackUnit(Unit unitToAttack, Action onActionComplete)
        {
            // Check the action points
            if(actionPointsRemaining <= 0 || trappedTurnsRemaining > 0)
            {
                return false;
            }

            // Check if the unit to attack is a valid target of this unit.
            Vector2Int gridPosition = gridManager.GetGridPositionFromWorldPosition(unit.transform.position);
            if(GetValidTargets(gridPosition).Contains(unitToAttack))
            {
                StartAttack(unitToAttack, onActionComplete);
                // Debug.Log("Attacking: " + unitToAttack.gameObject.name);
                return true;
            }
            // Debug.Log("Failed to attack: " + unitToAttack);
            return false;
        }

        public List<Unit> GetValidTargets(Vector2Int gridPosition)
        {
            List<Unit> validTargetList = new List<Unit>();

            // Check all the targets in range.
            // Get list of grid positions in range.
            List<Vector2Int> gridPositionsInRangeList = new List<Vector2Int>();

            // Add the starting position
            gridPositionsInRangeList.Add(gridPosition);


            // Increment outward getting all the positions in range one layer at a time.
            for (int i = 0; i < unit.GetAttackRange(); i++)
            {
                // Check the valid neighbours of each position and add them to the list if they are new.
                List<Vector2Int> newPositions = new List<Vector2Int>();
                foreach (Vector2Int position in gridPositionsInRangeList)
                {
                    newPositions = newPositions.Union(gridManager.GetNeighbourList(position)).ToList();
                }

                // Add the new positions to the positions list.
                foreach (Vector2Int newPosition in newPositions)
                {
                    if(!gridPositionsInRangeList.Contains(newPosition))
                    {
                        gridPositionsInRangeList.Add(newPosition);
                    }
                }
            }

            // Check grid positions for valid targets
            foreach (Vector2Int position in gridPositionsInRangeList)
            {
                GridObject gridObject = gridManager.GetGridObject(position);
                if(gridObject.GetCombatTarget() != null 
                    && gridObject.GetCombatTarget().IsFriendly() != unit.IsFriendly()
                    && !((Unit)gridObject.GetCombatTarget()).IsDead())
                {
                    validTargetList.Add((Unit)gridObject.GetCombatTarget());
                }
            }

            return validTargetList;
        }

        public override EnemyAIAction GetBestEnemyAIAction()
        {
            EnemyAIAction bestAction = null;
            Vector2Int gridPosition = gridManager.GetGridPositionFromWorldPosition(unit.transform.position);

            // For each valid target find the value
            foreach (Unit unit in GetValidTargets(gridPosition))
            {
                GridObject gridObject = gridManager.GetGridObjectFromWorldPosition(unit.transform.position);
                // If this is the first action is the best action so far. 
                if(bestAction == null)
                {
                    bestAction = new EnemyAIAction()
                    {
                        gridObject = gridObject,
                        actionValue = unitAttackActionBaseValue 
                                        + (1 - unit.GetNormalizedHealth())*unitAttackActionBaseValue 
                                        + CombatModifiers.UnitHasAdvantage(this.unit.GetUnitClass(), unit.GetUnitClass())*classAdvantageMultiplier,
                        unitAction = this,
                    };
                }
                else
                {
                    // Compare the new action to th best action
                    EnemyAIAction testAction = new EnemyAIAction()
                    {
                        gridObject = gridObject,
                        actionValue = unitAttackActionBaseValue 
                                        + (1 - unit.GetNormalizedHealth())*unitAttackActionBaseValue
                                        + CombatModifiers.UnitHasAdvantage(this.unit.GetUnitClass(), unit.GetUnitClass())*classAdvantageMultiplier,
                        unitAction = this,
                    }; 

                    // Check if this action is better.
                    if(testAction.actionValue > bestAction.actionValue)
                    {
                        bestAction = testAction;
                    }
                }
            }

            return bestAction;
        }

        public override int GetValidActionsRemaining()
        {
            // if there are action points remaining AND valid actions to take return the number of action points remaining.
            Vector2Int gridPosition = gridManager.GetGridPositionFromWorldPosition(unit.transform.position);
            if(GetValidTargets(gridPosition).Count > 0)
            {
                return actionPointsRemaining;
            }
            else
            {
                return 0;
            }
        }

        public override bool TryTakeAction(GridObject gridObject, Action onActionComplete)
        {
            return TryAttackUnit((Unit)gridObject.GetCombatTarget(), onActionComplete);
        }

        public Unit GetTarget()
        {
            return target;
        }

        public Unit GetUnit()
        {
            return unit;
        }

        protected override void CancelButton_OnCancelButtonPress()
        {
            base.CancelButton_OnCancelButtonPress();
        }

        public override void LoadAction(SaveUnitData loadData)
        {
            actionPointsRemaining = loadData.AttackActionPointsRemaining;
        }

        public override SaveUnitData SaveAction(SaveUnitData saveData)
        {
            saveData.AttackActionPointsRemaining = actionPointsRemaining;
            return saveData;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using RockPaperScissors.Grids;
using UnityEngine;

namespace RockPaperScissors.Units
{
    public class UnitAttack : UnitAction
    {
        [SerializeField] private UnitAnimator unitAnimator;
        [SerializeField] private AnimationCurve attackAnimationCurve;
        private Unit unit;
        private Unit target;
        private GridManager gridManager;
        private float timer;
        private float attackAnimationTime = 0.6f;
        private Vector3 attackStartPosition;
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

            AnimateAttack(target.transform.position - transform.position);

            // Check the attack timer to ensure the animation is complete, then compelte the attack.
            timer += Time.deltaTime;
            if(timer >= attackAnimationTime)
            {
                CompleteAttack();
            }
        }

        private void StartAttack(Unit unitToAttack, Action onActionComplete)
        {
            timer = 0f;
            attackStartPosition = transform.position;
            target = unitToAttack;
            attacking = true;
            ActionStart(onActionComplete);
        }

        private void CompleteAttack()
        {
            int damageAmount = CombatModifiers.GetDamage(unit, target);
            target.Damage(damageAmount, unit);
            actionPointsRemaining -= 1;
            attacking = false;
            ActionComplete();
        }

        // Get a list of grid positions that neighbour the current position
        private List<Vector2Int> GetNeighbourList(Vector2Int currentPosition)
        {
            List<Vector2Int> neighbourList = new List<Vector2Int>();

            bool oddRow = currentPosition.y % 2 == 1;

            if(currentPosition.x - 1 >= 0)
            {
                // Left
                neighbourList.Add(new Vector2Int(currentPosition.x -1, currentPosition.y +0));

            }

            if(currentPosition.x + 1 < gridManager.GetGridSize().x)
            {
                // Right
                neighbourList.Add(new Vector2Int(currentPosition.x +1, currentPosition.y +0));
            }

            if(currentPosition.y -1 >= 0)
            {
                // Down (left and right)
                neighbourList.Add(new Vector2Int(currentPosition.x +0, currentPosition.y -1));
                if(currentPosition.x - 1 >= 0 && currentPosition.x + 1 < gridManager.GetGridSize().x)
                {
                    neighbourList.Add(new Vector2Int(currentPosition.x + (oddRow ? +1 : -1), currentPosition.y -1));                
                }
            }

            if(currentPosition.y + 1 < gridManager.GetGridSize().y)
            {
                // Up (left and right)
                neighbourList.Add(new Vector2Int(currentPosition.x + 0, currentPosition.y +1));
                if(currentPosition.x - 1 >= 0 && currentPosition.x + 1 < gridManager.GetGridSize().x)
                {
                    neighbourList.Add(new Vector2Int(currentPosition.x + (oddRow ? +1 : -1), currentPosition.y +1));
                }
            }

            return neighbourList;
        }

        private void AnimateAttack(Vector2 attackDirection)
        {
            int level = unit.GetUnitProgression().GetLevel();
            float normalizedAnimationTime = timer/attackAnimationTime;
            // Animate the attack by moving the unit towards the unit it is attacking. 
            transform.position = attackStartPosition + (target.transform.position - attackStartPosition)*attackAnimationCurve.Evaluate(normalizedAnimationTime);

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
            if(actionPointsRemaining <= 0)
            {
                return false;
            }

            // Check if the unit to attack is a valid target of this unit.
            Vector2Int gridPosition = gridManager.GetGridPositionFromWorldPosition(unit.transform.position);
            if(GetValidTargets(gridPosition).Contains(unitToAttack))
            {
                StartAttack(unitToAttack, onActionComplete);
                return true;
            }
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
                    foreach (Vector2Int neighbourPosition in GetNeighbourList(position))
                    {
                        if(!newPositions.Contains(neighbourPosition))
                        {
                            newPositions.Add(neighbourPosition);
                        }
                    }
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
                if(gridObject.GetOccupent() != null && gridObject.GetOccupent().IsFriendly() != unit.IsFriendly())
                {
                    validTargetList.Add(gridObject.GetOccupent());
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
            return TryAttackUnit(gridObject.GetOccupent(), onActionComplete);
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
    }
}

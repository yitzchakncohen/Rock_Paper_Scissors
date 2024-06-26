using System;
using System.Collections;
using System.Collections.Generic;
using RockPaperScissors.Grids;
using RockPaperScissors.PathFindings;
using RockPaperScissors.SaveSystem;
using UnityEngine;

namespace RockPaperScissors.Units
{
    public class UnitMovement : UnitAction
    {
        public static event EventHandler OnUnitMove;
        [SerializeField] private UnitAnimator unitAnimator;
        [SerializeField] private float movementSpeed = 5f;
        [SerializeField] private float stoppingDistance = 0.1f;
        private float cameraSnapDelay = 0.2f;
        private GridManager gridManager;
        private UnitManager unitManager;
        private PathFinding pathFinding;
        private UnitAttack unitAttack;
        private List<GridObject> targetGridObjects = null;
        private int currentPositionIndex = 0;
        private bool moving = false;

        protected override void Awake() 
        {
            base.Awake();
            unitAttack = GetComponent<UnitAttack>();
        }
        
        protected override void Start() 
        {
            base.Start();
            IsCancellableAction = false;
            gridManager = FindObjectOfType<GridManager>();
            pathFinding = FindObjectOfType<PathFinding>();
            unitManager = FindObjectOfType<UnitManager>();
        }

        private void Update() 
        {
            if(moving)
            {
                Move();
            }
        }

        private void Move()
        {
            if (targetGridObjects != null && currentPositionIndex < targetGridObjects.Count)
            {
                if (Vector2.Distance(transform.position, targetGridObjects[currentPositionIndex].transform.position) < stoppingDistance)
                {
                    // Reach position
                    transform.position = targetGridObjects[currentPositionIndex].transform.position;
                    // Play sound every other position.
                    if(currentPositionIndex % 2 == 0)
                    {
                        AudioManager.Instance.PlayUnitMovementSound();
                    }

                    // Check for Trap
                    if(CheckGridObjectForTrap(targetGridObjects[currentPositionIndex]))
                    {
                        StartCoroutine(EndMove());
                        return;
                    }

                    // Increment move target
                    currentPositionIndex++;
                }
                else
                {
                    Vector2 moveDirection = (targetGridObjects[currentPositionIndex].transform.position - transform.position).normalized;
                    AnimateMovement(moveDirection);
                    if((moveDirection * movementSpeed * Time.deltaTime).magnitude > Vector2.Distance(targetGridObjects[currentPositionIndex].transform.position, transform.position))
                    {
                        transform.position = targetGridObjects[currentPositionIndex].transform.position;
                    }
                    else
                    {
                        transform.position += (Vector3)(moveDirection * movementSpeed * Time.deltaTime);
                    }
                    OnUnitMove?.Invoke(this, EventArgs.Empty);
                }
            }
            else
            {
                StartCoroutine(EndMove());
            }
        }

        private bool CheckGridObjectForTrap(GridObject gridObject)
        {
            Unit unit = gridObject.OccupantTrap as Unit;
            if(unit == null)
            {
                return false;
            }

            UnitTrap unitTrap = unit.GetComponent<UnitTrap>();
            if(unitTrap != null)
            {   
                if(unit.IsFriendly != Unit.IsFriendly && !unitTrap.IsTrapSprung)
                {
                    return true;
                }
                else
                {
                    // Friendly trap doesn't stop movement.
                }
            }
            return false;
        }

        private IEnumerator EndMove()
        {
            unitAnimator.ToggleMoveAnimation(false);
            moving = false;
            actionPointsRemaining -= 1;
            
            // Wait to complete the action until the camera has snapped to the new location.
            yield return new WaitForSeconds(cameraSnapDelay);
            ActionComplete();

            // TODO remove this debug statement. 
            gridManager.ResetActionValueTexts();
        }

        public bool TryStartMove(GridObject targetGridObject, Action onActionComplete)
        {
            if(actionPointsRemaining <= 0 || trappedTurnsRemaining > 0) 
            {
                return false;
            }

            Vector2Int currentGridPosition = gridManager.GetGridPositionFromWorldPosition(transform.position);
            targetGridObjects = pathFinding.FindPath(currentGridPosition, targetGridObject.Position, out int pathLength, unit);

            // Check if position is within movement range and moveable
            if(pathLength > unit.MoveDistance || targetGridObjects == null)
            {
                return false;
            }

            // Start Move
            currentPositionIndex = 0;
            moving = true;
            ActionStart(onActionComplete);
            return true;
        }

        private void AnimateMovement(Vector2 moveDirection)
        {
            int level = unit.UnitProgression.Level;
            if(moveDirection.x > 0 && moveDirection.y > 0)
            {
                unitAnimator.MoveUpRight(level);
            }
            else if(moveDirection.x > 0 && moveDirection.y < 0)
            {
                unitAnimator.MoveDownRight(level);
            }
            else if(moveDirection.x > 0 && moveDirection.y == 0)
            {
                unitAnimator.MoveRight(level);
            }
            if(moveDirection.x < 0 && moveDirection.y > 0)
            {
                unitAnimator.MoveUpLeft(level);
            }
            else if(moveDirection.x < 0 && moveDirection.y < 0)
            {
                unitAnimator.MoveDownLeft(level);
            }
            else if(moveDirection.x < 0 && moveDirection.y == 0)
            {
                unitAnimator.MoveLeft(level);
            }
            unitAnimator.ToggleMoveAnimation(true);
        }

        public List<Vector2Int> GetValidMovementPositions()
        {
            List<Vector2Int> gridPositionList = new List<Vector2Int>();
            Vector2Int gridPosition = gridManager.GetGridPositionFromWorldPosition(transform.position);
            if(unit == null)
            {
                unit = GetComponent<Unit>();
            }

            for (int x = -unit.MoveDistance; x <= unit.MoveDistance; x++)
            {
                for (int z = -unit.MoveDistance; z <= unit.MoveDistance; z++)
                {
                    Vector2Int testGridPosition = gridPosition + new Vector2Int(x, z);

                    // Check if it's on the grid.
                    if (!gridManager.IsValidGridPosition(testGridPosition))
                    {
                        continue;
                    }

                    // Check if it's walkable
                    if (!gridManager.GetGridObject(testGridPosition).IsWalkable(unit))
                    {
                        continue;
                    }

                    // Check if it's within movement distance
                    pathFinding.FindPath(gridPosition, testGridPosition, out int testDistance, unit);
                    if (testDistance > unit.MoveDistance)
                    {
                        continue;
                    }

                    gridPositionList.Add(testGridPosition);
                }
            }

            return gridPositionList;
        }

        public override EnemyAIAction GetBestEnemyAIAction()
        {
            EnemyAIAction bestAction = null;
            List<Vector2Int> validMovePositions = GetValidMovementPositions();

            foreach (Vector2Int position in validMovePositions)
            {
                GridObject gridObject = gridManager.GetGridObject(position);
                List<Unit> targetList = unitAttack.GetValidTargets(position);

                // Find the average health of the units nearby.
                float healthAmountValue = GetAverageNormalizedHealth(targetList);
                float targetCountValue = GetValueFromTargetList(targetList);

                if (bestAction == null)
                {
                    bestAction = new EnemyAIAction()
                    {
                        gridObject = gridObject,
                        actionValue = targetCountValue + healthAmountValue,
                        unitAction = this,
                    };
                }
                else
                {
                    EnemyAIAction testAction = new EnemyAIAction()
                    {
                        gridObject = gridObject,
                        actionValue = targetCountValue + healthAmountValue,
                        unitAction = this,
                    };

                    // Check if this action is better.
                    if (testAction.actionValue > bestAction.actionValue)
                    {
                        bestAction = testAction;
                    }
                }

                gridManager.GetGridObject(position).SetActionValue(targetCountValue + healthAmountValue);
            }
            
            // If there are no units in range of any of the movement spaces, the best action value will still be 0.
            // Instead move towards the closes enemy by setting a value from 1 to 10;
            // float startTime = Time.realtimeSinceStartup;
            if(bestAction.actionValue == 0)
            {
                Vector2Int gridPosition = gridManager.GetGridPositionFromWorldPosition(transform.position);
                Unit closestUnit  = unitManager.GetClosestFriendlyUnitToPosition(gridPosition, out float distance);
                Vector2Int closestUnitPosition;
                if(closestUnit != null)
                {
                    closestUnitPosition = gridManager.GetGridPositionFromWorldPosition(closestUnit.transform.position);
                }
                else
                {
                    return bestAction;
                }
                
                foreach (Vector2Int position in validMovePositions)
                {
                    distance = gridManager.GetGridDistanceBetweenPositions(closestUnitPosition, position);

                    if(1 + 9f/distance > bestAction.actionValue)
                    {
                        GridObject gridObject = gridManager.GetGridObject(position);

                        bestAction = new EnemyAIAction()
                        {
                            gridObject = gridObject,
                            actionValue = 1 + 9f/distance,
                            unitAction = this,
                        };
                    }
                }
            }

            // Debug.Log("Get Closest Friendly: " + (Time.realtimeSinceStartup - startTime) * 1000f);
            return bestAction;
        }

        private float GetValueFromTargetList(List<Unit> targetList)
        {
            float targetCountValue = 0;
            if (targetList.Count > 0)
            {
                // 60 is the number possible adjacent hexes times 10. 
                targetCountValue = 60f / targetList.Count;
            }

            foreach (Unit unit in targetList)
            {
                // Add or substract 1 if there is a combat advantage over the unit.
                targetCountValue += CombatModifiers.UnitHasAdvantage(this.unit.Class, unit.Class);
            }

            return targetCountValue;
        }

        private static float GetAverageNormalizedHealth(List<Unit> targetList)
        {
            float healthAmountValue = 0;
            if(targetList.Count > 0)
            {
                float averageNormalizedHealth = 0f;
                foreach (Unit unit in targetList)
                {
                    averageNormalizedHealth += unit.NormalizedHealth;
                }
                averageNormalizedHealth = averageNormalizedHealth / targetList.Count;
                // The health value is a number between 0 and 10;
                healthAmountValue = 10 * (1 - averageNormalizedHealth);
            }

            return healthAmountValue;
        }

        private Vector2Int GetCurrentGridPosition()
        {
            return gridManager.GetGridPositionFromWorldPosition(transform.position);
        }

        public override int GetValidActionsRemaining()
        {
            if( trappedTurnsRemaining > 0)
            {
                return 0;
            }
            else if(GetValidMovementPositions().Count > 0)
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
            return TryStartMove(gridObject, onActionComplete);
        }

        protected override void CancelButton_OnCancelButtonPress()
        {
            base.CancelButton_OnCancelButtonPress();
        }

        public override void LoadAction(SaveUnitData loadData)
        {
            actionPointsRemaining = loadData.MoveActionPointsRemaining;
            trappedTurnsRemaining = loadData.TrappedTurnsRemaining;
        }

        public override SaveUnitData SaveAction(SaveUnitData saveData)
        {
            saveData.MoveActionPointsRemaining = actionPointsRemaining;
            saveData.TrappedTurnsRemaining = trappedTurnsRemaining;
            return saveData;
        }
    }
}

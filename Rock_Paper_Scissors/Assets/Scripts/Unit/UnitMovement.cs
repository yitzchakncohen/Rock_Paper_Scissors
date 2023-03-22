using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitMovement : UnitAction
{
    [SerializeField] private UnitAnimator unitAnimator;
    [SerializeField] private int moveDistance = 5;
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float stoppingDistance = 0.1f;
    private GridManager gridManager;
    private UnitManager unitManager;
    private PathFinding pathFinding;
    private UnitAttack unitAttack;
    private List<GridObject> targetGridObjects = null;
    private int currentPositionIndex = 0;
    private bool moving;

    private void Awake() 
    {
        unitAttack = GetComponent<UnitAttack>();
    }
    
    private void Start() 
    {
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
                transform.position = targetGridObjects[currentPositionIndex].transform.position;
                currentPositionIndex++;
            }
            else
            {
                Vector2 moveDirection = (targetGridObjects[currentPositionIndex].transform.position - transform.position).normalized;
                AnimateMovement(moveDirection);
                transform.position += (Vector3)(moveDirection * movementSpeed * Time.deltaTime);
            }
        }
        else
        {
            moving = false;
            actionPointsRemaining -= 1;
            ActionComplete();
        }
    }

    public bool TryStartMove(GridObject targetGridObject, Action onActionComplete)
    {
        if(actionPointsRemaining <= 0)
        {
            return false;
        }

        Vector2Int currentGridPosition = gridManager.GetGridPositionFromWorldPosition(transform.position);
        targetGridObjects = pathFinding.FindPath(currentGridPosition, targetGridObject.GetGridPostion(), out int pathLength);
        if(pathLength > moveDistance)
        {
            return false;
        }
        currentPositionIndex = 0;
        moving = true;
        ActionStart(onActionComplete);
        return true;
    }

    public int GetMoveDistance()
    {
        return moveDistance;
    }

    public bool IsMoving()
    {
        return moving;
    }

    private void AnimateMovement(Vector2 moveDirection)
    {
        if(moveDirection.x > 0 && moveDirection.y > 0)
        {
            unitAnimator.MoveUpRight();
        }
        else if(moveDirection.x > 0 && moveDirection.y < 0)
        {
            unitAnimator.MoveDownRight();
        }
        else if(moveDirection.x > 0 && moveDirection.y == 0)
        {
            unitAnimator.MoveRight();
        }
        if(moveDirection.x < 0 && moveDirection.y > 0)
        {
            unitAnimator.MoveUpLeft();
        }
        else if(moveDirection.x < 0 && moveDirection.y < 0)
        {
            unitAnimator.MoveDownLeft();
        }
        else if(moveDirection.x < 0 && moveDirection.y == 0)
        {
            unitAnimator.MoveLeft();
        }
    }

    public List<Vector2Int> GetValidMovementPositions(Vector2Int gridPosition, int range)
    {
        List<Vector2Int> gridPositionList = new List<Vector2Int>();
        for (int x = -range; x <= range; x++)
        {
            for (int z = -range; z <= range; z++)
            {
                Vector2Int testGridPosition = gridPosition + new Vector2Int(x, z);

                // Check if it's on the grid.
                if (!gridManager.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                // Check if it's within movement distance
                pathFinding.FindPath(gridPosition, testGridPosition, out int testDistance);
                if (testDistance > range)
                {
                    continue;
                }

                // Check if it's walkable
                if (!gridManager.GetGridObject(testGridPosition).IsWalkable())
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
        List<Vector2Int> validMovePositions = GetValidMovementPositions(gridManager.GetGridPositionFromWorldPosition(transform.position), moveDistance);

        foreach (Vector2Int position in validMovePositions)
        {
            GridObject gridObject = gridManager.GetGridObject(position);
            List<Unit> targetList = unitAttack.GetValidTargets(position);
            int targetCountAtPosition = targetList.Count;

            // Find the average health of the units nearby.
            float healthAmountValue = GetAverageNormalizedHealth(targetList);
            float targetCountValue = GetValueByTargetCount(targetCountAtPosition);

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
        if(bestAction.actionValue == 0)
        {
            foreach (Vector2Int position in validMovePositions)
            {
                GridObject gridObject = gridManager.GetGridObject(position);

                unitManager.GetClosestFriendlyUnitToPosition(position, out float distance);
                if(1 + 9f/distance > bestAction.actionValue)
                {
                    bestAction = new EnemyAIAction()
                    {
                        gridObject = gridObject,
                        actionValue = 1 + 9f/distance,
                        unitAction = this,
                    };
                }

                gridManager.GetGridObject(position).SetActionValue(1 + 9f/distance);
            }
        }

        return bestAction;
    }

    private static float GetValueByTargetCount(int targetCountAtPosition)
    {
        float targetCountValue = 0;
        if (targetCountAtPosition > 0)
        {
            // 60 is the number possible adjacent hexes times 10. 
            targetCountValue = 60f / targetCountAtPosition;
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
                averageNormalizedHealth += unit.GetComponent<Health>().GetNormalizedHealth();
            }
            averageNormalizedHealth = averageNormalizedHealth / targetList.Count;
            // The health value is a number between 0 and 10;
            healthAmountValue = 10 * (1 - averageNormalizedHealth);
        }

        return healthAmountValue;
    }

    public override bool TryTakeAction(GridObject gridObject, Action onActionComplete)
    {
        return TryStartMove(gridObject, onActionComplete);
    }
}

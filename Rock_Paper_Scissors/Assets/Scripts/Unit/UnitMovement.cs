using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitMovement : UnitAction
{
    public static event EventHandler OnUnitMove;
    [SerializeField] private UnitAnimator unitAnimator;
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float stoppingDistance = 0.1f;
    private GridManager gridManager;
    private UnitManager unitManager;
    private PathFinding pathFinding;
    private UnitAttack unitAttack;
    private Unit unit;
    private List<GridObject> targetGridObjects = null;
    private int currentPositionIndex = 0;
    private bool moving;

    private void Awake() 
    {
        unitAttack = GetComponent<UnitAttack>();
        unit = GetComponent<Unit>();
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
                transform.position = targetGridObjects[currentPositionIndex].transform.position;
                currentPositionIndex++;
            }
            else
            {
                Vector2 moveDirection = (targetGridObjects[currentPositionIndex].transform.position - transform.position).normalized;
                AnimateMovement(moveDirection);
                transform.position += (Vector3)(moveDirection * movementSpeed * Time.deltaTime);
                OnUnitMove?.Invoke(this, EventArgs.Empty);
            }
        }
        else
        {
            unitAnimator.ToggleMoveAnimation(false);
            moving = false;
            actionPointsRemaining -= 1;
            ActionComplete();
            // TODO remove this debug statement. 
            gridManager.ResetActionValueTexts();
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
        if(pathLength > unit.GetMoveDistance())
        {
            return false;
        }
        currentPositionIndex = 0;
        moving = true;
        ActionStart(onActionComplete);
        return true;
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
        unitAnimator.ToggleMoveAnimation(true);
    }

    public List<Vector2Int> GetValidMovementPositions()
    {
        List<Vector2Int> gridPositionList = new List<Vector2Int>();
        Vector2Int gridPosition = gridManager.GetGridPositionFromWorldPosition(transform.position);

        for (int x = -unit.GetMoveDistance(); x <= unit.GetMoveDistance(); x++)
        {
            for (int z = -unit.GetMoveDistance(); z <= unit.GetMoveDistance(); z++)
            {
                Vector2Int testGridPosition = gridPosition + new Vector2Int(x, z);

                // Check if it's on the grid.
                if (!gridManager.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                // Check if it's within movement distance
                pathFinding.FindPath(gridPosition, testGridPosition, out int testDistance);
                if (testDistance > unit.GetMoveDistance())
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
            targetCountValue += CombatModifiers.UnitHasAdvantage(this.unit.GetUnitClass(), unit.GetUnitClass());
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
                averageNormalizedHealth += unit.GetNormalizedHealth();
            }
            averageNormalizedHealth = averageNormalizedHealth / targetList.Count;
            // The health value is a number between 0 and 10;
            healthAmountValue = 10 * (1 - averageNormalizedHealth);
        }

        return healthAmountValue;
    }

    public override int GetValidActionsRemaining()
    {
        if(GetValidMovementPositions().Count > 0)
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
}

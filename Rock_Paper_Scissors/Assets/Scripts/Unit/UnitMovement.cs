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
    private Grid grid;
    private GridManager gridManager;
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
        grid = FindObjectOfType<Grid>();
        gridManager = FindObjectOfType<GridManager>();
        pathFinding = FindObjectOfType<PathFinding>();
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
        this.onActionComplete = onActionComplete;

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

        foreach (Vector2Int position in GetValidMovementPositions(gridManager.GetGridPositionFromWorldPosition(transform.position), moveDistance))
        {
            GridObject gridObject = gridManager.GetGridObject(position);
            int targetCountAtPosition = unitAttack.GetValidTargets(position).Count;

            if(bestAction == null)
            {
                bestAction = new EnemyAIAction()
                {
                    gridObject = gridObject,
                    actionValue = targetCountAtPosition * 10,
                };
            }
            else
            {
                EnemyAIAction testAction = new EnemyAIAction()
                {
                    gridObject = gridObject,
                    actionValue = targetCountAtPosition * 10,
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

    public override bool TryTakeAction(GridObject gridObject, Action onActionComplete)
    {
        return TryStartMove(gridObject, onActionComplete);
    }
}

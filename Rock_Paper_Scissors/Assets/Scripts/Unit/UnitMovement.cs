using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitMovement : MonoBehaviour
{
    public static event EventHandler OnMovementCompleted;
    [SerializeField] private UnitAnimator unitAnimator;
    [SerializeField] private int moveDistance = 5;
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float stoppingDistance = 0.1f;
    private Grid grid;
    private GridManager gridManager;
    private PathFinding pathfinding;
    private List<GridObject> targetGridObjects = null;
    private int movementPointsRemaining = 1;
    private int currentPositionIndex = 0;
    private bool moving;
    
    private void Start() 
    {
        grid = FindObjectOfType<Grid>();
        gridManager = FindObjectOfType<GridManager>();
        pathfinding = FindObjectOfType<PathFinding>();
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
            movementPointsRemaining -= 1;
            OnMovementCompleted?.Invoke(this, EventArgs.Empty);
        }
    }

    public bool TryStartMove(GridObject targetGridObject)
    {
        if(movementPointsRemaining <= 0)
        {
            return false;
        }

        Vector2Int currentGridPosition = gridManager.GetGridPositionFromWorldPosition(transform.position);
        targetGridObjects = pathfinding.FindPath(currentGridPosition, targetGridObject.GetGridPostion(), out int pathLength);
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

    public int GetMovementPointsRemaining()
    {
        return movementPointsRemaining;
    }

    public void ResetMovementPoints()
    {
        movementPointsRemaining = 1;
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
}

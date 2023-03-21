using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionHandler : MonoBehaviour
{
    public static event EventHandler<Unit> OnUnitSelected;
    public static EventHandler<bool> OnBusyChanged; 
    [SerializeField] private Unit selectedUnit;
    private InputManager inputManager;
    private GridManager gridManager;
    private GridUIManager gridUIManager;
    private PathFinding pathFinding;
    private TurnManager turnManager;
    private bool isBusy = false;

    private void Start() 
    {
        inputManager = FindObjectOfType<InputManager>();
        gridManager = FindObjectOfType<GridManager>();
        gridUIManager = FindObjectOfType<GridUIManager>();
        inputManager = FindObjectOfType<InputManager>();
        pathFinding = FindObjectOfType<PathFinding>();
        turnManager = FindObjectOfType<TurnManager>();

        inputManager.OnSingleTap += InputManager_onSingleTouch;
        TurnManager.OnNextTurn += TurnManager_OnNextTurn;
    }

    private void OnDestroy() 
    {
        inputManager.OnSingleTap -= InputManager_onSingleTouch;
    }

    private void InputManager_onSingleTouch(object sender, Vector2 touchPosition)
    {
        if (!turnManager.IsPlayersTurn())
        {
            return;
        }

        if(isBusy)
        {
            return;
        }

        GridObject targetGridObject = CheckForObjectAtTouchPosition(touchPosition);
        HandleGridObjectTouch(targetGridObject);
    }

    private GridObject CheckForObjectAtTouchPosition(Vector2 touchPosition)
    {
        Vector3 worldPositionOfInput = Camera.main.ScreenToWorldPoint(touchPosition);
        GridObject targetGridObject = gridManager.GetGridObjectFromWorldPosition(worldPositionOfInput);
        return targetGridObject;
    }

    private void HandleGridObjectTouch(GridObject gridObject)
    {
        // Check if the grid position is occupied. 
        if(gridObject.GetOccupent() != null)
        {
            // If you have a unit selected, check the cell for an action.
            if(selectedUnit != gridObject.GetOccupent())
            {
                if(gridObject.GetOccupent().IsFriendly())
                {
                    //For friendly
                    selectedUnit = gridObject.GetOccupent();
                    OnUnitSelected?.Invoke(this, selectedUnit);
                    HighlightActionGrid();
                }
                else
                {
                    // For Enemy
                    if(selectedUnit != null)
                    {
                        UnitAttack unitAttacking = selectedUnit.GetUnitAttacking();
                        if(unitAttacking.TryAttackUnit(gridObject.GetOccupent(), ClearBusy))
                        {
                            SetBusy();
                        }
                    }
                }
            }
        }
        else if(selectedUnit != null)
        {
            // If the grid position is not occupied, move the selected unit there
            UnitMovement unitMovement = selectedUnit.GetComponent<UnitMovement>();
            if(!unitMovement.IsMoving())
            {
                if(unitMovement.TryStartMove(gridObject, ClearBusy))
                {
                    SetBusy();
                }
            }
        }
    }

    private void HighlightActionGrid()
    {
        gridUIManager.HideAllGridPosition();

        // Not the player's turn
        if(!turnManager.IsPlayersTurn())
        {
            return;
        }

        // No selected unit
        if(selectedUnit == null)
        {
            return;
        }

        Vector2Int unitGridPosition = gridManager.GetGridPositionFromWorldPosition(selectedUnit.transform.position);

        // Check unit movement
        UnitMovement unitMovement = selectedUnit.GetUnitMovement();
        if(unitMovement.GetMovementPointsRemaining() > 0)
        {
            HighlightMovePositionRange(unitGridPosition, unitMovement.GetMoveDistance());
        }

        // Check unit attacking
        UnitAttack unitAttacking = selectedUnit.GetUnitAttacking();
        if(unitAttacking.GetAttackPointsRemaining() > 0)
        {
            HighlightAttackTargets();
        }
    }

    private void HighlightMovePositionRange(Vector2Int gridPosition, int range)
    {
        List<Vector2Int> gridPositionList = new List<Vector2Int>();
        for (int x = -range; x <= range; x++)
        {
            for (int z = -range; z <= range; z++)
            {
                Vector2Int testGridPosition = gridPosition + new Vector2Int(x,z);
                
                // Check if it's on the grid.
                if(!gridManager.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                // Check if it's within movement distance
                pathFinding.FindPath(gridPosition, testGridPosition, out int testDistance);
                if(testDistance > range)
                {
                    continue;
                }

                // Check if it's walkable
                if(!gridManager.GetGridObject(testGridPosition).IsWalkable())
                {
                    continue;
                }

                gridPositionList.Add(testGridPosition);
            }
        }

        gridUIManager.ShowGridPositionList(gridPositionList, GridHighlightType.Movement);
    }

    private void HighlightAttackTargets()
    {
        UnitAttack unitAttacking = selectedUnit.GetUnitAttacking();
        List<Unit> validUnitTargets = unitAttacking.GetValidTargets();

        List<Vector2Int> validAttackPositions = new List<Vector2Int>();
        foreach (Unit unit in validUnitTargets)
        {
            validAttackPositions.Add(gridManager.GetGridPositionFromWorldPosition(unit.transform.position));
        }

        gridUIManager.ShowGridPositionList(validAttackPositions, GridHighlightType.Attack);
    }

    private void TurnManager_OnNextTurn(object sender, EventArgs e)
    {
        HighlightActionGrid();
        selectedUnit = null;
        OnUnitSelected?.Invoke(this, selectedUnit);
    }

    private void SetBusy()
    {
        isBusy = true;
        OnBusyChanged?.Invoke(this, isBusy);
    }

    private void ClearBusy()
    {
        isBusy = false;
        OnBusyChanged?.Invoke(this, isBusy);
        HighlightActionGrid();
    }
}

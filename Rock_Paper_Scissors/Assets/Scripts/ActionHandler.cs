using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionHandler : MonoBehaviour
{
    public static event EventHandler<Unit> OnUnitSelected;
    [SerializeField] private Unit selectedUnit;
    private InputManager inputManager;
    private GridManager gridManager;
    private GridUIManager gridUIManager;
    private PathFinding pathFinding;

    private void Start() 
    {
        inputManager = FindObjectOfType<InputManager>();
        gridManager = FindObjectOfType<GridManager>();
        gridUIManager = FindObjectOfType<GridUIManager>();
        inputManager = FindObjectOfType<InputManager>();
        pathFinding = FindObjectOfType<PathFinding>();

        inputManager.onSingleTouch += InputManager_onSingleTouch;
        UnitMovement.OnMovementCompleted += UnitMovement_OnMovementCompleted;
        UnitAttacking.OnAttackingCompleted += UnitAttacking_OnAttackingCompleted;
    }

    private void OnDestroy() 
    {
        inputManager.onSingleTouch -= InputManager_onSingleTouch;
        UnitMovement.OnMovementCompleted -= UnitMovement_OnMovementCompleted;
        UnitAttacking.OnAttackingCompleted -= UnitAttacking_OnAttackingCompleted;
    }

    private void InputManager_onSingleTouch(object sender, Vector2 touchPosition)
    {
        Vector3 worldPositionOfInput = Camera.main.ScreenToWorldPoint(touchPosition);
        GridObject targetGridObject = gridManager.GetGridObjectFromWorldPosition(worldPositionOfInput);
        HandleGridObjectTouch(targetGridObject);
    }

    private void HandleGridObjectTouch(GridObject gridObject)
    {
        // Check if the grid position is occupied. 
        if(gridObject.GetOccupent() != null)
        {
            // If you have a unit selected, check the cell for an action.
            if(selectedUnit != gridObject.GetOccupent())
            {
                //For friendly
                if(gridObject.GetOccupent().IsFriendly())
                {
                        selectedUnit = gridObject.GetOccupent();
                        OnUnitSelected?.Invoke(this, selectedUnit);
                        HighlightActionGrid();
                }
                else
                {
                    // For Enemy
                    if(selectedUnit != null)
                    {
                        UnitAttacking unitAttacking = selectedUnit.GetUnitAttacking();
                        unitAttacking.TryAttackUnit(gridObject.GetOccupent());
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
                unitMovement.TryStartMove(gridObject);
            }
        }
    }

    private void HighlightActionGrid()
    {
        Vector2Int unitGridPosition = gridManager.GetGridPositionFromWorldPosition(selectedUnit.transform.position);

        // Check unit movement
        UnitMovement unitMovement = selectedUnit.GetUnitMovement();
        gridUIManager.HideAllGridPosition();
        if(unitMovement.GetMovementPointsRemaining() > 0)
        {
            HighlightMovePositionRange(unitGridPosition, unitMovement.GetMoveDistance());
        }

        // Check unit attacking
        UnitAttacking unitAttacking = selectedUnit.GetUnitAttacking();
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
        UnitAttacking unitAttacking = selectedUnit.GetUnitAttacking();
        List<Unit> validUnitTargets = unitAttacking.GetValidTargets();

        List<Vector2Int> validAttackPositions = new List<Vector2Int>();
        foreach (Unit unit in validUnitTargets)
        {
            validAttackPositions.Add(gridManager.GetGridPositionFromWorldPosition(unit.transform.position));
        }

        gridUIManager.ShowGridPositionList(validAttackPositions, GridHighlightType.Attack);
    }

    private void UnitMovement_OnMovementCompleted(object sender, EventArgs e)
    {
        HighlightActionGrid();
    }

    private void UnitAttacking_OnAttackingCompleted(object sender, EventArgs e)
    {
        HighlightActionGrid();
    }
}

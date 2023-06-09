using System;
using System.Collections;
using System.Collections.Generic;
using RockPaperScissors.Grids;
using RockPaperScissors.PathFindings;
using RockPaperScissors.UI;
using RockPaperScissors.Units;
using UnityEngine;

public class ActionHandler : MonoBehaviour
{
    public static event EventHandler<Unit> OnUnitSelected;
    public static EventHandler<bool> BusyUpdated; 
    [SerializeField] private Unit selectedBuilding;
    private InputManager inputManager;
    private GridManager gridManager;
    private GridUI gridUIManager;
    private PathFinding pathFinding;
    private TurnManager turnManager;
    private UnitManager unitManager;
    private Queue<Unit> unitQueue = new Queue<Unit>();
    private bool isBusy = false;
    private bool updateGridActionHighlight = false;

    private void Start() 
    {
        inputManager = FindObjectOfType<InputManager>();
        gridManager = FindObjectOfType<GridManager>();
        gridUIManager = FindObjectOfType<GridUI>();
        inputManager = FindObjectOfType<InputManager>();
        pathFinding = FindObjectOfType<PathFinding>();
        turnManager = FindObjectOfType<TurnManager>();
        unitManager = FindObjectOfType<UnitManager>();

        inputManager.OnSingleTap += InputManager_onSingleTouch;
        TurnManager.OnNextTurn += TurnManager_OnNextTurn;
        BuildingButton.OnBuildingButtonPressed += BuildingButton_BuildingButtonPressed;
        UnitHealth.OnDeath += Health_OnDeath;
        Unit.OnUnitSpawn += Unit_OnUnitSpawn;

        ResetUnitQueue();
    }

    private void LateUpdate() 
    {
        if(updateGridActionHighlight)
        {
            UpdateGridActionHighlight();
            updateGridActionHighlight = false;
        }
    }

    private void OnDestroy() 
    {
        inputManager.OnSingleTap -= InputManager_onSingleTouch;
        TurnManager.OnNextTurn -= TurnManager_OnNextTurn;
        BuildingButton.OnBuildingButtonPressed -= BuildingButton_BuildingButtonPressed;
        UnitHealth.OnDeath -= Health_OnDeath;
        Unit.OnUnitSpawn -= Unit_OnUnitSpawn;
    }

    private void InputManager_onSingleTouch(object sender, Vector2 touchPosition)
    {
        if (!turnManager.IsPlayerTurn())
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
        Unit gridOccupentUnit = gridObject.GetOccupentUnit();
        Unit gridOccupentBuilding = gridObject.GetOccupentBuilding(); 
        
        // Check if the grid position is occupied by a Unit
        if(gridOccupentUnit != null)
        {
            // Select a friendly unit
            if(gridOccupentUnit.IsFriendly())
            {
                SelectUnitOccupyingGridPosition(gridObject);
            }
            else
            {
                TryAttackUnitOccupyingGridPosition(gridObject);
            }
            return;
        }
        
        // Check if the grid position is occupied by a Tower
        if(gridOccupentBuilding != null)
        {
            // If it is not friendly, attack the tower.
            if(!gridOccupentBuilding.IsFriendly())
            {
                TryAttackUnitOccupyingGridPosition(gridObject);
            }
            else
            {
                SelectBuildingOccupyingGridPosition(gridObject);
            }
            return;
        } 
        
        if(selectedBuilding != null)
        {
            TryMoveToGridPosition(gridObject);
        }
    }

    private void TryMoveToGridPosition(GridObject gridObject)
    {
        // If the grid position is not occupied, move the selected unit there
        if (selectedBuilding.TryGetComponent<UnitMovement>(out UnitMovement unitMovement))
        {
            if (unitMovement.TryStartMove(gridObject, ClearBusy))
            {
                SetBusy();
            }
        }
    }

    private void TryAttackUnitOccupyingGridPosition(GridObject gridObject)
    {
        // Attack an enemy unit
        if (selectedBuilding != null)
        {
            if (selectedBuilding.TryGetComponent<UnitAttack>(out UnitAttack unitAttack))
            {
                if (unitAttack.TryAttackUnit(gridObject.GetCombatTarget(), ClearBusy))
                {
                    SetBusy();
                }
            }
        }
    }

    private void SelectUnitOccupyingGridPosition(GridObject gridObject)
    {
        selectedBuilding = gridObject.GetOccupentUnit();
        OnUnitSelected?.Invoke(this, selectedBuilding);
        updateGridActionHighlight = true;
    }

    private void SelectBuildingOccupyingGridPosition(GridObject gridObject)
    {
        selectedBuilding = gridObject.GetOccupentBuilding();
        OnUnitSelected?.Invoke(this, selectedBuilding);
        updateGridActionHighlight = true;
    }

    private void UpdateGridActionHighlight()
    {
        gridUIManager.HideAllGridPosition();

        // Not the player's turn
        if(!turnManager.IsPlayerTurn())
        {
            return;
        }

        // No selected unit
        if(selectedBuilding == null)
        {
            return;
        }

        foreach (UnitAction unitAction in selectedBuilding.GetUnitActions())
        {
            // Only show if there are action points
            if(unitAction.GetActionPointsRemaining() <= 0)
            {
                continue;
            }

            if(unitAction is UnitMovement)
            {
                HighlightMovePositionRange(unitAction as UnitMovement);
            }
            else if(unitAction is UnitAttack)
            {
                HighlightAttackTargets(unitAction as UnitAttack);
            }
        }
    }

    private void HighlightMovePositionRange(UnitMovement unitMovement)
    {
        List<Vector2Int> gridPositionList = unitMovement.GetValidMovementPositions();

        gridUIManager.ShowGridPositionList(gridPositionList, GridHighlightType.Movement);
    }

    private void HighlightAttackTargets(UnitAttack unitAttacking)
    {
        Vector2Int gridPosition = gridManager.GetGridPositionFromWorldPosition(selectedBuilding.transform.position);
        List<Unit> validUnitTargets = unitAttacking.GetValidTargets(gridPosition);

        List<Vector2Int> validAttackPositions = new List<Vector2Int>();
        foreach (Unit unit in validUnitTargets)
        {
            validAttackPositions.Add(gridManager.GetGridPositionFromWorldPosition(unit.transform.position));
        }

        gridUIManager.ShowGridPositionList(validAttackPositions, GridHighlightType.Attack);
    }

    private void HighlightPlacementTargets(UnitSpawner unitSpawner)
    {
        List<Vector2Int> validPlacementPositions = unitSpawner.GetValidPlacementPositions();
        gridUIManager.ShowGridPositionList(validPlacementPositions, GridHighlightType.PlaceObject);
    }

    private void BuildingButton_BuildingButtonPressed(object sender, BuildButtonArguments arguments)
    {
        HighlightPlacementTargets(arguments.unitSpawner);
        GridObject gridObject = gridManager.GetGridObjectFromWorldPosition(arguments.unitSpawner.transform.position);
        SetBusy();
        if(arguments.unitSpawner.TryTakeAction(gridObject , ClearBusy))
        {
            selectedBuilding = null;
            OnUnitSelected?.Invoke(this, selectedBuilding);
        }        
    }

    private void TurnManager_OnNextTurn(object sender, EventArgs eventArgs)
    {
        updateGridActionHighlight = true;
        selectedBuilding = null;
        ResetUnitQueue();
        OnUnitSelected?.Invoke(this, selectedBuilding);
    }

    private void ResetUnitQueue()
    {
        unitQueue.Clear();
        foreach (Unit unit in unitManager.GetFriendlyUnitsList())
        {
            unitQueue.Enqueue(unit);
        }
    }

    private void Health_OnDeath(object sender, Unit e)
    {
        updateGridActionHighlight = true;
    }

    private void Unit_OnUnitSpawn(object sender, EventArgs e)
    {
        Unit unit = sender as Unit;
        if(unit != null && unit.IsFriendly())
        {
            unitQueue.Enqueue(unit);
        }
    }

    private void SetBusy()
    {
        isBusy = true;
        BusyUpdated?.Invoke(this, isBusy);
    }

    private void ClearBusy()
    {
        isBusy = false;
        BusyUpdated?.Invoke(this, isBusy);
        updateGridActionHighlight = true;
    }

    public void SelectNextAvaliableUnit()
    {
        Unit nextAvaliableUnit = null;
        nextAvaliableUnit = FindNextAvaliableUnit(nextAvaliableUnit);
        for (int i = 0; i < unitQueue.Count; i++)
        {
            if(unitQueue.Peek() == nextAvaliableUnit)
            {
                unitQueue.Enqueue(unitQueue.Dequeue());  
                break;
            }
            else
            {
                unitQueue.Enqueue(unitQueue.Dequeue());            
            }
        }
    }

    private Unit FindNextAvaliableUnit(Unit nextAvaliableUnit)
    {
        foreach (Unit unit in unitQueue)
        {
            foreach (UnitAction unitAction in unit.GetUnitActions())
            {
                if (unitAction.GetValidActionsRemaining() > 0)
                {
                    selectedBuilding = unit;
                    OnUnitSelected?.Invoke(this, unit);
                    updateGridActionHighlight = true;
                    return unit;
                }
            }
        }
        return null;
    }
}

using System;
using System.Collections.Generic;
using RockPaperScissors;
using RockPaperScissors.Grids;
using RockPaperScissors.UI;
using RockPaperScissors.Units;
using UnityEngine;

public class ActionHandler : MonoBehaviour
{
    public static event EventHandler<Unit> OnUnitSelected;
    public static EventHandler<bool> BusyUpdated; 
    [SerializeField] private Unit selectedUnit;
    private InputManager inputManager;
    private GridManager gridManager;
    private GridUI gridUIManager;
    private UnitManager unitManager;
    private Queue<Unit> unitQueue = new Queue<Unit>();
    private bool isBusy = false;
    private bool updateGridActionHighlight = false;
    private bool controlsLocked = false;

    private void Start() 
    {
        inputManager = FindObjectOfType<InputManager>();
        gridManager = FindObjectOfType<GridManager>();
        gridUIManager = FindObjectOfType<GridUI>();
        inputManager = FindObjectOfType<InputManager>();
        unitManager = FindObjectOfType<UnitManager>();

        inputManager.OnSingleTap += InputManager_onSingleTouch;
        TurnManager.OnNextTurn += TurnManager_OnNextTurn;
        BuildingButton.OnBuildingButtonPressed += BuildingButton_BuildingButtonPressed;
        BuildingMenu.OnGarrisonedUnitSelected += BuildingMenu_OnGarrisonedUnitSelected;
        UnitHealth.OnDeath += Health_OnDeath;
        Unit.OnUnitSpawn += Unit_OnUnitSpawn;
        GameplayManager.OnGameOver += GameplayManager_OnGameOver;
        WaveManager.OnWaveStarted += WaveManager_OnWaveStarted;
        WaveManager.OnWaveCompleted += WaveManager_OnWaveCompleted;
        UnitAction.OnAnyActionStarted += UnitAction_OnAnyActionStarted;

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
        BuildingMenu.OnGarrisonedUnitSelected -= BuildingMenu_OnGarrisonedUnitSelected;
        UnitHealth.OnDeath -= Health_OnDeath;
        Unit.OnUnitSpawn -= Unit_OnUnitSpawn;
        GameplayManager.OnGameOver -= GameplayManager_OnGameOver;
        WaveManager.OnWaveStarted -= WaveManager_OnWaveStarted;
        WaveManager.OnWaveCompleted -= WaveManager_OnWaveCompleted;
        UnitAction.OnAnyActionStarted -= UnitAction_OnAnyActionStarted;
    }

    private void InputManager_onSingleTouch(object sender, Vector2 touchPosition)
    {
        // Not the player's turn or game over
        if(controlsLocked)
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
        IGridOccupantInterface gridOccupantUnit = gridObject.GetOccupantUnit();
        IGridOccupantInterface gridOccupantBuilding = gridObject.GetOccupantBuilding(); 

        // Try and attack 
        if(gridOccupantUnit != null && !gridOccupantUnit.IsFriendly())
        {
            TryAttackUnitOccupyingGridPosition(gridObject);
            return;
        }
        else if(gridOccupantBuilding != null && !gridOccupantBuilding.IsFriendly())
        {
            // Attack the building
            return;
        }
        // Try to move / select
        else if(gridOccupantBuilding != null && gridOccupantBuilding.IsFriendly())
        {
            // Check if you can move the unit onto the tower.
            if(selectedUnit != null && selectedUnit.IsMoveable() && gridOccupantUnit == null)
            {
                if(!TryMoveToGridPosition(gridObject))
                {
                    SelectBuildingOccupyingGridPosition(gridObject);
                }
            }
            else
            {
                SelectBuildingOccupyingGridPosition(gridObject);
            }
            return;
        }
        else if(gridOccupantUnit != null && gridOccupantUnit.IsFriendly())
        {
            SelectUnitOccupyingGridPosition(gridObject);
            return;
        }
        
        if(selectedUnit != null)
        {
            if(TryMoveToGridPosition(gridObject))
            {
               // Moved Succesfuly 
            }
            else
            {
                // Tapping on a space with no options will deselect the unit. 
                DeselectUnit();
                updateGridActionHighlight = true;
            }
        }
    }

    private bool TryMoveToGridPosition(GridObject gridObject)
    {
        // If the grid position is not occupied, move the selected unit there
        if (selectedUnit.TryGetComponent<UnitMovement>(out UnitMovement unitMovement))
        {
            if (unitMovement.TryStartMove(gridObject, ClearBusy))
            {
                SetBusy();
                return true;
            }
        }
        return false;
    }

    private void TryAttackUnitOccupyingGridPosition(GridObject gridObject)
    {
        // Attack an enemy unit
        if (selectedUnit != null)
        {
            if (selectedUnit.TryGetComponent<UnitAttack>(out UnitAttack unitAttack))
            {
                if (unitAttack.TryAttackUnit((Unit)gridObject.GetCombatTarget(), ClearBusy))
                {
                    SetBusy();
                }
            }
        }
    }

    private void SelectUnitOccupyingGridPosition(GridObject gridObject)
    {
        selectedUnit = (Unit)gridObject.GetOccupantUnit();
        OnUnitSelected?.Invoke(this, selectedUnit);
        updateGridActionHighlight = true;
    }

    private void SelectBuildingOccupyingGridPosition(GridObject gridObject)
    {
        selectedUnit = (Unit)gridObject.GetOccupantBuilding();
        if(gridObject.GetOccupantUnit() != null)
        {
            BuildingMenu buildingMenu = selectedUnit.GetComponentInChildren<BuildingMenu>();
            buildingMenu.SetGarrisonedUnit(gridObject.GetOccupantUnit() as Unit);
        }
        OnUnitSelected?.Invoke(this, selectedUnit);
        updateGridActionHighlight = true;
    }

    private void UpdateGridActionHighlight()
    {
        gridUIManager.HideAllGridPosition();

        // Not the player's turn or game over
        if(controlsLocked)
        {
            return;
        }

        // No selected unit
        if(selectedUnit == null)
        {
            return;
        }

        foreach (UnitAction unitAction in selectedUnit.GetUnitActions())
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
        Vector2Int gridPosition = gridManager.GetGridPositionFromWorldPosition(selectedUnit.transform.position);
        List<Unit> validUnitTargets = unitAttacking.GetValidTargets(gridPosition);

        List<Vector2Int> validAttackPositions = new List<Vector2Int>();
        foreach (Unit unit in validUnitTargets)
        {
            validAttackPositions.Add(gridManager.GetGridPositionFromWorldPosition(unit.transform.position));
        }

        gridUIManager.ShowGridPositionList(validAttackPositions, GridHighlightType.Attack);
    }

    private void HighlightPlacementTargets(UnitSpawner unitSpawner, Unit unitToSpawn)
    {
        List<Vector2Int> validPlacementPositions = unitSpawner.GetValidPlacementPositions(unitToSpawn);
        gridUIManager.ShowGridPositionList(validPlacementPositions, GridHighlightType.PlaceObject);
    }

    private void DeselectUnit()
    {
        selectedUnit = null;
        OnUnitSelected?.Invoke(this, selectedUnit);
    }

    private void BuildingButton_BuildingButtonPressed(object sender, BuildButtonArguments arguments)
    {
        HighlightPlacementTargets(arguments.unitSpawner, arguments.unit);
        GridObject gridObject = gridManager.GetGridObjectFromWorldPosition(arguments.unitSpawner.transform.position);
        SetBusy();
        if(arguments.unitSpawner.TryTakeAction(gridObject , ClearBusy))
        {
            selectedUnit = null;
            OnUnitSelected?.Invoke(this, selectedUnit);
        }        
    }

    private void BuildingMenu_OnGarrisonedUnitSelected(Unit unit)
    {
        selectedUnit = unit;
        OnUnitSelected?.Invoke(this, selectedUnit);
        updateGridActionHighlight = true;
    }

    private void TurnManager_OnNextTurn(object sender, TurnManager.OnNextTurnEventArgs eventArgs)
    {
        updateGridActionHighlight = true;
        DeselectUnit();
        ResetUnitQueue();
        if(eventArgs.IsPlayersTurn)
        {
            controlsLocked = false;
        }
        else
        {
            controlsLocked = true;
        }
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

    private void GameplayManager_OnGameOver(int score)
    {
        controlsLocked = true;
        DeselectUnit();

    }
    private void WaveManager_OnWaveStarted()
    {
        controlsLocked = true;
        DeselectUnit();

    }

    private void WaveManager_OnWaveCompleted()
    {
        controlsLocked = false;
    }

    private void UnitAction_OnAnyActionStarted(object sender, EventArgs e)
    {
        UnitTrap unitTrap = sender as UnitTrap;
        if(unitTrap != null)
        {
            SetBusy();
            unitTrap.SetActionCompletedAction(ClearBusy);
        }
    }

    private void SetBusy()
    {
        isBusy = true;
        BusyUpdated?.Invoke(this, isBusy);
    }

    public bool IsBusy()
    {
        return isBusy;
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
                    selectedUnit = unit;
                    OnUnitSelected?.Invoke(this, unit);
                    updateGridActionHighlight = true;
                    return unit;
                }
            }
        }
        return null;
    }
}

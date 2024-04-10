using System;
using System.Collections.Generic;
using RockPaperScissors;
using RockPaperScissors.Grids;
using RockPaperScissors.UI.Buttons;
using RockPaperScissors.UI.Menus;
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
    private TurnManager turnManager;
    private bool isBusy = false;
    private bool updateGridActionHighlight = false;
    private bool controlsLocked = false;

    void Awake()
    {
        TurnManager.OnNextTurn += TurnManager_OnNextTurn;
        BuildingButton.OnBuildingButtonPressed += BuildingButton_BuildingButtonPressed;
        BuildingMenu.OnGarrisonedUnitSelected += BuildingMenu_OnGarrisonedUnitSelected;
        UnitHealth.OnDeath += Health_OnDeath;
        Unit.OnUnitSpawn += Unit_OnUnitSpawn;
        GameplayManager.OnGameOver += GameplayManager_OnGameOver;
        WaveManager.OnWaveStarted += WaveManager_OnWaveStarted;
        WaveManager.OnWaveCompleted += WaveManager_OnWaveCompleted;
        UnitAction.OnAnyActionStarted += UnitAction_OnAnyActionStarted;
        UnitAction.OnAnyActionCompleted += UnitAction_OnAnyActionCompleted;
    }

    private void Start() 
    {
        inputManager = FindObjectOfType<InputManager>();
        gridManager = FindObjectOfType<GridManager>();
        gridUIManager = FindObjectOfType<GridUI>();
        inputManager = FindObjectOfType<InputManager>();
        unitManager = FindObjectOfType<UnitManager>();
        turnManager = FindObjectOfType<TurnManager>();

        inputManager.OnSingleTap += InputManager_onSingleTouch;

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
        UnitAction.OnAnyActionCompleted -= UnitAction_OnAnyActionCompleted;
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
        IGridOccupantInterface gridOccupantTrap = gridObject.GetOccupantTrap();

        // Selected unit on a trampoline
        if(selectedUnit != null)
        {
            GridObject selectedUnitOccupiedGridObject = gridManager.GetGridObjectFromWorldPosition(selectedUnit.transform.position);
            Unit trap = selectedUnitOccupiedGridObject.GetOccupantTrap() as Unit;
            if(trap != null && trap.TryGetComponent<TrampolineTrap>(out TrampolineTrap trampolineTrap))
            {
                if(trampolineTrap.GetLaunchLocations(selectedUnit, selectedUnitOccupiedGridObject).Contains(gridObject.Position))
                {
                    if(trampolineTrap.TryTakeAction(gridObject, () => TrampolineTrapActionComplete(selectedUnit)))
                    {
                        return;
                    }
                }
            }
        }

        // Try and attack 
        if(gridOccupantUnit != null && !gridOccupantUnit.IsFriendly)
        {
            TryAttackUnitOccupyingGridPosition(gridObject);
            return;
        }
        else if(gridOccupantBuilding != null && !gridOccupantBuilding.IsFriendly)
        {
            // Attack the building
            return;
        }
        // Try to move / select
        else if(gridOccupantBuilding != null && gridOccupantBuilding.IsFriendly)
        {
            // Check if you can move the unit onto the tower.
            if(selectedUnit != null && selectedUnit.IsMoveable && gridOccupantUnit == null)
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
        else if(gridOccupantUnit != null && gridOccupantUnit.IsFriendly)
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
        AudioManager.Instance.PlayUnitSelectionSound();
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
        AudioManager.Instance.PlayUnitSelectionSound();
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

        // Unit on TrampolineTrap
        GridObject selectedUnitOccupiedGridObject = gridManager.GetGridObjectFromWorldPosition(selectedUnit.transform.position);
        if(selectedUnitOccupiedGridObject.GetOccupantTrap() != null)
        {
            if((selectedUnitOccupiedGridObject.GetOccupantTrap() as Unit).TryGetComponent<TrampolineTrap>(out TrampolineTrap trampolineTrap))
            {
                gridUIManager.ShowGridPositionList(trampolineTrap.GetLaunchLocations(selectedUnit, selectedUnitOccupiedGridObject), GridHighlightType.PlaceObject);
            }
        }


        foreach (UnitAction unitAction in selectedUnit.UnitActions)
        {
            // Only show if there are action points
            if(unitAction.ActionPointsRemaining <= 0)
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
                if(unitAction.ActionPointsRemaining > 0)
                {
                    HighlightAttackRange();
                }
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

    private void HighlightAttackRange()
    {
        Vector2Int gridPosition = gridManager.GetGridPositionFromWorldPosition(selectedUnit.transform.position);
        for (int x = 0; x < gridManager.GridSize.x; x++)
        {
            for (int z = 0; z < gridManager.GridSize.y; z++)
            {
                Vector2Int testGridPosition = new Vector2Int(x, z);
                GridObject gridObject = gridManager.GetGridObject(testGridPosition);
                int distance = gridManager.GetGridDistanceBetweenPositions(testGridPosition, gridPosition);
                if(distance == selectedUnit.AttackRange)
                {
                    int dx = testGridPosition.x - gridPosition.x;
                    int dy = testGridPosition.y - gridPosition.y;
                    int dxAbs = Mathf.Abs(dx);
                    int dyAbs = Mathf.Abs(dy);

                    // Same Row
                    if(testGridPosition.y == gridPosition.y)
                    {
                        if(testGridPosition.x < gridPosition.x)
                        {
                                gridObject.EnableAttackRangeIndicator(Direction.AllWest);
                                continue;
                        }
                        else if(testGridPosition.x > gridPosition.x)
                        {

                                gridObject.EnableAttackRangeIndicator(Direction.AllEast);
                                continue;
                        }
                    }

                    // Top and Bottom Rows
                    if(distance == dyAbs)
                    {
                        // Corners
                        if(((gridPosition.y % 2 == 1 ^ dx < 0) && (distance == (dxAbs - (dyAbs+1) / 2 + dyAbs)))
                        || (distance == (dxAbs - (dyAbs) / 2 + dyAbs) && (gridPosition.y % 2 == 1) && testGridPosition.x <= gridPosition.x)
                        || (distance == (dxAbs - (dyAbs) / 2 + dyAbs) && (gridPosition.y % 2 == 0) && testGridPosition.x >= gridPosition.x))
                        {
                            // North
                            if(testGridPosition.y > gridPosition.y)
                            {
                                if(testGridPosition.x > gridPosition.x)
                                {
                                    gridObject.EnableAttackRangeIndicator(Direction.NorthAndEast);
                                }
                                else if(testGridPosition.x == gridPosition.x && (gridPosition.y % 2 == 0))
                                {
                                    gridObject.EnableAttackRangeIndicator(Direction.NorthAndEast);
                                }
                                else if(testGridPosition.x == gridPosition.x && (gridPosition.y % 2 == 1))
                                {
                                    gridObject.EnableAttackRangeIndicator(Direction.NorthAndWest);
                                }
                                else
                                {
                                    gridObject.EnableAttackRangeIndicator(Direction.NorthAndWest);
                                }
                            }
                            else
                            { 
                                // South
                                if(testGridPosition.x > gridPosition.x)
                                {
                                    gridObject.EnableAttackRangeIndicator(Direction.SouthAndEast);
                                }
                                else if(testGridPosition.x == gridPosition.x && (gridPosition.y % 2 == 0))
                                {
                                    gridObject.EnableAttackRangeIndicator(Direction.SouthAndEast);
                                }
                                else if(testGridPosition.x == gridPosition.x && (gridPosition.y % 2 == 1))
                                {
                                    gridObject.EnableAttackRangeIndicator(Direction.SouthAndWest);
                                }
                                else
                                {
                                    gridObject.EnableAttackRangeIndicator(Direction.SouthAndWest);
                                }
                            }
                            continue;
                        }
                        else if(testGridPosition.y > gridPosition.y)
                        {
                            gridObject.EnableAttackRangeIndicator(Direction.AllNorth);
                        }
                        else
                        {
                            gridObject.EnableAttackRangeIndicator(Direction.AllSouth);
                        }
                        continue;
                    }

                    // Special Cases
                    // Even rows XAND to the left
                    if(gridPosition.x % 2 == 0 ^ testGridPosition.x < gridPosition.x)
                    {
                        if(distance == (dxAbs - (dyAbs + 1) / 2 + dyAbs)  || distance == (dxAbs - (dyAbs - 1) / 2 + dyAbs))
                        {
                            if(testGridPosition.y > gridPosition.y)
                            {
                                if(testGridPosition.x > gridPosition.x)
                                {
                                    gridObject.EnableAttackRangeIndicator(Direction.EastNorthEast);
                                }
                                else
                                {
                                    gridObject.EnableAttackRangeIndicator(Direction.WestNorthWest);
                                }
                            }
                            else
                            {
                                if(testGridPosition.x > gridPosition.x)
                                {
                                    gridObject.EnableAttackRangeIndicator(Direction.EastSouthEast);
                                }
                                else
                                {
                                    gridObject.EnableAttackRangeIndicator(Direction.WestSouthWest);
                                }
                            }
                            continue;
                        }
                    }
                    // Odd Rows
                    else
                    {
                        if(distance == (dxAbs - dyAbs / 2 + dyAbs) || distance == (dxAbs - (dyAbs - 1) / 2 + dyAbs) || distance == (dxAbs - (dyAbs + 1) / 2 + dyAbs))
                        {
                            if(testGridPosition.y > gridPosition.y)
                            {
                                if(testGridPosition.x > gridPosition.x)
                                {
                                    gridObject.EnableAttackRangeIndicator(Direction.EastNorthEast);
                                }
                                else
                                {
                                    gridObject.EnableAttackRangeIndicator(Direction.WestNorthWest);
                                }
                            }
                            else
                            {
                                if(testGridPosition.x > gridPosition.x)
                                {
                                    gridObject.EnableAttackRangeIndicator(Direction.EastSouthEast);
                                }
                                else
                                {
                                    gridObject.EnableAttackRangeIndicator(Direction.WestSouthWest);
                                }
                            }
                            continue;
                        }
                    }
                }
            }
        }
    }
    

    private void HighlightPlacementTargets(UnitSpawner unitSpawner, Unit unitToSpawn)
    {
        List<Vector2Int> validPlacementPositions = unitSpawner.GetValidPlacementPositions(unitToSpawn);
        gridUIManager.ShowGridPositionList(validPlacementPositions, GridHighlightType.PlaceObject);
    }

    private void HighlightUnitActionsAvailable()
    {
        gridManager.UpdateActionHighlights(null);
        if(turnManager.IsPlayerTurn)
        {
            List<Unit> units = unitManager.GetFriendlyUnitsList();
            gridManager.UpdateActionHighlights(units);
        }
    }

    private void DeselectUnit()
    {
        selectedUnit = null;
        OnUnitSelected?.Invoke(this, selectedUnit);
        AudioManager.Instance.PlayUnitDeselectionSound();
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
        AudioManager.Instance.PlayUnitSelectionSound();
        updateGridActionHighlight = true;
    }

    private void TurnManager_OnNextTurn(object sender, TurnManager.OnNextTurnEventArgs eventArgs)
    {
        updateGridActionHighlight = true;
        HighlightUnitActionsAvailable();
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
        HighlightUnitActionsAvailable();
    }

    private void Unit_OnUnitSpawn(object sender, EventArgs e)
    {
        Unit unit = sender as Unit;
        if(unit != null && unit.IsFriendly)
        {
            unitQueue.Enqueue(unit);
        }
        HighlightUnitActionsAvailable();
    }

    private void GameplayManager_OnGameOver(object sender, EventArgs e)
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
        // Exclude the case of the enemy unit by checking if a unit is selected.
        if(unitTrap != null && selectedUnit == null)
        {
            SetBusy();
            unitTrap.SetActionCompletedAction(ClearBusy);
        }
    }

    private void UnitAction_OnAnyActionCompleted(object sender, EventArgs e)
    {
        // Check for unit landed on friendly trampoline.
        UnitMovement movingUnit = sender as UnitMovement;
        if(movingUnit != null)
        {
            GridObject movingUnitGridObject = gridManager.GetGridObjectFromWorldPosition(movingUnit.transform.position);
            TrampolineTrap trampolineTrap = movingUnitGridObject.GetOccupantTrap() as TrampolineTrap;
            if(trampolineTrap != null && trampolineTrap.Unit.IsFriendly == movingUnit.Unit.IsFriendly)
            {
                selectedUnit = movingUnit.Unit;
                updateGridActionHighlight = true;
            }
        }

    }

    private void TrampolineTrapActionComplete(Unit launchedUnit)
    {
        selectedUnit = launchedUnit;
        OnUnitSelected?.Invoke(this, selectedUnit);
        updateGridActionHighlight = true;
        ClearBusy();
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
        HighlightUnitActionsAvailable();
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
            foreach (UnitAction unitAction in unit.UnitActions)
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

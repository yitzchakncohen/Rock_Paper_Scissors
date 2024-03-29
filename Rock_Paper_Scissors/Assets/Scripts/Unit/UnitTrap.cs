using System;
using System.Collections;
using System.Collections.Generic;
using RockPaperScissors.Grids;
using RockPaperScissors.SaveSystem;
using RockPaperScissors.Units;
using UnityEditor.VersionControl;
using UnityEngine;

public class UnitTrap : UnitAction
{
    private GridManager gridManager;
    private Unit unit;
    private bool IsTrapSprung = false;

    private void Awake() 
    {
        unit = GetComponent<Unit>();
    }

    protected override void Start()
    {
        base.Start();
        gridManager = FindObjectOfType<GridManager>();
    }

    private void OnEnable() 
    {
        UnitMovement.OnAnyActionCompleted += UnitMovement_OnAnyActionCompleted;
    }

    public override EnemyAIAction GetBestEnemyAIAction()
    {
        return null;
    }

    public override int GetValidActionsRemaining()
    {
        return 0; 
    }

    public override void LoadAction(SaveUnitData loadData)
    {
    }

    public override void SaveAction(SaveUnitData saveData)
    {
    }

    public override bool TryTakeAction(GridObject gridObject, Action onActionComplete)
    {
        Debug.Log("It's a trap!");
        return true;
    }

    public Unit GetUnit()
    {
        return unit;
    }
    
    public bool GetIsTrapSprung()
    {
        return IsTrapSprung;
    }

    private void UnitMovement_OnAnyActionCompleted(object sender, EventArgs e)
    {
        // Check if the unit that is moving is on the grid location of this trap.
        UnitMovement movingUnit = sender as UnitMovement;
        if(movingUnit == null)
        {
            return;
        }

        GridObject movingUnitGridObject = gridManager.GetGridObjectFromWorldPosition(movingUnit.transform.position);
        GridObject trapGridObject = gridManager.GetGridObjectFromWorldPosition(transform.position);
        if(movingUnitGridObject == trapGridObject)
        {
            TryTakeAction(trapGridObject, ActionComplete);
        }
    }
}

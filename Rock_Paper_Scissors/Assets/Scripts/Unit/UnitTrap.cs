using System;
using System.Collections;
using System.Collections.Generic;
using RockPaperScissors.Grids;
using RockPaperScissors.SaveSystem;
using RockPaperScissors.Units;
using UnityEngine;

public class UnitTrap : UnitAction
{
    private bool TrapSprung = false;

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
        Debug.Log("Spring the trap!");
        return true;
    }
}

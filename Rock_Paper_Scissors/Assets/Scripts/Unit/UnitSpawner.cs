using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSpawner : UnitAction
{
    [SerializeField] private BuildingMenu buildingMenu;
    
    public override EnemyAIAction GetBestEnemyAIAction()
    {
        throw new NotImplementedException();
    }

    public override bool TryTakeAction(GridObject gridObject, Action onActionComplete)
    {
        throw new NotImplementedException();
    }
}

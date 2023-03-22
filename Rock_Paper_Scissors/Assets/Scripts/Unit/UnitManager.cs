using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    private GridManager gridManager;
    private List<Unit> friendlyUnits;
    private List<Unit> enemyUnits;

    private void Awake() 
    {
        Health.OnDeath += Health_OnDeath;
        friendlyUnits = new List<Unit>();
        enemyUnits = new List<Unit>();
        gridManager = FindObjectOfType<GridManager>();
    }

    private void Start() 
    {
        foreach (Unit unit in FindObjectsOfType<Unit>())
        {
            if(unit.IsFriendly())
            {
                friendlyUnits.Add(unit);
            }
            else
            {
                enemyUnits.Add(unit);
            }
        }
    }

    private void OnDestroy() 
    {
        Health.OnDeath -= Health_OnDeath;
    }

    private void Health_OnDeath(object sender, EventArgs e)
    {
        ((Health)sender).TryGetComponent<Unit>(out Unit unit);

        if(unit != null)
        {
            if(friendlyUnits.Contains(unit))
            {
                friendlyUnits.Remove(unit);
            }
            else if(enemyUnits.Contains(unit))
            {
                enemyUnits.Remove(unit);
            }
        }
    }

    public void ResetAllUnitActionPoints()
    {
        foreach (Unit unit in friendlyUnits)
        {
            foreach (UnitAction unitAction in unit.GetUnitActions())
            {
                unitAction.ResetActionPoints();
            }
        }

        foreach (Unit unit in enemyUnits)
        {
            foreach (UnitAction unitAction in unit.GetUnitActions())
            {
                unitAction.ResetActionPoints();
            }
        }
    }

    public List<Unit> GetEnemyUnitsList()
    {
        return enemyUnits;
    }

    public List<Unit> GetFriendlyUnitsList()
    {
        return friendlyUnits;
    }

    public Unit GetClosestFriendlyUnitToPosition(Vector2Int gridPosition, out float closestUnitDistance)
    {
        Unit closestUnit = null;
        closestUnitDistance = 0f;

        foreach (Unit friendlyUnit in friendlyUnits)
        {
            float testDistance = gridManager.GetRelativeDistanceOfGridPositions(gridPosition, gridManager.GetGridPositionFromWorldPosition(friendlyUnit.transform.position));
            if(closestUnit == null || testDistance < closestUnitDistance)
            {
                closestUnit = friendlyUnit;
                closestUnitDistance = testDistance;
            }
        }

        return closestUnit;
    }
}

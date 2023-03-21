using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    [SerializeField] private List<Unit> friendlyUnits;
    [SerializeField] private List<Unit> enemyUnits;

    private void Awake() 
    {
        Health.OnDeath += Health_OnDeath;
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
            unit.GetComponent<UnitAttack>().ResetActionPoints();
            unit.GetComponent<UnitMovement>().ResetActionPoints();
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
}

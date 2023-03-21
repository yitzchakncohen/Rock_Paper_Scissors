using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    [SerializeField] private List<Unit> friendlyUnits;
    [SerializeField] private List<Unit> enemyUnits;

    public void ResetAllUnitActionPoints()
    {
        foreach (Unit unit in friendlyUnits)
        {
            unit.GetComponent<UnitAttack>().ResetAttackPoints();
            unit.GetComponent<UnitMovement>().ResetMovementPoints();
        }
    }
}

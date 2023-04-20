using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RockPaperScissors.Unit
{
    public class UnitManager : MonoBehaviour
    {
        private GridManager gridManager;
        private List<Unit> friendlyUnits;
        private List<Unit> enemyUnits;

        private void Awake() 
        {
            UnitHealth.OnDeath += Health_OnDeath;
            Unit.OnUnitSpawn += Unit_OnUnitSpawn;
            friendlyUnits = new List<Unit>();
            enemyUnits = new List<Unit>();
            gridManager = FindObjectOfType<GridManager>();
        }

        private void Unit_OnUnitSpawn(object sender, EventArgs e)
        {
            Unit unit = (Unit)sender;
            if(unit.IsFriendly())
            {
                friendlyUnits.Add(unit);
            }
            else
            {
                enemyUnits.Add(unit);
            }
        }

        private void OnDestroy() 
        {
            UnitHealth.OnDeath -= Health_OnDeath;
        }

        private void Health_OnDeath(object sender, Unit attacker)
        {
            ((UnitHealth)sender).TryGetComponent<Unit>(out Unit unit);

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

        public int GetFriendlyAvaliableActionsRemaining()
        {
            int actionPoints = 0;

            foreach (Unit friendlyUnit in friendlyUnits)
            {
                foreach (UnitAction unitAction in friendlyUnit.GetUnitActions())
                {
                    actionPoints += unitAction.GetValidActionsRemaining();
                }
            }

            return actionPoints;
        }
    }
}

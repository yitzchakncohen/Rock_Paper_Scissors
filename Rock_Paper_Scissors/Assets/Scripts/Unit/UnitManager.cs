using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using RockPaperScissors.Grids;
using UnityEngine;

namespace RockPaperScissors.Units
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
            TurnManager.OnNextTurn += TurnManager_OnNextTurn;
            friendlyUnits = new List<Unit>();
            enemyUnits = new List<Unit>();
            gridManager = FindObjectOfType<GridManager>();
        }

        private void OnDestroy() 
        {
            UnitHealth.OnDeath -= Health_OnDeath;
            Unit.OnUnitSpawn -= Unit_OnUnitSpawn;
            TurnManager.OnNextTurn -= TurnManager_OnNextTurn;
        }

        private void Unit_OnUnitSpawn(object sender, EventArgs e)
        {
            Unit unit = (Unit)sender;
            if(unit.IsFriendly)
            {
                friendlyUnits.Add(unit);
            }
            else
            {
                enemyUnits.Add(unit);
            }
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

        private void TurnManager_OnNextTurn(object sender, TurnManager.OnNextTurnEventArgs e)
        {
            ResetAllUnitActionPoints();
        }

        public void ResetAllUnitActionPoints()
        {
            foreach (Unit unit in friendlyUnits)
            {
                foreach (UnitAction unitAction in unit.UnitActions)
                {
                    unitAction.ResetActionPoints();
                }
            }

            foreach (Unit unit in enemyUnits)
            {
                foreach (UnitAction unitAction in unit.UnitActions)
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
                float testDistance = gridManager.GetGridDistanceBetweenPositions(gridPosition, gridManager.GetGridPositionFromWorldPosition(friendlyUnit.transform.position));
                if(closestUnit == null || testDistance < closestUnitDistance)
                {
                    closestUnit = friendlyUnit;
                    closestUnitDistance = testDistance;
                }
            }

            return closestUnit;
        }

        public async Task<int> GetFriendlyAvaliableActionsRemaining()
        {
            await Task.Yield();
            int actionPoints = 0;

            foreach (Unit friendlyUnit in friendlyUnits)
            {
                foreach (UnitAction unitAction in friendlyUnit.UnitActions)
                {
                    actionPoints += unitAction.GetValidActionsRemaining();
                }
            }

            return actionPoints;
        }
    }
}

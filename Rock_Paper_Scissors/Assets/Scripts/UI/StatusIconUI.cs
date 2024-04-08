using System;
using System.Collections;
using System.Collections.Generic;
using RockPaperScissors.Grids;
using RockPaperScissors.Units;
using UnityEngine;

namespace RockPaperScissors.UI
{
    public class StatusIconUI : MonoBehaviour
    {
        [SerializeField] private GameObject buildingOccupiedIcon;
        [SerializeField] private GameObject actionPointIcon;
        [SerializeField] private Unit unit;
        private GridManager gridManager;

        void Start()
        {
            Unit.OnUnitSpawn += Unit_OnUnitSpawn;            
            UnitAction.OnAnyActionCompleted += UnitAction_OnAnyActionCompleted;
            TurnManager.OnNextTurn += TurnManager_OnNextTurn;
            gridManager = FindObjectOfType<GridManager>();
            // if(!unit.IsFriendly)
            // {
                actionPointIcon.SetActive(false);  
            // }
            // else
            // {
                // actionPointIcon.SetActive(unit.GetTotalActionPointsRemaining() > 0);
            // }
        }

        private void OnDestroy() 
        {
            UnitAction.OnAnyActionCompleted -= UnitAction_OnAnyActionCompleted;
            Unit.OnUnitSpawn -= Unit_OnUnitSpawn;
            TurnManager.OnNextTurn -= TurnManager_OnNextTurn;
        }

        private void UnitAction_OnAnyActionCompleted(object sender, EventArgs e)
        {
            if(unit.IsFriendly)
            {
                UpdateActionPointsIconByAction(sender as UnitAction);
            }
        }

        private void UpdateActionPointsIconByAction(UnitAction unitAction)
        {
            Unit updatedUnit = unitAction.Unit;
            if(updatedUnit == unit && !updatedUnit.IsBuilding)
            {
                CheckForTowerOccupency(updatedUnit);
            }
        }

        private void Unit_OnUnitSpawn(object sender, EventArgs e)
        {
            Unit spawnedUnit = (Unit)sender;
            if(spawnedUnit == unit)
            {
                // actionPointIcon.SetActive(unit.GetTotalActionPointsRemaining() > 0);
                CheckForTowerOccupency(spawnedUnit);
            }
            else if(spawnedUnit.IsBuilding)
            {
                CheckForTowerOccupency(spawnedUnit);
            }
        }
        
        private void CheckForTowerOccupency(Unit updatedUnit)
        {
            GridObject gridObject = gridManager.GetGridObjectFromWorldPosition(updatedUnit.transform.position);
            if(updatedUnit.UnitClass == UnitClass.PillowFort || updatedUnit.UnitClass == UnitClass.PillowOutpost)
            {
                if ((Unit)gridObject.GetOccupantUnit() == unit)
                {
                    buildingOccupiedIcon.SetActive(true);
                }
            }
            else
            {
                if (gridObject.GetOccupantBuilding() != null)
                {
                    buildingOccupiedIcon.SetActive(true);
                }
                else
                {
                    buildingOccupiedIcon.SetActive(false);
                }
            }
        }

        private void TurnManager_OnNextTurn(object sender, TurnManager.OnNextTurnEventArgs e)
        {
            // if(e.IsPlayersTurn && unit.IsFriendly)
            // {
            //     actionPointIcon.SetActive(true);                
            // }
            // else
            // {
            //     actionPointIcon.SetActive(false);
            // }
        }
    }
}

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
        [SerializeField] private Unit unit;
        private GridManager gridManager;

        void Start()
        {
            Unit.OnUnitSpawn += Unit_OnUnitSpawn;            
            UnitAction.OnAnyActionCompleted += UnitAction_OnAnyActionCompleted;
            gridManager = FindObjectOfType<GridManager>();
        }

        private void OnDestroy() 
        {
            UnitAction.OnAnyActionCompleted -= UnitAction_OnAnyActionCompleted;
            Unit.OnUnitSpawn -= Unit_OnUnitSpawn;
        }

        private void UnitAction_OnAnyActionCompleted(object sender, EventArgs e)
        {
            if(sender as UnitMovement)
            {
                Unit updatedUnit = ((UnitMovement)sender).GetUnit();
                if(updatedUnit == unit)
                {
                    CheckForTowerOccupency(updatedUnit);
                }
            }
        }

        private void Unit_OnUnitSpawn(object sender, EventArgs e)
        {
            Unit spawnedUnit = (Unit)sender;
            if(spawnedUnit == unit)
            {
                CheckForTowerOccupency(spawnedUnit);
            }
        }
        
        private void CheckForTowerOccupency(Unit updatedUnit)
        {
            GridObject gridObject = gridManager.GetGridObjectFromWorldPosition(updatedUnit.transform.position);
            if (gridObject.GetOccupentBuilding() != null)
            {
                buildingOccupiedIcon.SetActive(true);
            }
            else
            {
                buildingOccupiedIcon.SetActive(false);
            }
        }
    }
}

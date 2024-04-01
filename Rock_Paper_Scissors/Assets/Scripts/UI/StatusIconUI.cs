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
            // float startTime = Time.realtimeSinceStartup;

            if(sender as UnitMovement)
            {
                Unit updatedUnit = ((UnitMovement)sender).GetUnit();
                if(updatedUnit == unit)
                {
                    CheckForTowerOccupency(updatedUnit);
                }
            }
            
            // Debug.Log("StatusIconUI Action Complete Time: " + (Time.realtimeSinceStartup - startTime)*1000f);
        }

        private void Unit_OnUnitSpawn(object sender, EventArgs e)
        {
            Unit spawnedUnit = (Unit)sender;
            if(spawnedUnit == unit)
            {
                CheckForTowerOccupency(spawnedUnit);
            }
            else if(spawnedUnit.IsBuilding())
            {
                CheckForTowerOccupency(spawnedUnit);
            }
        }
        
        private void CheckForTowerOccupency(Unit updatedUnit)
        {
            GridObject gridObject = gridManager.GetGridObjectFromWorldPosition(updatedUnit.transform.position);
            if(updatedUnit.GetUnitClass() == UnitClass.PillowFort || updatedUnit.GetUnitClass() == UnitClass.PillowOutpost)
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
    }
}

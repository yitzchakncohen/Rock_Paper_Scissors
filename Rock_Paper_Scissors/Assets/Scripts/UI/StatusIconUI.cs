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
            UnitAction.OnAnyActionCompleted += UnitAction_OnAnyActionCompleted;
            gridManager = FindObjectOfType<GridManager>();
        }

        private void UnitAction_OnAnyActionCompleted(object sender, EventArgs e)
        {
            if(sender as UnitMovement)
            {
                Unit updatedUnit = ((UnitMovement)sender).GetUnit();
                if(updatedUnit == unit)
                {
                    GridObject gridObject = gridManager.GetGridObjectFromWorldPosition(updatedUnit.transform.position);
                    if(gridObject.GetOccupentTower() != null)
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
}

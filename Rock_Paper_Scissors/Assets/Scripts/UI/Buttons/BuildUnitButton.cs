using System.Collections;
using System.Collections.Generic;
using RockPaperScissors.Units;
using UnityEngine;

namespace RockPaperScissors.UI.Buttons
{
    public class BuildUnitButton : BuildingButton
    {
        protected override void UpdateButtonInteractability()
        {
            base.UpdateButtonInteractability();
            bool lowLevelBuilding = (unitPrefab.IsBuilding || unitPrefab.IsTrap ) && unitSpawner.Unit.UnitProgression.Level == 1;
            bool unitButtonInteractability = !lowLevelBuilding || (unitSpawner.Unit.Class == UnitClass.PillowFort) || unitSpawner.Unit.IsMoveable;
            bool sufficientCurrency = currencyBank.GetCurrencyRemaining() >= unitPrefab.Cost;

            Button.interactable = Button.interactable && sufficientCurrency && unitButtonInteractability;

            if(Button.interactable)
            {
                unitThumbnail.color = Color.white;
                unitCostText.color = Color.white;
                currencyIcon.color = Color.white;
            }
            else
            {
                if(!sufficientCurrency)
                {
                    unitCostText.color = notEnoughCurrencyRed;
                }
                if(!unitButtonInteractability)
                {
                    unitThumbnail.color = Button.colors.disabledColor;
                    currencyIcon.color = Button.colors.disabledColor;
                }
            }
        }
    }
}

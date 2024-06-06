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
            bool unitButtonInteractability = !lowLevelBuilding || (unitSpawner.Unit.Class == UnitClass.PillowFort) || unitPrefab.IsMoveable;
            bool sufficientCurrency = currencyBank.GetCurrencyRemaining() >= unitPrefab.Cost;

            Button.interactable = Button.interactable && sufficientCurrency && unitButtonInteractability;

            if(Button.interactable)
            {
                unitThumbnail.color = Color.white;
                unitThumbnail.material.SetFloat("_Alpha", 1);
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
                    currencyIcon.color = Button.colors.disabledColor;
                }
                unitThumbnail.color = Button.colors.disabledColor;
                unitThumbnail.material.SetFloat("_Alpha", Button.colors.disabledColor.a);
            }
        }
    }
}

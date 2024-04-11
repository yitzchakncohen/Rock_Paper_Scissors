using System;
using System.Collections;
using System.Collections.Generic;
using RockPaperScissors.Units;
using UnityEngine;

namespace RockPaperScissors.UI.Buttons
{
    public class UpgradeUnitSpawnerButton : BuildingButton
    {
        protected override void Awake()
        {
            base.Awake();
            Button.onClick.AddListener(UpgradeUnitSpawner);
        }

        private void UpgradeUnitSpawner()
        {
            int upgradeCost = unitSpawner.UnitSpawnerData.UpgradeCost[unitSpawner.Unit.UnitProgression.Level];
            if(currencyBank.TrySpendCurrency(upgradeCost))
            {
                int currentLevel = unitSpawner.Unit.UnitProgression.Level;
                unitSpawner.Unit.UnitProgression.SetLevel(currentLevel+1);
            }
        }

        protected override void UpdateButtonInteractability()
        {
            base.UpdateButtonInteractability();
            bool sufficientCurrency = currencyBank.GetCurrencyRemaining() >= unitSpawner.UnitSpawnerData.UpgradeCost[unitSpawner.Unit.UnitProgression.Level];
            bool isPillowFort = unitSpawner.Unit.Class == UnitClass.PillowFort;
            bool upgradeAvailable = unitSpawner.Unit.UnitProgression.Level == 1;
            Button.interactable = Button.interactable && sufficientCurrency && !isPillowFort && upgradeAvailable;

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

                if(isPillowFort || !upgradeAvailable)
                {
                    unitThumbnail.color = Button.colors.disabledColor;
                    currencyIcon.color = Button.colors.disabledColor;
                }
            }
        }
    }
}

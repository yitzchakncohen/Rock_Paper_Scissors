using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingMenu : MonoBehaviour
{
    [SerializeField] private RadialLayoutGroup buildingMenuUI;
    private Unit parentUnit;

    private void Awake() 
    {
        parentUnit = GetComponentInParent<Unit>();
        ActionHandler.OnUnitSelected += ActionHandler_OnUnitSelected;
        BuildingButton.OnBuildingButtonPressed += BuildingButton_OnBuildingButtonPressed;
    }

    private void BuildingButton_OnBuildingButtonPressed(object sender, BuildButtonArguments e)
    {
        buildingMenuUI.gameObject.SetActive(false);
    }

    private void ActionHandler_OnUnitSelected(object sender, Unit unit)
    {
        if(unit == parentUnit)
        {
            OpenBuildingMenu();
        }
        else
        {
            CloseBuildingMenu();
        }
    }

    public void OpenBuildingMenu()
    {
        buildingMenuUI.gameObject.SetActive(true);
        buildingMenuUI.AnimateMenuOpen();
    }

    public void CloseBuildingMenu()
    {
        buildingMenuUI.AnimateMenuClosed();       
    }
}

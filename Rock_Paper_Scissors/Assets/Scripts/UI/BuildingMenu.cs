using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingMenu : MonoBehaviour
{
    [SerializeField] private Transform buildingMenuUI;
    [SerializeField] private RadialLayoutGroup radialLayoutGroup;
    private Unit parentUnit;

    private void Awake() 
    {
        parentUnit = GetComponentInParent<Unit>();
        ActionHandler.OnUnitSelected += ActionHandler_OnUnitSelected;
        BuildingButton.OnBuildingButtonPressed += BuildingButton_OnBuildingButtonPressed;
        radialLayoutGroup.OnCloseAnimationComplete += RadialLayoutGroup_OnCloseAnimationComplete;
    }

    private void OnDestroy() 
    {
        ActionHandler.OnUnitSelected -= ActionHandler_OnUnitSelected;
        BuildingButton.OnBuildingButtonPressed -= BuildingButton_OnBuildingButtonPressed;
        radialLayoutGroup.OnCloseAnimationComplete -= RadialLayoutGroup_OnCloseAnimationComplete;
    }

    private void RadialLayoutGroup_OnCloseAnimationComplete()
    {
        buildingMenuUI.gameObject.SetActive(false);
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
        else if(unit != null)
        {
            CloseBuildingMenu();
        }
    }

    public void OpenBuildingMenu()
    {
        buildingMenuUI.gameObject.SetActive(true);
        radialLayoutGroup.AnimateMenuOpen();
    }

    public void CloseBuildingMenu()
    {
        if(buildingMenuUI.gameObject.activeSelf)
        {
            radialLayoutGroup.AnimateMenuClosed();       
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildButtonArguments : EventArgs
{
    public Unit unit {get; set;}
    public UnitSpawner unitSpawner {get; set;}
}

public class BuildingButton : MonoBehaviour
{
    [SerializeField] private Unit rockUnitPrefab;
    public static event EventHandler<BuildButtonArguments> OnBuildingButtonPressed;
    private UnitSpawner unitSpawner;

    private void Awake() 
    {
        unitSpawner = GetComponentInParent<UnitSpawner>();
    }

    public void ButtonPressed()
    {
        BuildButtonArguments arguments = new BuildButtonArguments();
        arguments.unit = rockUnitPrefab;
        arguments.unitSpawner = this.unitSpawner;
        OnBuildingButtonPressed?.Invoke(this, arguments);
    }
}


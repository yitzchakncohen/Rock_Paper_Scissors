using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    private Button button;
    private CurrencyBank currencyBank;

    private void Awake() 
    {
        unitSpawner = GetComponentInParent<UnitSpawner>();
        button = GetComponent<Button>();
    }

    private void Start() 
    {
        currencyBank = FindObjectOfType<CurrencyBank>();
        button.interactable = (currencyBank.GetCurrencyRemaining() >= rockUnitPrefab.GetCost());      
    }

    public void ButtonPressed()
    {
        if(currencyBank.GetCurrencyRemaining() >= rockUnitPrefab.GetCost())
        {
            BuildButtonArguments arguments = new BuildButtonArguments();
            arguments.unit = rockUnitPrefab;
            arguments.unitSpawner = this.unitSpawner;
            OnBuildingButtonPressed?.Invoke(this, arguments);
        }
    }
}


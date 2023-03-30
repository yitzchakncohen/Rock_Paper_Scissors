using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildButtonArguments : EventArgs
{
    public Unit unit {get; set;}
    public UnitSpawner unitSpawner {get; set;}
}

public class BuildingButton : MonoBehaviour
{
    [SerializeField] private Image unitThumbnail;
    [SerializeField] private TextMeshProUGUI unitCostText;
    private Unit unitPrefab;
    public static event EventHandler<BuildButtonArguments> OnBuildingButtonPressed;
    private UnitSpawner unitSpawner;
    private Button button;
    private CurrencyBank currencyBank;

    private void Awake() 
    {
        button = GetComponent<Button>();        
    }

    private void Start() 
    {
        currencyBank = FindObjectOfType<CurrencyBank>();
        button.interactable = (currencyBank.GetCurrencyRemaining() >= unitPrefab.GetCost());      
    }

    public void Setup(Unit unit) 
    {
        unitPrefab = unit;
        unitSpawner = GetComponentInParent<UnitSpawner>();
        unitThumbnail.sprite = unitPrefab.GetUnitThumbnail();
        unitCostText.text = unitPrefab.GetCost().ToString();
    }

    public void ButtonPressed()
    {
        if(currencyBank.GetCurrencyRemaining() >= unitPrefab.GetCost())
        {
            BuildButtonArguments arguments = new BuildButtonArguments();
            arguments.unit = unitPrefab;
            arguments.unitSpawner = this.unitSpawner;
            OnBuildingButtonPressed?.Invoke(this, arguments);
        }
    }
}


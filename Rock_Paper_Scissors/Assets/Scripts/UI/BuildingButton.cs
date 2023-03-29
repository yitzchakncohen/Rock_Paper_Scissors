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
    [SerializeField] private Unit unitPrefab;
    [SerializeField] private Image unitThumbnail;
    [SerializeField] private TextMeshProUGUI unitCostText;
    public static event EventHandler<BuildButtonArguments> OnBuildingButtonPressed;
    private UnitSpawner unitSpawner;
    private Button button;
    private CurrencyBank currencyBank;

    private void Awake() 
    {
        unitSpawner = GetComponentInParent<UnitSpawner>();
        button = GetComponent<Button>();
        unitThumbnail.sprite = unitPrefab.GetUnitThumbnail();
        unitCostText.text = unitPrefab.GetCost().ToString();
    }

    private void Start() 
    {
        currencyBank = FindObjectOfType<CurrencyBank>();
        button.interactable = (currencyBank.GetCurrencyRemaining() >= unitPrefab.GetCost());      
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


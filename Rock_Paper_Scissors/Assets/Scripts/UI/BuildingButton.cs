using System;
using RockPaperScissors.Units;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RockPaperScissors.UI
{
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
            UpdateButtonInteractability();
            currencyBank.OnCurrencyChanged += currencyBank_OnCurrencyChanged;
        }

        private void currencyBank_OnCurrencyChanged(object sender, int e)
        {
            UpdateButtonInteractability();
        }

        private void UpdateButtonInteractability()
        {
            button.interactable = currencyBank.GetCurrencyRemaining() >= unitPrefab.GetCost();
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
                BuildButtonArguments arguments = new BuildButtonArguments
                {
                    unit = unitPrefab,
                    unitSpawner = this.unitSpawner
                };
                OnBuildingButtonPressed?.Invoke(this, arguments);
            }
        }
    }
}


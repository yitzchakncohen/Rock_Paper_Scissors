using System;
using RockPaperScissors.Units;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RockPaperScissors.UI.Buttons
{
    public class BuildButtonArguments : EventArgs
    {
        public Unit unit {get; set;}
        public UnitSpawner unitSpawner {get; set;}
    }

    public class BuildingButton : MonoBehaviour
    {
        [SerializeField] protected Color notEnoughCurrencyRed;
        [SerializeField] protected Image buttonImage;
        [SerializeField] protected Image unitThumbnail;
        [SerializeField] protected Image currencyIcon;
        [SerializeField] protected TextMeshProUGUI unitCostText;
        protected Unit unitPrefab;
        public static event EventHandler<BuildButtonArguments> OnBuildingButtonPressed;
        protected UnitSpawner unitSpawner;
        private Button button;
        public Button Button => button;
        protected CurrencyBank currencyBank;

        protected virtual void Awake() 
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(ButtonPressed);       
        }
        
        private void Start() 
        {
            currencyBank = FindObjectOfType<CurrencyBank>();            
        }

        private void OnEnable()
        {
            if(currencyBank == null)
            {
                currencyBank = FindObjectOfType<CurrencyBank>();            
            }
            UpdateButtonInteractability();
            currencyBank.OnCurrencyChanged += currencyBank_OnCurrencyChanged;
        }

        private void OnDisable() 
        {
            currencyBank.OnCurrencyChanged -= currencyBank_OnCurrencyChanged;
        }

        private void OnDestroy() 
        {
            button.onClick.RemoveAllListeners();
        }

        private void currencyBank_OnCurrencyChanged(object sender, int e)
        {
            UpdateButtonInteractability();
        }

        protected virtual void UpdateButtonInteractability()
        {
            bool availableActionPoints = unitSpawner.ActionPointsRemaining > 0;
            bool availablStationaryActionPoints = (unitPrefab.IsBuilding || unitPrefab.IsTrap) && unitSpawner.BuildStationaryUnitActionsRemaining > 0;
            bool availablMoveableActionPoints = unitPrefab.IsMoveable && unitSpawner.BuildMoveableUnitActionsRemaining > 0;

            bool buttonInteractable = availableActionPoints && (availablStationaryActionPoints || availablMoveableActionPoints);
            if(buttonInteractable)
            {
                unitThumbnail.color = Color.white;
                unitCostText.color = Color.white;
                currencyIcon.color = Color.white;
            }
            else
            {
                unitThumbnail.color = button.colors.disabledColor;
                if(!availableActionPoints)
                {
                    unitCostText.color = button.colors.disabledColor;
                }

                currencyIcon.color = button.colors.disabledColor;
            }
            button.interactable = buttonInteractable;
        }

        public void Setup(Unit unit, Color color) 
        {
            unitPrefab = unit;
            unitSpawner = GetComponentInParent<UnitSpawner>();
            unitThumbnail.sprite = unitPrefab.UnitThumbnail;
            unitCostText.text = unitPrefab.Cost.ToString();
            buttonImage.color = color;
        }

        public void ButtonPressed()
        {
            if(currencyBank.GetCurrencyRemaining() >= unitPrefab.Cost)
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


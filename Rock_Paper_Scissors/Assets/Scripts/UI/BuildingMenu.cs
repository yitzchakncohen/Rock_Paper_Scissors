using System;
using RockPaperScissors.Units;
using UnityEngine;

namespace RockPaperScissors.UI
{
    public class BuildingMenu : MonoBehaviour
    {
        [SerializeField] private Transform buildingMenuUI;
        [SerializeField] private RadialLayoutGroup radialLayoutGroup;
        [SerializeField] private BuildingButton buildingButtonPrefab;
        private Unit parentUnit;
        private UnitSpawner unitSpawner;
        private float lastFrameZoom;

        private void Awake() 
        {
            parentUnit = GetComponentInParent<Unit>();
            unitSpawner = parentUnit.GetComponent<UnitSpawner>();
            if(unitSpawner == null)
            {
                Debug.Log("This building menu is not attached to a unit spawner.");
            }else
            {
                foreach (Transform child in radialLayoutGroup.transform)
                {
                    Destroy(child.gameObject);
                }
                foreach (Unit unit in unitSpawner.GetSpawnableUnits())
                {
                    BuildingButton buildingButton = Instantiate(buildingButtonPrefab, radialLayoutGroup.transform);
                    buildingButton.Setup(unit);
                }
            }
            ActionHandler.OnUnitSelected += ActionHandler_OnUnitSelected;
            BuildingButton.OnBuildingButtonPressed += BuildingButton_OnBuildingButtonPressed;
            radialLayoutGroup.OnCloseAnimationComplete += RadialLayoutGroup_OnCloseAnimationComplete;
            GameplayManager.OnGameOver += GameplayManager_OnGameOver;
            TurnManager.OnNextTurn += TurnManager_OnNextTurn;
        }

        private void Start() 
        {
            lastFrameZoom = Camera.main.orthographicSize;
        }

        private void Update() 
        {
            if(lastFrameZoom != Camera.main.orthographicSize)
            {
                transform.localScale = transform.localScale * Camera.main.orthographicSize/lastFrameZoom;
                lastFrameZoom = Camera.main.orthographicSize;
            }
        }

        private void OnDestroy() 
        {
            ActionHandler.OnUnitSelected -= ActionHandler_OnUnitSelected;
            BuildingButton.OnBuildingButtonPressed -= BuildingButton_OnBuildingButtonPressed;
            radialLayoutGroup.OnCloseAnimationComplete -= RadialLayoutGroup_OnCloseAnimationComplete;
            GameplayManager.OnGameOver -= GameplayManager_OnGameOver;
            TurnManager.OnNextTurn -= TurnManager_OnNextTurn;
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

        private void GameplayManager_OnGameOver(int obj)
        {
            CloseBuildingMenu();
        }

        private void TurnManager_OnNextTurn(object sender, TurnManager.OnNextTurnEventArgs e)
        {
            CloseBuildingMenu();
        }
    }
}

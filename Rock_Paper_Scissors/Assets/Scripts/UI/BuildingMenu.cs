using System;
using JetBrains.Annotations;
using RockPaperScissors.Units;
using UnityEngine;
using UnityEngine.UI;

namespace RockPaperScissors.UI
{
    public class BuildingMenu : MonoBehaviour
    {
        public static event Action<Unit> OnGarrisonedUnitSelected;
        [SerializeField] private Transform buildingMainMenuUI;
        [SerializeField] private Transform buildMenuUI;
        [SerializeField] private RadialLayoutGroup radialLayoutGroup;
        [SerializeField] private RadialLayoutGroup mainRadialLayoutGroup;
        [SerializeField] private BuildingButton buildingButtonPrefab;
        [SerializeField] private Button openBuildMenuButton;
        [SerializeField] private Button selectUnitButton;
        [SerializeField] private Button closeMainMenuButton;
        [SerializeField] private Button closeBuildMenuButton;
        private Unit parentUnit;
        private UnitSpawner unitSpawner;
        private Unit garrisonedUnit = null;
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
            mainRadialLayoutGroup.OnCloseAnimationComplete += MainRadialLayoutGroup_OnCloseAnimationComplete;
            GameplayManager.OnGameOver += GameplayManager_OnGameOver;
            TurnManager.OnNextTurn += TurnManager_OnNextTurn;
            openBuildMenuButton.onClick.AddListener(OpenBuildMenu);
            selectUnitButton.onClick.AddListener(OnSelectUnitButtonPress);
            closeMainMenuButton.onClick.AddListener(CloseMainMenu);
            closeBuildMenuButton.onClick.AddListener(CloseBuildingMenu);
            selectUnitButton.interactable = false;
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
            mainRadialLayoutGroup.OnCloseAnimationComplete -= MainRadialLayoutGroup_OnCloseAnimationComplete;
            GameplayManager.OnGameOver -= GameplayManager_OnGameOver;
            TurnManager.OnNextTurn -= TurnManager_OnNextTurn;
            openBuildMenuButton.onClick.RemoveAllListeners();
            selectUnitButton.onClick.RemoveAllListeners();
            closeMainMenuButton.onClick.RemoveAllListeners();
            closeBuildMenuButton.onClick.RemoveAllListeners();
        }

        private void RadialLayoutGroup_OnCloseAnimationComplete()
        {
            buildMenuUI.gameObject.SetActive(false);
        }

        private void MainRadialLayoutGroup_OnCloseAnimationComplete()
        {
            buildingMainMenuUI.gameObject.SetActive(false);
        }

        private void BuildingButton_OnBuildingButtonPressed(object sender, BuildButtonArguments e)
        {
            CloseBuildingMenu();
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
                CloseMainMenu();
                garrisonedUnit = null;
            }
        }

        public void OpenBuildingMenu()
        {
            if(garrisonedUnit != null)
            {
                Debug.Log(garrisonedUnit.gameObject.name);
                selectUnitButton.interactable = true;
                OpenMainMenu();
            }
            else
            {
                OpenBuildMenu();
            }
        }

        public void OpenMainMenu()
        {
            buildingMainMenuUI.gameObject.SetActive(true);
            mainRadialLayoutGroup.AnimateMenuOpen();
        }

        public void OpenBuildMenu()
        {
            CloseMainMenu();
            buildMenuUI.gameObject.SetActive(true);
            radialLayoutGroup.AnimateMenuOpen();
        }

        public void CloseBuildingMenu()
        {
            if(buildMenuUI.gameObject.activeSelf)
            {
                radialLayoutGroup.AnimateMenuClosed();       
            }
            if(garrisonedUnit != null)
            {
                OpenMainMenu();
            }
        }

        public void CloseMainMenu()
        {
            if(buildingMainMenuUI.gameObject.activeSelf)
            {
                mainRadialLayoutGroup.AnimateMenuClosed();
            }
        }

        private void GameplayManager_OnGameOver(int obj)
        {
            CloseBuildingMenu();
            CloseMainMenu();
        }

        private void TurnManager_OnNextTurn(object sender, TurnManager.OnNextTurnEventArgs e)
        {
            CloseBuildingMenu();
            CloseMainMenu();
        }

        private void OnSelectUnitButtonPress()
        {
            OnGarrisonedUnitSelected?.Invoke(garrisonedUnit);
        }

        public void SetGarrisonedUnit(Unit unit)
        {
            garrisonedUnit = unit;
            if(garrisonedUnit != null)
            {
                selectUnitButton.interactable = true;
            }
        }
    }
}

using System;
using System.Collections;
using RockPaperScissors.UI.Buttons;
using RockPaperScissors.UI.Components;
using RockPaperScissors.Units;
using UnityEngine;
using UnityEngine.UI;

namespace RockPaperScissors.UI.Menus
{
    public class BuildingMenu : MonoBehaviour
    {
        public static event Action<Unit> OnGarrisonedUnitSelected;
        [SerializeField] private Transform buildingMainMenuUI;
        [SerializeField] private Transform buildMenuUI;
        [SerializeField] private RadialLayoutGroup buildButtonsRadialLayoutGroup;
        [SerializeField] private RadialLayoutGroup mainRadialLayoutGroup;
        [SerializeField] private BuildingButton buildingButtonPrefab;
        [SerializeField] private Button openBuildMenuButton;
        [SerializeField] private Button selectUnitButton;
        [SerializeField] private UpgradeUnitSpawnerButton upgradeUnitSpawnerButtonPrefab;
        [SerializeField] private Image selectUnitImage;
        [SerializeField] private Button closeMainMenuButton;
        [SerializeField] private Button closeBuildMenuButton;
        [SerializeField] private Color moveableUnitButtonColor;
        [SerializeField] private Color stationaryUnitButtonColor;
        private UpgradeUnitSpawnerButton upgradeUnitSpawnerButton;
        private Unit parentUnit;
        private UnitSpawner unitSpawner;
        private Unit garrisonedUnit = null;


        private void Awake() 
        {
            parentUnit = GetComponentInParent<Unit>();
            unitSpawner = parentUnit.GetComponent<UnitSpawner>();
            if(unitSpawner == null)
            {
                Debug.Log("This building menu is not attached to a unit spawner.");
            }else
            {
                foreach (Transform child in buildButtonsRadialLayoutGroup.transform)
                {
                    Destroy(child.gameObject);
                }
                foreach (Unit unit in unitSpawner.GetSpawnableUnits())
                {
                    BuildingButton buildingButton = Instantiate(buildingButtonPrefab, buildButtonsRadialLayoutGroup.transform);
                    if(unit.IsMoveable)
                    {
                        buildingButton.Setup(unit, moveableUnitButtonColor);
                    }
                    else
                    {
                        buildingButton.Setup(unit, stationaryUnitButtonColor);
                    }
                }
                upgradeUnitSpawnerButton = Instantiate(upgradeUnitSpawnerButtonPrefab, buildButtonsRadialLayoutGroup.transform);
                upgradeUnitSpawnerButton.Setup(parentUnit, stationaryUnitButtonColor);
            }
            ActionHandler.OnUnitSelected += ActionHandler_OnUnitSelected;
            BuildingButton.OnBuildingButtonPressed += BuildingButton_OnBuildingButtonPressed;
            buildButtonsRadialLayoutGroup.OnCloseAnimationComplete += RadialLayoutGroup_OnCloseAnimationComplete;
            mainRadialLayoutGroup.OnCloseAnimationComplete += MainRadialLayoutGroup_OnCloseAnimationComplete;
            GameplayManager.OnGameOver += GameplayManager_OnGameOver;
            TurnManager.OnNextTurn += TurnManager_OnNextTurn;
            openBuildMenuButton.onClick.AddListener(OnOpenBuildMenu);
            selectUnitButton.onClick.AddListener(OnSelectUnitButtonPress);
            closeMainMenuButton.onClick.AddListener(OnCloseMainMenuButtonPress);
            closeBuildMenuButton.onClick.AddListener(OnCloseBuildMenuButtonPress);
            selectUnitButton.interactable = false;

            buildingMainMenuUI.gameObject.SetActive(false);
            buildMenuUI.gameObject.SetActive(false);            
        }


        private void OnDestroy() 
        {
            ActionHandler.OnUnitSelected -= ActionHandler_OnUnitSelected;
            BuildingButton.OnBuildingButtonPressed -= BuildingButton_OnBuildingButtonPressed;
            buildButtonsRadialLayoutGroup.OnCloseAnimationComplete -= RadialLayoutGroup_OnCloseAnimationComplete;
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
            StartCoroutine(CloseBuildingMenu());
            AudioManager.Instance.PlayMenuNavigationSound();
        }

        private void ActionHandler_OnUnitSelected(object sender, Unit unit)
        {
            if(unit == parentUnit)
            {
                OpenBuildingMenu();
            }
            else
            {
                if(buildMenuUI.gameObject.activeSelf)
                {
                    StartCoroutine(CloseBuildingMenu());
                }
                else if(buildingMainMenuUI.gameObject.activeSelf)
                {
                    StartCoroutine(CloseMainMenu());
                }
                garrisonedUnit = null;
            }
        }

        public void OpenBuildingMenu()
        {
            if(garrisonedUnit != null)
            {
                selectUnitButton.interactable = true;
                StartCoroutine(OpenMainMenu());
            }
            else
            {
                OnOpenBuildMenu();
            }
        }

        public IEnumerator OpenMainMenu()
        {
            buildingMainMenuUI.gameObject.SetActive(true);
            yield return StartCoroutine(mainRadialLayoutGroup.AnimateMenuOpen());
        }

        public void OnOpenBuildMenu()
        {
            StopAllCoroutines();
            StartCoroutine(OpenBuildMenuRoutine());
        }

        private IEnumerator OpenBuildMenuRoutine()
        {
            AudioManager.Instance.PlayMenuNavigationSound();
            yield return StartCoroutine(CloseMainMenu());
            buildMenuUI.gameObject.SetActive(true);
            yield return StartCoroutine(buildButtonsRadialLayoutGroup.AnimateMenuOpen());
        }

        public IEnumerator CloseBuildingMenu()
        {
            if(buildMenuUI.gameObject.activeSelf)
            {
                yield return StartCoroutine(buildButtonsRadialLayoutGroup.AnimateMenuClosed());       
            }
            if(garrisonedUnit != null)
            {
                yield return StartCoroutine(OpenMainMenu());
            }
        }

        public IEnumerator CloseMainMenu()
        {
            if(buildingMainMenuUI.gameObject.activeSelf)
            {
                yield return StartCoroutine(mainRadialLayoutGroup.AnimateMenuClosed());
            }
            else
            {
                yield return null;
            }
        }

        private void GameplayManager_OnGameOver(object sender, EventArgs e)
        {
            StartCoroutine(CloseBuildingMenu());
            StartCoroutine(CloseMainMenu());
        }

        private void TurnManager_OnNextTurn(object sender, TurnManager.OnNextTurnEventArgs e)
        {
            StartCoroutine(CloseBuildingMenu());
            StartCoroutine(CloseMainMenu());
        }

        private void OnCloseBuildMenuButtonPress()
        {
            StopAllCoroutines();
            StartCoroutine(CloseBuildingMenu());
            AudioManager.Instance.PlayMenuNavigationSound();
        }

        private void OnCloseMainMenuButtonPress()
        {
            StopAllCoroutines();
            StartCoroutine(CloseMainMenu());
            AudioManager.Instance.PlayMenuNavigationSound();
        }

        private void OnSelectUnitButtonPress()
        {
            StopAllCoroutines();
            OnGarrisonedUnitSelected?.Invoke(garrisonedUnit);
            AudioManager.Instance.PlayMenuNavigationSound();
        }

        public void SetGarrisonedUnit(Unit unit)
        {
            garrisonedUnit = unit;
            if(garrisonedUnit != null)
            {
                selectUnitButton.interactable = true;
                selectUnitImage.sprite = garrisonedUnit.UnitThumbnail;
            }
        }
    }
}

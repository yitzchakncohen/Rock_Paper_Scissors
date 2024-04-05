using System;
using System.Collections;
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
        [SerializeField] private RadialLayoutGroup buildButtonsRadialLayoutGroup;
        [SerializeField] private RadialLayoutGroup mainRadialLayoutGroup;
        [SerializeField] private BuildingButton buildingButtonPrefab;
        [SerializeField] private Button openBuildMenuButton;
        [SerializeField] private Button selectUnitButton;
        [SerializeField] private Image selectUnitImage;
        [SerializeField] private Button closeMainMenuButton;
        [SerializeField] private Button closeBuildMenuButton;
        private Unit parentUnit;
        private UnitSpawner unitSpawner;
        private CameraController cameraController;
        private Unit garrisonedUnit = null;
        private float startingZoom;
        private Vector3 startingScale;

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
                    buildingButton.Setup(unit);
                }
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

            startingScale = transform.localScale;
        }

        private void Start() 
        {
            cameraController = FindObjectOfType<CameraController>();
            startingZoom = cameraController.DefaultZoom;
        }

        private void Update() 
        {
            if(startingZoom != Camera.main.orthographicSize)
            {
                transform.localScale = startingScale * Camera.main.orthographicSize/startingZoom;
            }
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
                Debug.Log(garrisonedUnit.gameObject.name);
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
                selectUnitImage.sprite = garrisonedUnit.GetUnitThumbnail();
            }
        }
    }
}

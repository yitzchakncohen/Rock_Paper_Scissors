using System;
using System.Collections.Generic;
using RockPaperScissors.Grids;
using RockPaperScissors.SaveSystem;
using RockPaperScissors.UI.Buttons;
using Unity.VisualScripting;
using UnityEngine;

namespace RockPaperScissors.Units
{
    public class UnitSpawner : UnitAction
    {
        [SerializeField] private UnitSpawnerData unitSpawnerData;
        public UnitSpawnerData UnitSpawnerData => unitSpawnerData;
        private GridManager gridManager;
        private InputManager inputManager;
        private CurrencyBank currencyBank;
        bool placingUnit = false;
        bool unitSpawning = false;
        private Unit unitToSpawn = null;
        private float timer;
        private int buildMoveableUnitActionsRemaining = 1;
        private int buildStationaryUnitActionsRemaining = 1;
        public int BuildMoveableUnitActionsRemaining => buildMoveableUnitActionsRemaining;
        public int BuildStationaryUnitActionsRemaining => buildStationaryUnitActionsRemaining;

        protected override void Awake()
        {
            base.Awake();
            maxActionPoints = unitSpawnerData.ActionPoints;
            actionPointsRemaining = maxActionPoints;
        }

        protected override void Start() 
        {
            base.Start(); 
            IsCancellableAction = true;

            gridManager = FindObjectOfType<GridManager>();
            inputManager = FindObjectOfType<InputManager>();
            currencyBank = FindObjectOfType<CurrencyBank>();
            inputManager.OnSingleTap += InputManager_OnSingleTap;
            BuildingButton.OnBuildingButtonPressed += BuildingButton_OnBuildingButtonPressed;
            TurnManager.OnNextTurn += TurnManager_OnNextTurn;            
        }

        protected override void OnDestroy() 
        {
            base.OnDestroy();
            BuildingButton.OnBuildingButtonPressed -= BuildingButton_OnBuildingButtonPressed;  
            TurnManager.OnNextTurn -= TurnManager_OnNextTurn;
        }

        private void Update() 
        {
            if(!placingUnit)
            {
                return;
            }

            if(unitSpawning)
            {
                timer -= Time.deltaTime;
                if(timer < 0)
                {
                    placingUnit = false;
                    unitSpawning = false;
                    ActionComplete();
                }
            }
        }

        private void BuildingButton_OnBuildingButtonPressed(object sender, BuildButtonArguments args)
        {
            if(args.unitSpawner == this)
            {
                if(sender as BuildUnitButton)
                {
                    unitToSpawn = args.unit;
                }
                else if(sender as UpgradeUnitSpawnerButton)
                {
                    buildStationaryUnitActionsRemaining--;
                    actionPointsRemaining -= 1;
                }
            }
        }

        private void TurnManager_OnNextTurn(object sender, TurnManager.OnNextTurnEventArgs e)
        {
            // Gain currency on player's turn. 
            if(e.IsPlayersTurn)
            {
                currencyBank.AddCurrencyToBank(GetCurrencyProducedThisTurn(), unit.transform);
                buildStationaryUnitActionsRemaining = 1;
                buildMoveableUnitActionsRemaining = 1;
            }
        }

        public override EnemyAIAction GetBestEnemyAIAction()
        {
            throw new NotImplementedException();
        }

        public override bool TryTakeAction(GridObject gridObject, Action onActionComplete)
        {
            if(actionPointsRemaining > 0)
            {
                ActionStart(onActionComplete);
                placingUnit = true;
                return true;
            }
            return false;
        }

        public List<Vector2Int> GetValidPlacementPositions(Unit unitToSpawn)
        {
            List<Vector2Int> gridPositionList = new List<Vector2Int>();
            Vector2Int gridPosition = gridManager.GetGridPositionFromWorldPosition(transform.position);
            // Debug.Log($"Spawner at position { gridPosition}");
            int spawnRadius = unitSpawnerData.SpawnRadius[unit.UnitProgression.Level];

            for (int x = -spawnRadius; x <= spawnRadius; x++)
            {
                for (int y = -spawnRadius; y <= spawnRadius; y++)
                {
                    Vector2Int testGridPosition = gridPosition + new Vector2Int(x, y);

                    // Check if it's on the grid.
                    if (!gridManager.IsValidGridPosition(testGridPosition))
                    {
                        continue;
                    }

                    // Check if it's walkable for units
                    if (unitToSpawn.Class != UnitClass.PillowOutpost
                        && !gridManager.GetGridObject(testGridPosition).IsWalkable(unitToSpawn))
                    {
                        continue;
                    }

                    // Check if it has a building already for buildings
                    if (unitToSpawn.Class == UnitClass.PillowOutpost
                        && gridManager.GetGridObject(testGridPosition).OccupantBuilding != null)
                    {
                        continue;
                    }

                    // Check if it's within spawn distance for the outermost grid positions.
                    if (x >= spawnRadius - 1 || x <= -spawnRadius + 1 || y >= spawnRadius - 1 || y <= -spawnRadius + 1)
                    {
                        int testDistance = gridManager.GetGridDistanceBetweenPositions(gridPosition, testGridPosition);
                        if (testDistance > spawnRadius)
                        {
                            continue;
                        }
                    }

                    gridPositionList.Add(testGridPosition);
                }
            }
            return gridPositionList;
        }

        private void InputManager_OnSingleTap(object sender, Vector2 touchPosition)
        {
            if (!placingUnit)
            {
                return;
            }

            Vector3 worldPositionOfInput = Camera.main.ScreenToWorldPoint(touchPosition);
            Vector2Int gridPosition = gridManager.GetGridPositionFromWorldPosition(worldPositionOfInput);

            List<Vector2Int> validPlacementPositions = GetValidPlacementPositions(unitToSpawn);
            if (!validPlacementPositions.Contains(gridPosition))
            {
                return;
            }

            if (currencyBank.TrySpendCurrency(unitToSpawn.Cost))
            {
                Unit unit = Instantiate(unitToSpawn, gridManager.GetGridObject(gridPosition).transform.position, Quaternion.identity);
                timer = 0.25f;
                StartCoroutine(unit.UnitAnimator.SpawnAnimationRoutine(timer));
                AudioManager.Instance.PlayUnitSpawnSound();
                unitSpawning = true;
                if(unitToSpawn.IsBuilding || unitToSpawn.IsTrap)
                {
                    buildStationaryUnitActionsRemaining--;
                }
                else
                {
                    buildMoveableUnitActionsRemaining--;
                }
                actionPointsRemaining -= 1;
            }
        }

        public override int GetValidActionsRemaining()
        {
            int actionPoints = 0;
            if(currencyBank.GetCurrencyRemaining() >= GetMinimumMoveableUnitCost())
            {
                actionPoints += buildMoveableUnitActionsRemaining;
            }
            if(currencyBank.GetCurrencyRemaining() >= GetMinimumStationaryUnitCost() && unit.GetLevel() > 1)
            {
                actionPoints += buildStationaryUnitActionsRemaining;
            }
            else if(currencyBank.GetCurrencyRemaining() >= UnitSpawnerData.UpgradeCost[Unit.UnitProgression.Level])
            {
                actionPoints += buildStationaryUnitActionsRemaining;
            }
            return actionPoints;
        }

        private int GetMinimumMoveableUnitCost()
        {
            int minimumPrice = 0;
            foreach (Unit unit in unitSpawnerData.SpawnableUnits)
            {               
                if(minimumPrice == 0)
                {
                    minimumPrice = unit.Cost;
                }
                else if (unit.IsMoveable && unit.Cost < minimumPrice)
                {
                    minimumPrice = unit.Cost;
                }
            }
            return minimumPrice;
        }

        private int GetMinimumStationaryUnitCost()
        {
            int minimumPrice = 0;
            foreach (Unit unit in unitSpawnerData.SpawnableUnits)
            {               
                bool isStationary = unit.IsBuilding || unit.IsTrap;
                if(minimumPrice == 0)
                {
                    minimumPrice = unit.Cost;
                }
                else if (isStationary && unit.Cost < minimumPrice)
                {
                    minimumPrice = unit.Cost;
                }
            }
            return minimumPrice;
        }

        private int GetMinimumUnitCost()
        {
            int minimumPrice = 0;
            foreach (Unit unit in unitSpawnerData.SpawnableUnits)
            {
                if(minimumPrice == 0)
                {
                    minimumPrice = unit.Cost;
                }
                else if (unit.Cost < minimumPrice)
                {
                    minimumPrice = unit.Cost;
                }
            }
            return minimumPrice;
        }

        public List<Unit> GetSpawnableUnits()
        {
            return unitSpawnerData.SpawnableUnits;
        }

        protected override void CancelButton_OnCancelButtonPress()
        {
            placingUnit = false;
            unitSpawning = false;
            base.CancelButton_OnCancelButtonPress();
        }

        public override void LoadAction(SaveUnitData loadData)
        {
            actionPointsRemaining = loadData.SpawnerActionPointsRemaining;
            buildMoveableUnitActionsRemaining = loadData.SpawnerMoveableActionPointsRemaining;
            buildStationaryUnitActionsRemaining = loadData.SpawnerStationaryActionPointsRemaining;
        }

        public override SaveUnitData SaveAction(SaveUnitData saveData)
        {
            saveData.SpawnerActionPointsRemaining = actionPointsRemaining;
            saveData.SpawnerMoveableActionPointsRemaining = buildMoveableUnitActionsRemaining;
            saveData.SpawnerStationaryActionPointsRemaining = buildStationaryUnitActionsRemaining;
            return saveData;
        }

        public int GetCurrencyProducedThisTurn()
        {
            return unitSpawnerData.CurrencyProducedPerTurn[unit.UnitProgression.Level];
        }
    }
}

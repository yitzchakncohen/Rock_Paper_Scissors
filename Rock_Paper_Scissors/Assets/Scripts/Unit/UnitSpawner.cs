using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using RockPaperScissors.Grids;
using RockPaperScissors.SaveSystem;
using RockPaperScissors.UI;
using UnityEngine;

namespace RockPaperScissors.Units
{
    public class UnitSpawner : UnitAction
    {
        [SerializeField] private UnitSpawnerData unitSpawnerData;
        private GridManager gridManager;
        private InputManager inputManager;
        private CurrencyBank currencyBank;
        bool placingUnit = false;
        bool unitSpawning = false;
        private Unit unitToSpawn = null;
        private Unit unit;
        private float timer;


        protected override void Start() 
        {
            base.Start(); 
            IsCancellableAction = true;
            unit = GetComponent<Unit>();

            gridManager = FindObjectOfType<GridManager>();
            inputManager = FindObjectOfType<InputManager>();
            currencyBank = FindObjectOfType<CurrencyBank>();
            inputManager.OnSingleTap += InputManager_OnSingleTap;
            BuildingButton.OnBuildingButtonPressed += BuildingButton_OnBuildingButtonPressed;
            TurnManager.OnNextTurn += TurnManager_OnNextTurn;
        }

        private void OnDestroy() 
        {
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
                unitToSpawn = args.unit;
            }
        }

        private void TurnManager_OnNextTurn(object sender, TurnManager.OnNextTurnEventArgs e)
        {
            // Gain currency on player's turn. 
            if(e.IsPlayersTurn)
            {
                currencyBank.AddCurrencyToBank(GetCurrencyProducedThisTurn(), unit.transform);
            }
        }

        public override EnemyAIAction GetBestEnemyAIAction()
        {
            throw new NotImplementedException();
        }

        public override bool TryTakeAction(GridObject gridObject, Action onActionComplete)
        {
            ActionStart(onActionComplete);
            placingUnit = true;
            return true;
        }

        public List<Vector2Int> GetValidPlacementPositions(Unit unitToSpawn)
        {
            List<Vector2Int> gridPositionList = new List<Vector2Int>();
            Vector2Int gridPosition = gridManager.GetGridPositionFromWorldPosition(transform.position);
            // Debug.Log($"Spawner at position { gridPosition}");

            for (int x = -unitSpawnerData.SpawnRadius; x <= unitSpawnerData.SpawnRadius; x++)
            {
                for (int y = -unitSpawnerData.SpawnRadius; y <= unitSpawnerData.SpawnRadius; y++)
                {
                    Vector2Int testGridPosition = gridPosition + new Vector2Int(x, y);

                    // Check if it's on the grid.
                    if (!gridManager.IsValidGridPosition(testGridPosition))
                    {
                        continue;
                    }

                    // Check if it's walkable for units
                    if (unitToSpawn.GetUnitClass() != UnitClass.PillowOutpost
                        && !gridManager.GetGridObject(testGridPosition).IsWalkable(unitToSpawn))
                    {
                        continue;
                    }

                    // Check if it has a building already for buildings
                    if (unitToSpawn.GetUnitClass() == UnitClass.PillowOutpost
                        && gridManager.GetGridObject(testGridPosition).GetOccupantBuilding() != null)
                    {
                        continue;
                    }

                    // Check if it's within spawn distance for the outermost grid positions.
                    if (x >= unitSpawnerData.SpawnRadius - 1 || x <= -unitSpawnerData.SpawnRadius + 1 || y >= unitSpawnerData.SpawnRadius - 1 || y <= -unitSpawnerData.SpawnRadius + 1)
                    {
                        int testDistance = gridManager.GetGridDistanceBetweenPositions(gridPosition, testGridPosition);
                        if (testDistance > unitSpawnerData.SpawnRadius)
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

            if (currencyBank.TrySpendCurrency(unitToSpawn.GetCost()))
            {
                Unit unit = Instantiate(unitToSpawn, gridManager.GetGridObject(gridPosition).transform.position, Quaternion.identity);
                timer = 0.25f;
                StartCoroutine(unit.GetUnitAnimator().SpawnAnimationRoutine(timer));
                AudioManager.Instance.PlayUnitSpawnSound();
                unitSpawning = true;
            }
        }

        public override int GetValidActionsRemaining()
        {
            int minimumPrice = GetMinimumUnitCost();

            if (currencyBank.GetCurrencyRemaining() > minimumPrice)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        private int GetMinimumUnitCost()
        {
            // Arbitrarily large starting number
            int minimumPrice = 10000;
            foreach (Unit unit in unitSpawnerData.SpawnableUnits)
            {
                if (unit.GetCost() < minimumPrice)
                {
                    minimumPrice = unit.GetCost();
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
        }

        public override SaveUnitData SaveAction(SaveUnitData saveData)
        {
            return saveData;
        }

        public int GetCurrencyProducedThisTurn()
        {
            return unitSpawnerData.CurrencyProducedPerTurn;
        }
    }
}

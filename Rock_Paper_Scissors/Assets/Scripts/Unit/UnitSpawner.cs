using System;
using System.Collections;
using System.Collections.Generic;
using RockPaperScissors.Grids;
using RockPaperScissors.PathFindings;
using RockPaperScissors.SaveSystem;
using RockPaperScissors.UI;
using UnityEngine;

namespace RockPaperScissors.Units
{
    public class UnitSpawner : UnitAction
    {
        [SerializeField] private Unit[] spawnableUnits;
        [SerializeField] private int placementRadius = 2;
        private GridManager gridManager;
        private PathFinding pathFinding;
        private InputManager inputManager;
        private CurrencyBank currencyBank;
        bool placingUnit = false;
        bool unitSpawning = false;
        private Unit unitToSpawn = null;
        private float timer;


        protected override void Start() 
        {
            base.Start(); 
            IsCancellableAction = true;

            gridManager = FindObjectOfType<GridManager>();
            pathFinding = FindObjectOfType<PathFinding>();
            inputManager = FindObjectOfType<InputManager>();
            currencyBank = FindObjectOfType<CurrencyBank>();
            inputManager.OnSingleTap += InputManager_OnSingleTap;
            BuildingButton.OnBuildingButtonPressed += BuildingButton_OnBuildingButtonPressed;
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

            for (int x = -placementRadius-1; x <= placementRadius+1; x++)
            {
                for (int y = -placementRadius-1; y <= placementRadius+1; y++)
                {
                    Vector2Int testGridPosition = gridPosition + new Vector2Int(x, y);

                    // Check if it's on the grid.
                    if (!gridManager.IsValidGridPosition(testGridPosition))
                    {
                        continue;
                    }

                    // Check if it's walkable for units
                    if (unitToSpawn.GetUnitClass() != UnitClass.PillowOutpost 
                        && !gridManager.GetGridObject(testGridPosition).IsWalkable(unitToSpawn.IsFriendly()))
                    {
                        continue;
                    }

                    // Check if it has a building already for buildings
                    if (unitToSpawn.GetUnitClass() == UnitClass.PillowOutpost 
                        && gridManager.GetGridObject(testGridPosition).GetOccupentBuilding() != null )
                    {
                        continue;
                    }

                    // Check if it's within movement distance

                    // Check from the edge of the unit spawner instead of the middle to create a valid path.
                    // Find the closest spot on the edge
                    Vector2Int pathTestPostion = GetPositionOnEdge(gridPosition, x, y);

                    pathFinding.FindPath(pathTestPostion, testGridPosition, out int testDistance, unitToSpawn.IsFriendly());
                    // Debug.Log($"{pathTestPostion} to {testGridPosition}, path length {testDistance}");
                    if (testDistance > placementRadius)
                    {
                        continue;
                    }

                    gridPositionList.Add(testGridPosition);
                }
            }

            return gridPositionList;
        }

        private static Vector2Int GetPositionOnEdge(Vector2Int gridPosition, int x, int y)
        {
            Vector2Int edgePosition = new Vector2Int(0,0);

            // Left and right
            if(y == 0 && x != 0)
            {
                edgePosition = new Vector2Int(x/Math.Abs(x),0);
            }

            // If y is not zero check if this is an odd row in the grid.
            bool oddRow = gridPosition.y % 2 == 1;

            // Up/down + right/left
            if(y != 0)
            {   
                if(oddRow)
                {
                    if(x <= 0)
                    {
                        edgePosition = new Vector2Int(0,y/Math.Abs(y));
                    }
                    else
                    {
                        edgePosition = new Vector2Int(x/Math.Abs(x),y/Math.Abs(y));
                    }
                }
                else
                {
                    if(x <= 0)
                    {
                        edgePosition = new Vector2Int(-1,y/Math.Abs(y));
                    }
                    else
                    {
                        edgePosition = new Vector2Int(x/Math.Abs(x)-1,y/Math.Abs(y));
                    }
                }
            }

            Vector2Int pathTestPostion = gridPosition + edgePosition;
            return pathTestPostion;
        }

        private void InputManager_OnSingleTap(object sender, Vector2 touchPosition)
        {
            if(!placingUnit)
            {
                return;
            }

            Vector3 worldPositionOfInput = Camera.main.ScreenToWorldPoint(touchPosition);
            Vector2Int gridPosition = gridManager.GetGridPositionFromWorldPosition(worldPositionOfInput);

            if(!GetValidPlacementPositions(unitToSpawn).Contains(gridPosition))
            {
                return;
            }

            if(currencyBank.TrySpendCurrency(unitToSpawn.GetCost()))
            {
                Unit unit = Instantiate(unitToSpawn, gridManager.GetGridObject(gridPosition).transform.position, Quaternion.identity);
                timer = 0.25f;
                StartCoroutine(unit.GetUnitAnimator().SpawnAnimationRoutine(timer));
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
            foreach (Unit unit in spawnableUnits)
            {
                if (unit.GetCost() < minimumPrice)
                {
                    minimumPrice = unit.GetCost();
                }
            }
            return minimumPrice;
        }

        public Unit[] GetSpawnableUnits()
        {
            return spawnableUnits;
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

        public override void SaveAction(SaveUnitData saveData)
        {
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSpawner : UnitAction
{
    [SerializeField] private int placementRadius = 2;
    private GridManager gridManager;
    private PathFinding pathFinding;
    private InputManager inputManager;
    private CurrencyBank currencyBank;
    bool placingUnit = false;
    private Unit unitToSpawn = null;


    private void Start() 
    {
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

        // TODO Unit preview movement?
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

    public List<Vector2Int> GetValidPlacementPositions()
    {
        List<Vector2Int> gridPositionList = new List<Vector2Int>();
        Vector2Int gridPosition = gridManager.GetGridPositionFromWorldPosition(transform.position);

        for (int x = -placementRadius; x <= placementRadius; x++)
        {
            for (int z = -placementRadius; z <= placementRadius; z++)
            {
                Vector2Int testGridPosition = gridPosition + new Vector2Int(x, z);

                // Check if it's on the grid.
                if (!gridManager.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                // Check if it's within movement distance
                pathFinding.FindPath(gridPosition, testGridPosition, out int testDistance);
                if (testDistance > placementRadius)
                {
                    continue;
                }

                // Check if it's walkable
                if (!gridManager.GetGridObject(testGridPosition).IsWalkable())
                {
                    continue;
                }

                gridPositionList.Add(testGridPosition);
            }
        }

        return gridPositionList;
    }

    private void InputManager_OnSingleTap(object sender, Vector2 touchPosition)
    {
        if(!placingUnit)
        {
            return;
        }

        Vector3 worldPositionOfInput = Camera.main.ScreenToWorldPoint(touchPosition);
        Vector2Int gridPosition = gridManager.GetGridPositionFromWorldPosition(worldPositionOfInput);

        if(!GetValidPlacementPositions().Contains(gridPosition))
        {
            return;
        }

        if(currencyBank.TrySpendCurrency(unitToSpawn.GetCost()))
        {
            Instantiate(unitToSpawn, gridManager.GetGridObject(gridPosition).transform.position, Quaternion.identity);
            placingUnit = false;
            ActionComplete();
        }
    }
}

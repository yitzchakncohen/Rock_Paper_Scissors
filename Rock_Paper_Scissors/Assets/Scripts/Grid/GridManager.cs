using System;
using RockPaperScissors.Units;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RockPaperScissors.Grids
{
    public class GridManager : MonoBehaviour
    {
        [SerializeField] private LayerMask occupancyLayerMask;
        [SerializeField] private Vector2Int gridSize;
        [SerializeField] private GridObject gridObjectPrefab;
        [SerializeField] private GameObject borderTilePrefab;
        [SerializeField] private Tilemap baseTilemap;
        private Grid grid;
        private GridObject[,] gridObjects;

        private void Awake() 
        {
            grid = GetComponent<Grid>();
            gridObjects = new GridObject[gridSize.x, gridSize.y];

            // Setup the grid
            for (int x = -2; x <= gridSize.x + 1; x++)
            {
                for (int y = -2; y <= gridSize.y + 1; y++)
                {
                    if(x <= -1 || x >= gridSize.x || y <= -1 || y >= gridSize.y)
                    {
                        Vector3 gridPosition = grid.GetCellCenterWorld(new Vector3Int(x, y, 0));
                        GameObject gridObject = Instantiate(borderTilePrefab, gridPosition, Quaternion.identity);
                    }
                    else
                    {
                        Vector3 gridPosition = grid.GetCellCenterWorld(new Vector3Int(x, y, 0));
                        GridObject gridObject = Instantiate(gridObjectPrefab, gridPosition, Quaternion.identity);
                        gridObject.Setup(new Vector2Int(x,y));
                        gridObjects[x,y] = gridObject;
                    }
                }
            }
        }

        private void Start() 
        {
            UnitAction.OnAnyActionCompleted += UnitAction_OnAnyActionCompleted;
            UnitHealth.OnDeath += Health_OnDeath;
            Unit.OnUnitSpawn += Unit_OnUnitSpawn;
            UpdateGridOccupancy();
        }

        private void OnDestroy() 
        {
            UnitAction.OnAnyActionCompleted -= UnitAction_OnAnyActionCompleted;
            UnitHealth.OnDeath -= Health_OnDeath;
            Unit.OnUnitSpawn -= Unit_OnUnitSpawn;
        }

        public Vector2Int GetGridPositionFromWorldPosition(Vector2 worldPosition)
        {
            Vector3Int targetGridPosition = grid.WorldToCell(worldPosition);
            Vector3 cellCenterPosition = grid.GetCellCenterWorld(targetGridPosition);
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    if(gridObjects[x,y].transform.position == cellCenterPosition)
                    {
                        return new Vector2Int(x,y);
                    }
                }
            }   

            return Vector2Int.zero;
        }

        public GridObject GetGridObjectFromWorldPosition(Vector2 worldPosition)
        {
            Vector2Int gridPosition = GetGridPositionFromWorldPosition(worldPosition);
            return gridObjects[gridPosition.x, gridPosition.y];
        }

        public GridObject GetGridObject(Vector2Int gridPosition)
        {
            return gridObjects[gridPosition.x, gridPosition.y];
        }

        public Vector2Int GetGridSize()
        {
            return gridSize;
        }

        public void UpdateGridOccupancy()
        {
            float raycastDistance = 0.1f;
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    RaycastHit2D[] hits = Physics2D.RaycastAll(gridObjects[x,y].transform.position, Vector2.up, raycastDistance, occupancyLayerMask);
                    gridObjects[x,y].SetOccupentUnit(null);
                    gridObjects[x,y].SetOccupentBuilding(null);
                    foreach (RaycastHit2D hit in hits)
                    {
                        // If the object is a unit set as occupent
                        hit.collider.TryGetComponent<Unit>(out Unit unit);
                        {
                            // Check if the unit is a Building
                            if(unit.GetUnitClass() == UnitClass.PillowOutpost || unit.GetUnitClass() == UnitClass.PillowFort)
                            {
                                gridObjects[x,y].SetOccupentBuilding(unit);
                            }
                            else
                            {
                                gridObjects[x,y].SetOccupentUnit(unit);
                            }
                        }
                    }
                }
            }
        }

        public bool IsValidGridPosition(Vector2Int testGridPosition)
        {
            if(testGridPosition.x >= 0 && testGridPosition.x < gridSize.x)
            {
                if(testGridPosition.y >= 0 && testGridPosition.y < gridSize.y)
                {
                    return true;
                }
            }
            return false;
        }

        public float GetRelativeDistanceOfGridPositions(Vector2Int positionA, Vector2Int positionB)
        {
            Vector2 gridSpacingInWorldSpace = new Vector2(1.5f, 0.75f);
            Vector2 worldPositionA = new Vector2(positionA.x * gridSpacingInWorldSpace.x, positionA.y * gridSpacingInWorldSpace.y);
            Vector2 worldPositionB = new Vector2(positionB.x * gridSpacingInWorldSpace.x, positionB.y * gridSpacingInWorldSpace.y);
            return Vector2.Distance(worldPositionA, worldPositionB);
        }

        public void ResetActionValueTexts()
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                        gridObjects[x,y].SetActionValue(0);
                }
            }
        }

        private void UnitAction_OnAnyActionCompleted(object sender, EventArgs e)
        {
            UpdateGridOccupancy();
            // TODO more efficient occupency update
            // Unit unit = null;
            // unit = ((UnitAction)sender).GetComponent<Unit>();

            // if(unit != null)
            // {
            //     for (int x = 0; x < gridSize.x; x++)
            //     {
            //         for (int y = 0; y < gridSize.y; y++)
            //         {
            //             if(gridObjects[x, y].GetOccupent() == unit)
            //             {
            //                 gridObjects[x, y].SetOccupent(null);
            //             }
            //         }
            //     }

            //     GetGridObjectFromWorldPosition(unit.transform.position).SetOccupent(unit);
            // }
        }

        private void Health_OnDeath(object sender, Unit e)
        {
            Unit unit = ((UnitHealth)sender).GetUnit();
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    if(gridObjects[x, y].GetOccupentUnit() == unit)
                    {
                        gridObjects[x, y].SetOccupentUnit(null);
                    }
                    else if(gridObjects[x, y].GetOccupentBuilding() == unit)
                    {
                        gridObjects[x, y].SetOccupentBuilding(null);
                    }
                }
            }
        }   

        private void Unit_OnUnitSpawn(object sender, EventArgs e)
        {
            Unit unit = sender as Unit;
            if(unit.GetUnitClass() != UnitClass.PillowOutpost && unit.GetUnitClass() != UnitClass.PillowFort)
            {
                GetGridObjectFromWorldPosition(unit.transform.position).SetOccupentUnit(unit);
            }
            else if(unit.GetUnitClass() == UnitClass.PillowOutpost)
            {
                GetGridObjectFromWorldPosition(unit.transform.position).SetOccupentBuilding(unit);
            }
        }
    }    
}

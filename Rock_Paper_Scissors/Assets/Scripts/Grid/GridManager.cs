using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RockPaperScissors.Units;
using UnityEditor.PackageManager;
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
        public enum DirectionFlags
        {
            North, 
            South, 
            East, 
            West,
            NorthEast = North | East,
            SouthEast = South | East,
            NorthWest = North | West,
            SouthWest = South | West
        }

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
                        Instantiate(borderTilePrefab, gridPosition, Quaternion.identity);
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
                    gridObjects[x,y].SetOccupantUnit(null);
                    gridObjects[x,y].SetOccupantBuilding(null);
                    foreach (RaycastHit2D hit in hits)
                    {
                        // If the object is a unit set as Occupant
                        hit.collider.TryGetComponent<IGridOccupantInterface>(out IGridOccupantInterface unit);
                        {
                            // Check if the unit is a Building
                            if(unit.IsBuilding())
                            {
                                gridObjects[x,y].SetOccupantBuilding(unit);
                            }
                            else
                            {
                                gridObjects[x,y].SetOccupantUnit(unit);
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

        public float GetRelativeWorldDistanceOfGridPositions(Vector2Int positionA, Vector2Int positionB)
        {
            Vector2 gridSpacingInWorldSpace = new Vector2(1.5f, 0.75f);
            Vector2 worldPositionA = new Vector2(positionA.x * gridSpacingInWorldSpace.x, positionA.y * gridSpacingInWorldSpace.y);
            Vector2 worldPositionB = new Vector2(positionB.x * gridSpacingInWorldSpace.x, positionB.y * gridSpacingInWorldSpace.y);
            return Vector2.Distance(worldPositionA, worldPositionB);
        }

        public async Task<int> GetGridDistanceBetweenPositionsAsync(Vector2Int positionA, Vector2Int positionB)
        { 
            await Task.Yield();
            List<Vector2Int> gridPositionsInRangeList = new List<Vector2Int>
            {
                // Add the starting position
                positionA
            };


            // Increment outward getting all the positions one layer at a time.
            int distance = 0;
            List<Vector2Int> newPositions = new List<Vector2Int>();
            while (true)
            {
                distance++;
                // Check the valid neighbours of each position and add them to the list if they are new.
                newPositions.Clear();
                foreach (Vector2Int position in gridPositionsInRangeList)
                {
                    newPositions = newPositions.Union(GetNeighbourList(position, 0)).ToList();
                    if(newPositions.Contains(positionB))
                    {
                        return distance;
                    }
                }

                if (newPositions.Count == 0)
                {
                    // Position B not found
                    Debug.LogError("Position B not found on grid");
                    return -1;
                }
                // Add the new positions to the positions list.
                gridPositionsInRangeList = gridPositionsInRangeList.Union(newPositions).ToList();
            }
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
            // TODO more efficient occupency update
            // UpdateGridOccupancy();

            // Death and Spawning already handled

            // float startTime = Time.realtimeSinceStartup;

            // Movement
            if(sender.GetType() == typeof(UnitMovement))
            {
                // Remove from existing grid location
                Unit unit = ((UnitMovement)sender).GetComponent<Unit>();
                for (int x = 0; x < gridSize.x; x++)
                {
                    for (int y = 0; y < gridSize.y; y++)
                    {
                        if((Unit)gridObjects[x, y].GetOccupantUnit() == unit)
                        {
                            gridObjects[x, y].SetOccupantUnit(null);
                        }
                    }
                }

                // Add to new grid location.
                GridObject gridObject = GetGridObjectFromWorldPosition(unit.transform.position);
                gridObject.SetOccupantUnit(unit);
            }

            // Debug.Log("GridManager Action Complete Time: " + (Time.realtimeSinceStartup - startTime)*1000f);
        }

        private void Health_OnDeath(object sender, Unit e)
        {
            Unit unit = ((UnitHealth)sender).GetUnit();
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    if((Unit)gridObjects[x, y].GetOccupantUnit() == unit)
                    {
                        gridObjects[x, y].SetOccupantUnit(null);
                    }
                    else if((Unit)gridObjects[x, y].GetOccupantBuilding() == unit)
                    {
                        gridObjects[x, y].SetOccupantBuilding(null);
                    }
                }
            }
        }   

        private void Unit_OnUnitSpawn(object sender, EventArgs e)
        {
            // TODO Clean up this mess :)
            Unit unit = sender as Unit;
            if(unit.GetUnitClass() == UnitClass.PillowOutpost || unit.GetUnitClass() == UnitClass.PillowOutpost)
            {
                GetGridObjectFromWorldPosition(unit.transform.position).SetOccupantBuilding(unit);
            }
            else if(unit.GetUnitClass() == UnitClass.GlueTrap)
            {
                GetGridObjectFromWorldPosition(unit.transform.position).SetOccupantTrap(unit);
            }
            else
            {
                GetGridObjectFromWorldPosition(unit.transform.position).SetOccupantUnit(unit);
            }
        }

        // Get a list of grid positions that neighbour the current position
        public List<Vector2Int> GetNeighbourList(Vector2Int currentPosition, DirectionFlags directionFlags)
        {
            List<Vector2Int> neighbourList = new List<Vector2Int>();

            bool oddRow = currentPosition.y % 2 == 1;

            if(currentPosition.x - 1 >= 0)
            {
                // Left
                neighbourList.Add(new Vector2Int(currentPosition.x -1, currentPosition.y +0));

            }

            if(currentPosition.x + 1 < GetGridSize().x)
            {
                // Right
                neighbourList.Add(new Vector2Int(currentPosition.x +1, currentPosition.y +0));
            }

            if(currentPosition.y -1 >= 0)
            {
                // Down (left and right)
                neighbourList.Add(new Vector2Int(currentPosition.x +0, currentPosition.y -1));
                if(currentPosition.x - 1 >= 0 && currentPosition.x + 1 < GetGridSize().x)
                {
                    neighbourList.Add(new Vector2Int(currentPosition.x + (oddRow ? +1 : -1), currentPosition.y -1));                
                }
            }

            if(currentPosition.y + 1 < GetGridSize().y)
            {
                // Up (left and right)
                neighbourList.Add(new Vector2Int(currentPosition.x + 0, currentPosition.y +1));
                if(currentPosition.x - 1 >= 0 && currentPosition.x + 1 < GetGridSize().x)
                {
                    neighbourList.Add(new Vector2Int(currentPosition.x + (oddRow ? +1 : -1), currentPosition.y +1));
                }
            }

            return neighbourList;
        }
    }    
}

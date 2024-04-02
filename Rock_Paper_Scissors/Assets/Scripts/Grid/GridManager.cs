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

        [Obsolete("GetRelativeWorldDistanceOfGridPositions is deprecated, please use GetGridDistanceBetweenPositions instead.")]
        public float GetRelativeWorldDistanceOfGridPositions(Vector2Int positionA, Vector2Int positionB)
        {
            Vector2 gridSpacingInWorldSpace = new Vector2(1.5f, 0.75f);
            Vector2 worldPositionA = new Vector2(positionA.x * gridSpacingInWorldSpace.x, positionA.y * gridSpacingInWorldSpace.y);
            Vector2 worldPositionB = new Vector2(positionB.x * gridSpacingInWorldSpace.x, positionB.y * gridSpacingInWorldSpace.y);
            return Vector2.Distance(worldPositionA, worldPositionB);
        }

        public int GetGridDistanceBetweenPositions(Vector2Int positionA, Vector2Int positionB)
        {
            int dx = positionB.x - positionA.x;
            int dy = positionB.y - positionA.y;
            int x = Mathf.Abs(dx);
            int y = Mathf.Abs(dy);
            if (positionA.x % 2 == 1 ^ dx < 0)
            {
                return Mathf.Max(0, x - (y + 1) / 2) + y;
            }
            else
            {
                return Mathf.Max(0, x - y / 2) + y;
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
            UnitMovement unitMovement = sender as UnitMovement;
            if(unitMovement != null && !unitMovement.GetUnit().IsBuilding())
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
            if(unit.IsBuilding())
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
        public List<Vector2Int> GetNeighbourList(Vector2Int currentPosition)
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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    [SerializeField] private LayerMask occupancyLayerMask;
    [SerializeField] private Vector2Int gridSize;
    [SerializeField] private GameObject gridObjectPrefab;
    [SerializeField] private Tilemap baseTilemap;
    private Grid grid;
    private GridObject[,] gridObjects;

    private void Awake() 
    {
        grid = GetComponent<Grid>();
        gridObjects = new GridObject[gridSize.x, gridSize.y];

        // Setup the grid
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector3 gridPosition = grid.GetCellCenterWorld(new Vector3Int(x, y, 0));
                GameObject gridGameObject = Instantiate(gridObjectPrefab, gridPosition, Quaternion.identity);
                if(gridGameObject.TryGetComponent<GridObject>(out GridObject gridObject))
                {
                    gridObject.Setup(new Vector2Int(x,y));
                    gridObjects[x,y] = gridObject;
                }
            }
        }
    }

    private void Start() 
    {
        UnitMovement.OnAnyActionCompleted += UnitMovement_OnAnyActionCompleted;
        UpdateGridOccupancy();
    }

    private void OnDestroy() 
    {
        UnitMovement.OnAnyActionCompleted -= UnitMovement_OnAnyActionCompleted;
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
                RaycastHit2D hit = Physics2D.Raycast(gridObjects[x,y].transform.position, Vector2.up, raycastDistance, occupancyLayerMask);
                if(hit.collider != null)
                {
                    // If the object is a unit set as occupent
                    hit.collider.TryGetComponent<Unit>(out Unit unit);
                    {
                        gridObjects[x,y].SetOccupent(unit);
                        // Debug.Log(hit.collider.name + " at x:" + x + " y: " + y);
                    }
                }
                else
                {
                    gridObjects[x,y].SetOccupent(null);
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

    private void UnitMovement_OnAnyActionCompleted(object sender, EventArgs e)
    {
        UpdateGridOccupancy();
    }   
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject : MonoBehaviour
{
    private Vector2Int gridPosition;
    private GridObjectUI gridObjectUI;
    private bool isWalkable = true;

    private void Awake() 
    {
        gridObjectUI = GetComponent<GridObjectUI>();
    }
    public void Setup(Vector2Int gridPosition) 
    {
        this.gridPosition = gridPosition;
        gridObjectUI.SetGridPosition(gridPosition);
    }

    public Vector2Int GetGridPostion()
    {
        return gridPosition;
    }

    public bool IsWalkable()
    {
        return isWalkable;
    }

    public void SetIsWalkable(bool isWalkable)
    {
        this.isWalkable = isWalkable;
    }
}

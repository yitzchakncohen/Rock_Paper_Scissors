using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject : MonoBehaviour
{
    private Vector2Int gridPosition;
    private GridObjectUI gridObjectUI;

    private void Awake() 
    {
        gridObjectUI = GetComponent<GridObjectUI>();
    }
    public void Setup(Vector2Int gridPosition) 
    {
        this.gridPosition = gridPosition;
        gridObjectUI.SetGridPosition(gridPosition);
    }

    public Vector2 GetGridPostion()
    {
        return gridPosition;
    }
    
}

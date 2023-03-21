using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject : MonoBehaviour
{
    private Vector2Int gridPosition;
    private GridObjectUI gridObjectUI;
    private Unit gridPositionOccupyingGameObject = null;

    private void Awake() 
    {
        gridObjectUI = GetComponent<GridObjectUI>();
    }

    private void Start() 
    {
        
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
        // TODO can you walk over your oawn units?
        return (gridPositionOccupyingGameObject == null);
    }

    public void SetOccupent(Unit occupent)
    {
        gridPositionOccupyingGameObject = occupent;
    }

    public Unit GetOccupent()
    {
        return gridPositionOccupyingGameObject;
    }

    public void ShowHighlight(GridHighlightType highlightType) => gridObjectUI.ShowHighlight(highlightType);

    public void HideHighlight(GridHighlightType highlightType) => gridObjectUI.HideHighlight(highlightType);
    public void HideAllHighlights() => gridObjectUI.HideAllHighlights();
}

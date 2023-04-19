using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridUIManager : MonoBehaviour
{
    private GridManager gridManager;
    
    private void Awake() 
    {
        gridManager = GetComponent<GridManager>();
    }

    public void HideAllGridPosition()
    {
        for (int x = 0; x < gridManager.GetGridSize().x; x++)
        {
            for (int y = 0; y < gridManager.GetGridSize().y; y++)
            {
                Vector2Int gridPosition = new Vector2Int(x,y);
                gridManager.GetGridObject(gridPosition).HideAllHighlights();
            }
        }
    }

    public void ShowGridPositionList(List<Vector2Int> gridPositionList, GridHighlightType highlightType)
    {
        foreach (Vector2Int gridPosition in gridPositionList)
        {
            gridManager.GetGridObject(gridPosition).ShowHighlight(highlightType);
        }
    }
}

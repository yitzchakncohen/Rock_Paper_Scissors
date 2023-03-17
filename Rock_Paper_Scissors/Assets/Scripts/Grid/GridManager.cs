using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    [SerializeField] private Vector2Int gridSize;
    [SerializeField] private GameObject gridObjectPrefab;
    [SerializeField] private Tilemap baseTilemap;
    private Grid grid;
    private List<GridObject> gridObjects = new List<GridObject>();

    private void Awake() 
    {
        grid = GetComponent<Grid>();
    }

    private void Start() 
    {
        for (int x = -gridSize.x/2; x < gridSize.x/2; x++)
        {
            for (int y = -gridSize.y/2; y < gridSize.y/2; y++)
            {
                Vector3 gridPosition = grid.GetCellCenterWorld(new Vector3Int(x, y, 0));
                GameObject gridGameObject = Instantiate(gridObjectPrefab, gridPosition, Quaternion.identity);
                if(gridGameObject.TryGetComponent<GridObject>(out GridObject gridObject))
                {
                    gridObject.Setup(new Vector2Int(x,y));
                }
            }
        }
    }
}

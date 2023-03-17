using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float stoppingDistance = 0.1f;
    private Grid grid;
    private GridManager gridManager;
    private PathFinding pathfinding;
    private List<GridObject> targetGridObjects = null;
    private int currentPositionIndex = 0;
    
    private void Start() 
    {
        grid = FindObjectOfType<Grid>();
        gridManager = FindObjectOfType<GridManager>();
        pathfinding = FindObjectOfType<PathFinding>();
    }

    private void Update() 
    {
        if(targetGridObjects != null && currentPositionIndex < targetGridObjects.Count)
        {
            if(Vector2.Distance(transform.position, targetGridObjects[currentPositionIndex].transform.position) < stoppingDistance)
            {
                transform.position = targetGridObjects[currentPositionIndex].transform.position;
                currentPositionIndex++;
            }
            else
            {
                Vector2 moveDirection = (targetGridObjects[currentPositionIndex].transform.position - transform.position).normalized;
                transform.position += (Vector3)(moveDirection * movementSpeed * Time.deltaTime);
            }
        }
    }

    public void Move(GridObject targetGridObject)
    {
        currentPositionIndex = 0;
        Vector2Int currentGridPosition = gridManager.GetGridPositionFromWorldPosition(transform.position);
        targetGridObjects = pathfinding.FindPath(currentGridPosition, targetGridObject.GetGridPostion(), out int pathLength);
        Debug.Log(targetGridObject.GetGridPostion().ToString());
    }
}

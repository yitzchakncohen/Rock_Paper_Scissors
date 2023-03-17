using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float stoppingDistance = 0.1f;
    private InputManager inputManager;
    private Grid grid;
    private GridManager gridManager;
    private PathFinding pathfinding;
    private List<GridObject> targetGridObjects = null;
    private int currentPositionIndex = 0;
    
    private void Start() 
    {
        inputManager = FindObjectOfType<InputManager>();
        inputManager.onSingleTouch += InputManager_onSingleTouch;
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

    private void InputManager_onSingleTouch(object sender, Vector2 touchPosition)
    {
        Move(touchPosition);
    }

    public void Move(Vector2 touchPosition)
    {
        currentPositionIndex = 0;
        Vector3 worldPositionOfInput = Camera.main.ScreenToWorldPoint(touchPosition);
        GridObject targetGridObject = gridManager.GetGridObjectFromWorldPosition(worldPositionOfInput);
        Vector2Int currentGridPosition = gridManager.GetGridPositionFromWorldPosition(transform.position);
        targetGridObjects = pathfinding.FindPath(currentGridPosition, targetGridObject.GetGridPostion(), out int pathLength);
        Debug.Log(targetGridObject.GetGridPostion().ToString());
    }
}

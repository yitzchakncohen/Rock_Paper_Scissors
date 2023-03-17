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
    private GridObject targetGridObject = null;
    
    private void Start() 
    {
        inputManager = FindObjectOfType<InputManager>();
        inputManager.onSingleTouch += InputManager_onSingleTouch;
        grid = FindObjectOfType<Grid>();
        gridManager = FindObjectOfType<GridManager>();
    }

    private void Update() 
    {
        if(targetGridObject != null)
        {
            if(Vector2.Distance(transform.position, targetGridObject.transform.position) < stoppingDistance)
            {
                targetGridObject = null;
            }
            else
            {
                Vector2 moveDirection = (targetGridObject.transform.position - transform.position).normalized;
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
        Vector3 worldPositionOfInput = Camera.main.ScreenToWorldPoint(touchPosition);
        targetGridObject = gridManager.GetGridObjectFromWorldPosition(worldPositionOfInput);
        Debug.Log(targetGridObject.GetGridPostion().ToString());
    }
}

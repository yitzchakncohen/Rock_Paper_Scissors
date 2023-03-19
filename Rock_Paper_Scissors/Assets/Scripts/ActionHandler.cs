using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionHandler : MonoBehaviour
{
    public static event EventHandler<Unit> OnUnitSelected;
    [SerializeField] private Unit selectedUnit;
    private InputManager inputManager;
    private GridManager gridManager;

    private void Start() 
    {
        inputManager = FindObjectOfType<InputManager>();
        inputManager.onSingleTouch += InputManager_onSingleTouch;

        gridManager = FindObjectOfType<GridManager>();
        inputManager = FindObjectOfType<InputManager>();
    }

    private void InputManager_onSingleTouch(object sender, Vector2 touchPosition)
    {
        Vector3 worldPositionOfInput = Camera.main.ScreenToWorldPoint(touchPosition);
        GridObject targetGridObject = gridManager.GetGridObjectFromWorldPosition(worldPositionOfInput);
        HandleGridObjectTouch(targetGridObject);
    }

    private void HandleGridObjectTouch(GridObject gridObject)
    {
        if(selectedUnit != null)
        {
            // Check if the grid position is occupied. 
            if(gridObject.GetOccupent() != null)
            {
                selectedUnit = gridObject.GetOccupent();
                OnUnitSelected?.Invoke(this, selectedUnit);
            }
            else
            {
                // If the grid position is not occupied, move the selected unit there
                selectedUnit.GetComponent<UnitMovement>().StartMove(gridObject);
            }
        }
    }
}

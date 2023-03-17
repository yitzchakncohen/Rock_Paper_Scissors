using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionHandler : MonoBehaviour
{
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
            selectedUnit.GetComponent<UnitMovement>().Move(gridObject);
        }
    }
}

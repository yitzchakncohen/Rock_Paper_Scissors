using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float draggingSpeed = 5f;
    private InputManager inputManager;
    private Camera mainCamera;
    private Vector2 startDraggingPosition;
    private Vector2 startCameraPosition;
    private Vector2 draggingVector;
    private bool dragging = false;

    private void Start() 
    {
        mainCamera = Camera.main;
        inputManager = FindObjectOfType<InputManager>();
        inputManager.OnStartDragging += InputManager_OnStartDragging;
        inputManager.Dragging += InputManager_Dragging;
        inputManager.OnDraggingCompleted += InputManager_OnDraggingCompleted;
        ActionHandler.OnUnitSelected += ActionHandler_OnUnitSelected;
    }


    private void FixedUpdate() 
    {
        if(dragging)
        {
            transform.position = startCameraPosition + draggingVector;
        }
    }

    private void OnDestroy() 
    {
        inputManager.OnStartDragging -= InputManager_OnStartDragging;
        inputManager.Dragging -= InputManager_Dragging;
        inputManager.OnDraggingCompleted -= InputManager_OnDraggingCompleted;
        ActionHandler.OnUnitSelected -= ActionHandler_OnUnitSelected;
    }

    private void InputManager_OnStartDragging(object sender, Vector2 position)
    {
        startCameraPosition = transform.position;
        startDraggingPosition = mainCamera.ScreenToWorldPoint(position);
        dragging = true;
    }

    private void InputManager_Dragging(object sender, Vector2 position)
    {
        draggingVector = (startDraggingPosition - (Vector2)mainCamera.ScreenToWorldPoint(position));
    }

    private void InputManager_OnDraggingCompleted(object sender, Vector2 position)
    {
        dragging = false;
    }

    private void ActionHandler_OnUnitSelected(object sender, Unit unit)
    {
        transform.position = unit.transform.position;
    }
}

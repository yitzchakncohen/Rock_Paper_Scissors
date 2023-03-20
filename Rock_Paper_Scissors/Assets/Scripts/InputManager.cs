using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public event EventHandler<Vector2> OnSingleTap;
    public event EventHandler<Vector2> OnStartDragging;
    public event EventHandler<Vector2> Dragging;
    public event EventHandler<Vector2> OnDraggingCompleted;
    private PlayerControls playerControls;
    private bool isDragging = false;

    private void Awake() 
    {
        playerControls = new PlayerControls();
    }

    private void OnEnable() 
    {
        playerControls.GameInputs.Enable();        
    }

    private void Start() 
    {
        playerControls.GameInputs.SingleTap.performed += PlayerControls_GameInputs_SingleTouch_performed;
        playerControls.GameInputs.SingleHold.performed += PlayerControls_GameInputs_SingleHold_performed;
    }

    private void OnDisable() 
    {
        playerControls.GameInputs.Disable();
    }

    private void PlayerControls_GameInputs_SingleHold_performed(InputAction.CallbackContext obj)
    {
        DetectDrag();
    }

    private void PlayerControls_GameInputs_SingleTouch_performed(InputAction.CallbackContext obj)
    {   
        DetectTap();
    }

    private void Update()
    {
        if(isDragging && playerControls.GameInputs.SingleHold.phase == InputActionPhase.Performed)
        {
            Debug.Log("Dragging");
            Dragging?.Invoke(this, playerControls.GameInputs.TouchPosition.ReadValue<Vector2>());
        }
        else
        {
            isDragging = false;
            OnDraggingCompleted?.Invoke(this, playerControls.GameInputs.TouchPosition.ReadValue<Vector2>());
        }
    }

    private void DetectDrag()
    {
        Debug.Log("Detect Drag");
        isDragging = true;
        OnStartDragging?.Invoke(this, playerControls.GameInputs.TouchPosition.ReadValue<Vector2>());
    }

    private void DetectTap()
    {
        Debug.Log("Detect Tap");
        OnSingleTap?.Invoke(this, playerControls.GameInputs.TouchPosition.ReadValue<Vector2>());
    }
}

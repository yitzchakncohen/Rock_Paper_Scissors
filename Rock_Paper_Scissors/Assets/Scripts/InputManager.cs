using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public event EventHandler<Vector2> OnSingleTap;
    public event EventHandler<Vector2> OnStartDragging;
    public event EventHandler<Vector2> OnDragging;
    public event EventHandler<Vector2> OnDraggingCompleted;
    public event EventHandler<Vector2> OnStartPinching;
    public event EventHandler<Vector2> OnPinching;
    public event EventHandler<Vector2> OnPinchingCompleted;
    public event EventHandler<float> OnScroll;
    private EventSystem eventSystem;
    private PlayerControls playerControls;
    private bool isDragging = false;
    private bool isPinching = false;
    private bool mouseOverUI = false;

    private void Awake() 
    {
        playerControls = new PlayerControls();
        eventSystem = EventSystem.current;
    }

    private void OnEnable() 
    {
        playerControls.GameInputs.Enable();        
    }

    private void Start() 
    {
        playerControls.GameInputs.SingleTap.performed += PlayerControls_GameInputs_SingleTouch_performed;
        playerControls.GameInputs.SingleHold.performed += PlayerControls_GameInputs_SingleHold_performed;
        playerControls.GameInputs.MultiHold.performed += PlayerControls_GameInputs_MultiHold_performed;
        playerControls.GameInputs.Scroll.performed += PlayerControls_GameInputs_Scroll_performed;
    }

    private void OnDestroy() 
    {
        playerControls.GameInputs.SingleTap.performed -= PlayerControls_GameInputs_SingleTouch_performed;
        playerControls.GameInputs.SingleHold.performed -= PlayerControls_GameInputs_SingleHold_performed;
        playerControls.GameInputs.MultiHold.performed -= PlayerControls_GameInputs_MultiHold_performed;
        playerControls.GameInputs.Scroll.performed -= PlayerControls_GameInputs_Scroll_performed;
    }

    private void OnDisable() 
    {
        playerControls.GameInputs.Disable();
    }

    private void PlayerControls_GameInputs_SingleHold_performed(InputAction.CallbackContext obj)
    {
        DetectDrag();
    }

    private void PlayerControls_GameInputs_MultiHold_performed(InputAction.CallbackContext obj)
    {
        DetectPinch();
    }

    private void PlayerControls_GameInputs_SingleTouch_performed(InputAction.CallbackContext obj)
    {   
        DetectTap();
    }

    private void Update()
    {
        mouseOverUI = eventSystem.IsPointerOverGameObject();

        if(isDragging && playerControls.GameInputs.SingleHold.phase == InputActionPhase.Performed)
        {
            // Debug.Log("Dragging");
            OnDragging?.Invoke(this, playerControls.GameInputs.TouchPosition.ReadValue<Vector2>());

            // Check for double touch
            if(isPinching && playerControls.GameInputs.MultiHold.phase == InputActionPhase.Performed)
            {
                OnPinching?.Invoke(this, playerControls.GameInputs.TouchPosition.ReadValue<Vector2>());
            }
            else
            {
                isPinching = false;
                OnPinchingCompleted?.Invoke(this, playerControls.GameInputs.TouchPosition.ReadValue<Vector2>());
            }
        }
        else
        {
            isDragging = false;
            isPinching = false;
            OnDraggingCompleted?.Invoke(this, playerControls.GameInputs.TouchPosition.ReadValue<Vector2>());
        }
    }

    private void DetectPinch()
    {
        if(mouseOverUI)
        {
            return;
        }
        
        // Debug.Log("Detect Pinch");
        isPinching = true;
        OnStartPinching?.Invoke(this, playerControls.GameInputs.TouchPosition.ReadValue<Vector2>());
    }

    private void DetectDrag()
    {
        if(mouseOverUI)
        {
            return;
        }

        // Debug.Log("Detect Drag");
        isDragging = true;
        OnStartDragging?.Invoke(this, playerControls.GameInputs.TouchPosition.ReadValue<Vector2>());
    }

    private void DetectTap()
    {
        if(mouseOverUI)
        {
            return;
        }

        // Debug.Log("Detect Tap");
        OnSingleTap?.Invoke(this, playerControls.GameInputs.TouchPosition.ReadValue<Vector2>());
    }

    private void PlayerControls_GameInputs_Scroll_performed(InputAction.CallbackContext obj)
    {
        OnScroll?.Invoke(this, Mathf.Clamp(obj.ReadValue<Vector2>().y, -1, 1));
    }

    public PlayerControls GetPlayerControls()
    {
        return playerControls;
    }
}

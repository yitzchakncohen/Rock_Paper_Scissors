using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [SerializeField] private bool debugging = false;
    [SerializeField] float dragThresholdDistance = 10f;
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
    private bool isTouching = false;
    private bool isDragging = false;
    private bool isPinching = false;
    private bool mouseOverUI = false;
    private bool touchOverUI = false;
    private Vector2 touchStartPosition;

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
        playerControls.GameInputs.SingleTouch.started += PlayerControls_GameInputs_SingleTouch_started;
        playerControls.GameInputs.MultiTouch.started += PlayerControls_GameInputs_MultiTouch_started;
        playerControls.GameInputs.SingleTouch.canceled += PlayerControls_GameInputs_SingleTouch_canceled;
        playerControls.GameInputs.MultiTouch.canceled += PlayerControls_GameInputs_MultiTouch_canceled;
        playerControls.GameInputs.Scroll.performed += PlayerControls_GameInputs_Scroll_performed;
    }

    private void OnDisable() 
    {
        playerControls.GameInputs.Disable();
        playerControls.GameInputs.SingleTouch.started -= PlayerControls_GameInputs_SingleTouch_started;
        playerControls.GameInputs.MultiTouch.started -= PlayerControls_GameInputs_MultiTouch_started;
        playerControls.GameInputs.SingleTouch.canceled -= PlayerControls_GameInputs_SingleTouch_canceled;
        playerControls.GameInputs.MultiTouch.canceled -= PlayerControls_GameInputs_MultiTouch_canceled;
        playerControls.GameInputs.Scroll.performed -= PlayerControls_GameInputs_Scroll_performed;
    }

    private void Update() 
    {
        mouseOverUI = eventSystem.IsPointerOverGameObject();
        touchOverUI = false;
        foreach (Touch touch in Input.touches)
        {
            if(eventSystem.IsPointerOverGameObject(touch.fingerId))
            {
                touchOverUI = true;
            }
        }

        if(isTouching)
        {
            float travelDisance = Vector2.Distance(playerControls.GameInputs.TouchPosition.ReadValue<Vector2>(), touchStartPosition);
            if(travelDisance > dragThresholdDistance)
            {
                if(isPinching)
                {
                    if(debugging){Debug.Log("Pinching...");}
                    OnPinching?.Invoke(this, playerControls.GameInputs.TouchPosition.ReadValue<Vector2>());
                }
                else if(!isDragging)
                {
                    {
                        if(debugging){Debug.Log("Start Dragging...");}
                        OnStartDragging?.Invoke(this, playerControls.GameInputs.TouchPosition.ReadValue<Vector2>());
                        isDragging = true;
                    }
                }
                else
                {
                    if(debugging){Debug.Log("Dragging...");}
                    OnDragging?.Invoke(this, playerControls.GameInputs.TouchPosition.ReadValue<Vector2>());
                }
            }
        }
    }

    private void PlayerControls_GameInputs_SingleTouch_started(InputAction.CallbackContext obj)
    {
        if(mouseOverUI || touchOverUI)
        {
            return;
        }

        if(debugging){Debug.Log("Single Touch Started...");}
        isTouching = true;
        touchStartPosition = playerControls.GameInputs.TouchPosition.ReadValue<Vector2>();
    }

    private void PlayerControls_GameInputs_SingleTouch_canceled(InputAction.CallbackContext obj)
    {

        float travelDisance = Vector2.Distance(playerControls.GameInputs.TouchPosition.ReadValue<Vector2>(), touchStartPosition);
        if(travelDisance < dragThresholdDistance && !isPinching)
        {
            if(debugging){Debug.Log("Single Tap...");}
            if(!mouseOverUI && !touchOverUI)
            {
                OnSingleTap?.Invoke(this, playerControls.GameInputs.TouchPosition.ReadValue<Vector2>());                
            }
        }
        isTouching = false;
        isDragging = false;
        isPinching = false;
        if(debugging){Debug.Log("Stop Dragging...");}
        OnDraggingCompleted?.Invoke(this, playerControls.GameInputs.TouchPosition.ReadValue<Vector2>());
    }

    private void PlayerControls_GameInputs_MultiTouch_started(InputAction.CallbackContext obj)
    {
        if(mouseOverUI)
        {
            return;
        }

        isPinching = true;
        if(debugging){Debug.Log("Start Pinching...");}
        OnStartPinching.Invoke(this, playerControls.GameInputs.TouchPosition.ReadValue<Vector2>());
    }

    private void PlayerControls_GameInputs_MultiTouch_canceled(InputAction.CallbackContext obj)
    {
        isPinching = false;
        if(debugging){Debug.Log("Stop Pinching...");}
        OnPinchingCompleted.Invoke(this, playerControls.GameInputs.TouchPosition.ReadValue<Vector2>());
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
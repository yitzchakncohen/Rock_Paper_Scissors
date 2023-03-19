using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public event EventHandler<Vector2> onSingleTouch;
    private PlayerControls playerControls;

    private void Awake() 
    {
        playerControls = new PlayerControls();
        playerControls.GameInputs.Enable();
    }

    private void Update() 
    {
        if(playerControls.GameInputs.SingleTouch.IsPressed() && playerControls.GameInputs.SingleTouch.phase == InputActionPhase.Performed)
        {
            Vector2 touchPosition;
#if UNITY_EDITOR
            touchPosition = Mouse.current.position.ReadValue();
#elif (UNITY_IPHONE || UNITY_ANDROID)
            touchLocation = Touchscreen.current.position.ReadValue();
#endif
            onSingleTouch?.Invoke(this, touchPosition);
        }
    }
}

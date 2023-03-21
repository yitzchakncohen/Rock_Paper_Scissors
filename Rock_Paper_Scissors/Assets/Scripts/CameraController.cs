using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
    [SerializeField] private float zoomSpeed = 1f;
    [SerializeField] private Vector2 zoomClamp = new Vector2(3, 10);
    [SerializeField] private float zoomSmoothing = 0.1f;
    [SerializeField] private PolygonCollider2D cameraBoundaryCollider;
    [SerializeField] private Vector2 cameraMovementDamping = new Vector2(1, 1);
    private float cameraBoundaryMinX;
    private float cameraBoundaryMaxX;
    private float cameraBoundaryMinY;
    private float cameraBoundaryMaxY;
    private float zoomTarget;
    private InputManager inputManager;
    private PlayerControls playerControls;
    private Camera mainCamera;
    private Vector2 startDraggingPosition;
    private Vector2 startCameraPosition;
    private Vector2 draggingVector;
    private float pinchingStartDistance;
    private float pinchingDistance;
    private float pinchingStartZoomValue;
    private bool dragging = false;
    private bool pinching = false;

    private void Start() 
    {
        mainCamera = Camera.main;
        inputManager = FindObjectOfType<InputManager>();
        playerControls = inputManager.GetPlayerControls();
        
        inputManager.OnStartDragging += InputManager_OnStartDragging;
        inputManager.Dragging += InputManager_Dragging;
        inputManager.OnDraggingCompleted += InputManager_OnDraggingCompleted;
        inputManager.OnStartPinching += InputManager_OnStartPinching;
        inputManager.Pinching += InputManager_Pinching;
        inputManager.OnPinchingCompleted += InputManager_OnPinchingCompleted;
        inputManager.OnScroll += InputManager_OnScroll;
        ActionHandler.OnUnitSelected += ActionHandler_OnUnitSelected;

        zoomTarget = cinemachineVirtualCamera.m_Lens.OrthographicSize;

        CalculateCameraBoundary();
    }

    private void OnDestroy() 
    {
        inputManager.OnStartDragging -= InputManager_OnStartDragging;
        inputManager.Dragging -= InputManager_Dragging;
        inputManager.OnDraggingCompleted -= InputManager_OnDraggingCompleted;
        inputManager.OnStartPinching -= InputManager_OnStartPinching;
        inputManager.Pinching -= InputManager_Pinching;
        inputManager.OnPinchingCompleted -= InputManager_OnPinchingCompleted;
        inputManager.OnScroll -= InputManager_OnScroll;
        ActionHandler.OnUnitSelected -= ActionHandler_OnUnitSelected;

    }

    private void FixedUpdate() 
    {
        if(dragging && !pinching)
        {
            Debug.Log("Moving Camera");
            transform.position = startCameraPosition + draggingVector;
        }
        else
        {
            // Dampen the movement from dragging before stopping. 
            if(draggingVector.magnitude > 0.1)
            {
                draggingVector = new Vector2(Mathf.Lerp(draggingVector.x, 0, cameraMovementDamping.x), Mathf.Lerp(draggingVector.y, 0, cameraMovementDamping.y));
                transform.position += (Vector3)draggingVector;
            }
        }

        if(cinemachineVirtualCamera.m_Lens.OrthographicSize != zoomTarget)
        {
            cinemachineVirtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(cinemachineVirtualCamera.m_Lens.OrthographicSize, zoomTarget, zoomSmoothing);
            if(Math.Abs(cinemachineVirtualCamera.m_Lens.OrthographicSize - zoomTarget) < 0.1)
            {
                cinemachineVirtualCamera.m_Lens.OrthographicSize = zoomTarget;
            }
        }
    }

    private void LateUpdate() 
    {
        ClampCameraPosition();
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


    private void InputManager_OnStartPinching(object sender, Vector2 position)
    {
        pinchingStartDistance = Vector2.Distance(playerControls.GameInputs.TouchPosition.ReadValue<Vector2>(), 
                                                    playerControls.GameInputs.SecondaryTouchPosition.ReadValue<Vector2>());
        pinchingStartZoomValue = cinemachineVirtualCamera.m_Lens.OrthographicSize;
        pinching = true;
    }

    private void InputManager_Pinching(object sender, Vector2 position)
    {
        pinchingDistance = Vector2.Distance(playerControls.GameInputs.TouchPosition.ReadValue<Vector2>(), 
                                                    playerControls.GameInputs.SecondaryTouchPosition.ReadValue<Vector2>());
        zoomTarget = Mathf.Clamp(pinchingStartZoomValue * (pinchingStartDistance / pinchingDistance), zoomClamp.x, zoomClamp.y);
    }

    private void InputManager_OnPinchingCompleted(object sender, Vector2 position)
    {
        pinching = false;
    }

    private void ActionHandler_OnUnitSelected(object sender, Unit unit)
    {
        if(unit != null)
        {
            transform.position = unit.transform.position;
        }
    }
    

    private void InputManager_OnScroll(object sender, float amount)
    {
        zoomTarget = Mathf.Clamp(zoomTarget + (amount * zoomSpeed), zoomClamp.x, zoomClamp.y);
    }
    //TODO add pinch zoom

    private void ClampCameraPosition()
    {
        float horizontalBorder = cinemachineVirtualCamera.m_Lens.OrthographicSize * mainCamera.aspect;
        float verticalBorder = cinemachineVirtualCamera.m_Lens.OrthographicSize;
        Vector2 clampedPosition = new Vector2(
            Math.Clamp(transform.position.x, cameraBoundaryMinX+horizontalBorder, cameraBoundaryMaxX-horizontalBorder),
            Math.Clamp(transform.position.y, cameraBoundaryMinY+verticalBorder, cameraBoundaryMaxY-verticalBorder)
        );
        transform.position = clampedPosition;
    }

    private void CalculateCameraBoundary()
    {
        cameraBoundaryMinX = cameraBoundaryCollider.points[0].x;
        cameraBoundaryMaxX = cameraBoundaryCollider.points[0].x;
        cameraBoundaryMinY = cameraBoundaryCollider.points[0].y;
        cameraBoundaryMaxY = cameraBoundaryCollider.points[0].y;

        foreach (Vector2 point in cameraBoundaryCollider.points)
        {
            if(point.x < cameraBoundaryMinX)
            {
                cameraBoundaryMinX = point.x;
            }
            if(point.y < cameraBoundaryMinY)
            {
                cameraBoundaryMinY = point.y;
            }
            if(point.x > cameraBoundaryMaxX)
            {
                cameraBoundaryMaxX = point.x;
            }
            if(point.y > cameraBoundaryMaxY)
            {
                cameraBoundaryMaxY = point.y;
            }
        }

        cameraBoundaryMinX = cameraBoundaryMinX + cameraBoundaryCollider.transform.position.x;
        cameraBoundaryMaxX = cameraBoundaryMaxX + cameraBoundaryCollider.transform.position.x;
        cameraBoundaryMinY = cameraBoundaryMinY + cameraBoundaryCollider.transform.position.y;
        cameraBoundaryMaxY = cameraBoundaryMaxY + cameraBoundaryCollider.transform.position.y;
    }

    private void OnDrawGizmos() 
    {
        Gizmos.color = Color.red;
        Vector2 bottomLeft = new Vector2(cameraBoundaryMinX, cameraBoundaryMinY);
        Vector2 bottomRight = new Vector2(cameraBoundaryMaxX, cameraBoundaryMinY);
        Vector2 topLeft = new Vector2(cameraBoundaryMinX, cameraBoundaryMaxY);
        Vector2 topRight = new Vector2(cameraBoundaryMaxX, cameraBoundaryMaxY);
        Gizmos.DrawLine(bottomLeft, bottomRight);
        Gizmos.DrawLine(bottomLeft, topLeft);
        Gizmos.DrawLine(bottomRight, topRight);
        Gizmos.DrawLine(topLeft, topRight);
    }
}

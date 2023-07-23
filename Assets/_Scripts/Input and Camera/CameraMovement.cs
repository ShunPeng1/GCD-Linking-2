using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtilities;

[RequireComponent(typeof(Camera))]
public class CameraMovement : MonoBehaviour
{
    private Camera _camera;

    [Header("Drag")] 
    [SerializeField] private float _dragSpeed = 500f; // Speed of camera movement
    [SerializeField] private float _smoothTime = 0.125f;
    [SerializeField] private RangeFloat _verticalBoundPosition, _horizontalBoundPosition;
    private Vector3 _currentVelocity;
    private Vector3 _lastMousePosition;
    public bool IsDraggingMouse = false;
    
    [Header("Zoom")]
    [SerializeField] private float _zoomSpeed = 20f;    // Speed of camera zooming
    [SerializeField] private RangeFloat _zoomBound = new RangeFloat(5f, 20f);    // distance for zooming
    private float _originZoomDistance;

    [Header("Scale with camera")] 
    [SerializeField] private List<GameObject> _scaleWithCameraZoomObjects;

    private void Awake()
    {
        _camera = Camera.main;
        _originZoomDistance = _camera.orthographicSize;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            StartDragCamera();
        }

        if (Input.GetMouseButton(1))
        {
            DragCamera();
        }

        HandleZooming();
    }


    // Store the initial mouse position
    public void StartDragCamera()
    {
        _lastMousePosition = _camera.ScreenToViewportPoint(Input.mousePosition);
        IsDraggingMouse = true;
    }
    
    public void DragCamera()
    {
        if (!IsDraggingMouse) return;
        
        // Calculate the mouse movement delta
        Vector3 currentMousePosition = _camera.ScreenToViewportPoint(Input.mousePosition);
        Vector3 mouseDelta =  currentMousePosition - _lastMousePosition;

        // Calculate the camera movement direction based on mouse movement
        Vector3 dragDirection = new Vector3(-mouseDelta.x, -mouseDelta.y, 0f);
        Vector3 targetPosition = _camera.transform.position + dragDirection * _dragSpeed;
        Vector3 boundedTargetPosition =
            new Vector3(
                Mathf.Clamp(targetPosition.x, _horizontalBoundPosition.From, _horizontalBoundPosition.To),
                Mathf.Clamp(targetPosition.y, _verticalBoundPosition.From, _verticalBoundPosition.To),
                targetPosition.z);
        
        // Apply the drag movement
        _camera.transform.position = Vector3.SmoothDamp(_camera.transform.position, boundedTargetPosition, ref _currentVelocity, _smoothTime);
        _lastMousePosition = currentMousePosition;

    }

    public void HandleZooming()
    {
        // Get the scroll wheel input
        float scrollWheelInput = Input.GetAxis("Mouse ScrollWheel");

        // Calculate the new zoom distance
        float newZoomDistance = _camera.orthographicSize - (scrollWheelInput * _zoomSpeed);

        newZoomDistance = Mathf.Clamp(newZoomDistance, _zoomBound.From, _zoomBound.To);
        
        // Update the camera position for zooming
        _camera.orthographicSize = newZoomDistance;

        foreach (var scaleWithCameraObject in _scaleWithCameraZoomObjects)
        {
            scaleWithCameraObject.transform.localScale = Vector3.one * (newZoomDistance / _originZoomDistance);
        }
    }
}

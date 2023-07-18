using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraMovement : MonoBehaviour
{
    private Camera _camera;
    
    [Header("Drag")]
    [SerializeField] private float _dragSpeed = 1f, _smoothTime = 0.2f;   // Speed of camera movement
    private Vector3 _currentVelocity;
    private Vector3 _lastMousePosition;
    
    [Header("Zoom")]
    [SerializeField] private float _zoomSpeed = 20f;    // Speed of camera zooming
    [SerializeField] private float _minZoomDistance = 5f;    // Minimum distance for zooming
    [SerializeField] private float _maxZoomDistance = 20f;   // Maximum distance for zooming
    [SerializeField] private float _zoomElasticity = 0.5f;   // Elasticity factor for zooming
    private float _originZoomDistance;

    [Header("Scale with camera")] 
    [SerializeField] private List<GameObject> _scaleWithCameraObjects;

    private void Start()
    {
        _camera = Camera.main;
        _originZoomDistance = _camera.orthographicSize;
    }

    void LateUpdate()
    {
        // Camera _dragging
        HandleDragging();

        // Camera zooming
        HandleZooming();
    }

    
    private void HandleDragging()
    {
        // Check if right mouse button is pressed
        if (Input.GetMouseButtonDown(1))
        {
            // Store the initial mouse position
            _lastMousePosition = _camera.ScreenToViewportPoint(Input.mousePosition);
        }

        // Check if right mouse button is being held
        if (Input.GetMouseButton(1))
        {
            // Calculate the mouse movement delta
            Vector3 currentMousePosition = _camera.ScreenToViewportPoint(Input.mousePosition);
            Vector3 mouseDelta =  currentMousePosition - _lastMousePosition;

            // Calculate the camera movement direction based on mouse movement
            Vector3 dragDirection = new Vector3(-mouseDelta.x, -mouseDelta.y, 0f);
            Vector3 targetPosition = transform.position + dragDirection * _dragSpeed;
            
            // Apply the drag movement
            //transform.Translate(dragDirection * _dragSpeed, Space.World);
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _currentVelocity, _smoothTime);
            _lastMousePosition = currentMousePosition;
            
        }
    }

    private void HandleZooming()
    {
        // Get the scroll wheel input
        float scrollWheelInput = Input.GetAxis("Mouse ScrollWheel");

        // Calculate the new zoom distance
        float newZoomDistance = _camera.orthographicSize - (scrollWheelInput * _zoomSpeed);

        // Apply elastic bounds to the zoom distance
        if (newZoomDistance < _minZoomDistance)
            newZoomDistance = Mathf.Lerp(_camera.orthographicSize, _minZoomDistance, _zoomElasticity * Time.deltaTime);
        if (newZoomDistance > _maxZoomDistance)
            newZoomDistance = Mathf.Lerp(_camera.orthographicSize, _maxZoomDistance, _zoomElasticity * Time.deltaTime);

        // Update the camera position for zooming
        _camera.orthographicSize = newZoomDistance;

        foreach (var scaleWithCameraObject in _scaleWithCameraObjects)
        {
            scaleWithCameraObject.transform.localScale = Vector3.one * (newZoomDistance / _originZoomDistance);
        }
    }
}

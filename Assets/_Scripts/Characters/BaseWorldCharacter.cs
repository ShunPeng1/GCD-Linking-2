using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;



public class BaseWorldCharacter : BaseGridXYGameObject
{
    public float MoveSpeed = 5f;

    private Rigidbody2D _rb;

    [Header("Lights Components")] 
    [SerializeField] private Light2D _light2D;
    [SerializeField] private float _maxRange = 5f;
    [SerializeField] private int _rayCount = 36;
    [SerializeField] private LayerMask _wallLayerMask;


    protected override void Start()
    {
        base.Start();
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        UpdateLight();
    }

    private void Move()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector2 movement = new Vector2(horizontalInput, verticalInput);
        movement.Normalize();

        _rb.AddForce(movement * MoveSpeed);
        
        //Debug.Log(GetCell().XIndex + " " + GetCell().YIndex);
    }

    private void UpdateLight()
    {
        
        // Calculate the angle between each ray
        float angleStep = 360f / _rayCount;

        // Create a new shape for the light
        var shapePath = new Vector3[_rayCount];

        // For the light not straight the mesh is not linear
        Vector3 zigzagOffset = new Vector3(0.01f, 0.01f);
        
        for (int i = 0; i < _rayCount; i++)
        {
            // Calculate the direction of the ray
            float angle = i * angleStep;
            Vector3 direction = Quaternion.Euler(0f, 0f, angle) * Vector3.right;
            
            
            // Perform the raycast
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, _maxRange, _wallLayerMask);

            // Check if the ray hit a wall
            if (hit.collider != null)
            {
                // Set the vertex position to the collision point
                shapePath[i] = (Vector3)hit.point - transform.position + zigzagOffset;
            }
            else
            {
                // Set the vertex position to the end of the ray
                shapePath[i] = direction * _maxRange + zigzagOffset;
            }

            zigzagOffset = -zigzagOffset;
            //Debug.Log($"Shape path {i} = {shapePath[i]}");
        }

        // Update the light's shape with the new path
        _light2D.SetShapePath(shapePath);
    }

}

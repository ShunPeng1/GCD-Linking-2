using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CharacterLight : MonoBehaviour
{
    [Header("Lights Components")] 
    
    [SerializeField] private LightInformation _lightInformation;
    [SerializeField] private Light2D _light2D;
    
    [SerializeField] private float _lightRange = 5f;
    [SerializeField] private float _fieldOfView = 360f;
    [SerializeField] private Vector3 _startDirection = Vector3.right;

    private Collider2D _collider2D;
    private void Awake()
    {
        InitializeLight();
        UpdateLight();

        _collider2D = GetComponent<Collider2D>();
    }

    private void InitializeLight()
    {
        if (_lightInformation == null) return;
        _lightRange = _lightInformation.LightRange;
        _fieldOfView = _lightInformation.FieldOfView;
        _startDirection =_lightInformation.StartDirection.normalized;
        _light2D.color = _lightInformation.LightColor;
    }

    private void OnValidate()
    {
        if (_lightInformation != null)
        {
            InitializeLight();
            UpdateLight();
        }
        
    }

    public void UpdateLight()
    {
        
        // Calculate the angle between each ray
        float angleStep = _fieldOfView / _lightInformation.RayCount;

        // Create a new shape for the light
        var shapePath = new Vector3[ _lightInformation.RayCount];

        // For the light not straight the mesh is not linear
        Vector3 zigzagOffset = new Vector3(0.001f, 0.001f);
        
        for (int i = 0; i <  _lightInformation.RayCount; i++)
        {
            // Calculate the direction of the ray
            float angle = i * angleStep;
            Vector3 direction = Quaternion.Euler(0f, 0f, angle) * _startDirection;
            
            
            // Perform the raycast
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, _lightRange, _lightInformation.WallCastLayerMask);

            // Check if the ray hit a wall
            if (hit.collider != null)
            {
                // Set the vertex position to the collision point
                shapePath[i] = (Vector3)hit.point - transform.position + zigzagOffset;
            }
            else
            {
                // Set the vertex position to the end of the ray
                shapePath[i] = direction * _lightRange + zigzagOffset;
            }

            zigzagOffset = -zigzagOffset;
            //Debug.Log($"Shape path {i} = {shapePath[i]}");
        }

        // Update the light's shape with the new path
        _light2D.SetShapePath(shapePath);
    }

    public bool TryCastToCharacter(BaseCharacterMapDynamicGameObject checkingCharacter)
    {
        
        var direction = checkingCharacter.transform.position - transform.position ;
        
        // Ignore collisions between this character and other objects for this raycast.
        if (_collider2D != null) _collider2D.enabled = false;
        
        var hit2D = Physics2D.Raycast(transform.position, direction, _lightRange, _lightInformation.TargetCastLayerMask);
        
        // Re-enable collisions between this character and other objects after the raycast is done.
        if (_collider2D != null) _collider2D.enabled = true;
        
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), GetComponent<Collider2D>(), false);

        if ((hit2D.collider != null))
        {
            Debug.DrawLine(transform.position, hit2D.transform.position, Color.blue, 1000f);

            if (hit2D.collider.transform.gameObject.GetComponent<BaseCharacterMapDynamicGameObject>() ==
                checkingCharacter) return true;
        }

        
        Debug.Log("Cast from "+gameObject.name + " to "+ checkingCharacter.gameObject + " doesn't found "+ direction + " direction and "+ _lightRange +" range, " + " it hit " + hit2D.collider );
        return false;

    }
}

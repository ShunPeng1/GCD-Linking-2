using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseWorldCharacter : BaseGridXYGameObject
{
    public float MoveSpeed = 5f;
    private Rigidbody2D _rb;

    protected override void Start()
    {
        base.Start();
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector2 movement = new Vector2(horizontalInput, verticalInput);
        movement.Normalize();

        _rb.AddForce(movement * MoveSpeed);
        
        //Debug.Log(GetCell().XIndex + " " + GetCell().YIndex);
    }

    private void MoveToCell()
    {
        
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;



public class BaseWorldCharacter : BaseGridXYGameObject
{
    public float MoveSpeed = 5f;

    protected Rigidbody2D Rb;
    protected CharacterLight CharacterLight;
    
    protected override void Start()
    {
        base.Start();
        Rb = GetComponent<Rigidbody2D>();
        CharacterLight = GetComponent<CharacterLight>();
    }

    private void Update()
    {
        
    }

    private void Move()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector2 movement = new Vector2(horizontalInput, verticalInput);
        movement.Normalize();

        Rb.AddForce(movement * MoveSpeed);
        
        //Debug.Log(GetCell().XIndex + " " + GetCell().YIndex);
    }

}

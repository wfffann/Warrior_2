using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //组件
    public InPutControls inputControl;
    private Rigidbody2D rb;
    private PhysicsCheck physicsCheck;

    //数值
    public Vector2 inputDirction;
    public float speed;
    public float jumpForce;


    private void Awake()
    {
        inputControl = new InPutControls();
        rb = GetComponent<Rigidbody2D>();
        physicsCheck = GetComponent<PhysicsCheck>();

        inputControl.GamePlay.Jump.started += Jump;
    }

    private void OnEnable()
    {
        inputControl.Enable();
    }

    private void OnDisable()
    {
        inputControl.Disable();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Update()
    {
        inputDirction = inputControl.GamePlay.Movement.ReadValue<Vector2>();
    }

    public void Move()
    {
        rb.velocity = new Vector2(inputDirction.x * speed * Time.deltaTime, rb.velocity.y);

        int faceDir = (int)transform.localScale.x;
        
        if(inputDirction.x > 0)
        {
            faceDir = 1;
        }
        if (inputDirction.x < 0)
        {
            faceDir = -1;
        }

        //翻转
        transform.localScale = new Vector3(faceDir, transform.localScale.y, transform.localScale.z);
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (physicsCheck.isGround)
        {
            rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
        }
    }
}
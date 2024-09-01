using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Processors;

public class PlayerController : MonoBehaviour
{
    //���
    public InPutControls inputControl;
    private Rigidbody2D rb;
    private PhysicsCheck physicsCheck;
    private PlayerAnimation playerAnimation;
    private CapsuleCollider2D coll;

    public PhysicsMaterial2D normal;
    public PhysicsMaterial2D wall;

    //��ֵ
    public Vector2 inputDirction;
    public float speed;
    private float runSpeed;
    private float walkSpeed => speed / 2.5f;
    public float jumpForce;

    private Vector2 originalOfset;
    private Vector2 originalSize;

    public float hurtForce;

    //bool
    public bool isCrouch;
    public bool isHurt;
    public bool isDead;
    public bool isAttack;
    

    private void Awake()
    {
        inputControl = new InPutControls();
        rb = GetComponent<Rigidbody2D>();
        physicsCheck = GetComponent<PhysicsCheck>();
        coll = GetComponent<CapsuleCollider2D>();
        playerAnimation = GetComponent<PlayerAnimation>();

        originalOfset = coll.offset;
        originalSize = coll.size;

        inputControl.GamePlay.Jump.started += Jump;
        inputControl.GamePlay.Attack.started += PlayerAttack;

        #region ǿ����·
        runSpeed = speed;
        inputControl.GamePlay.WalkButton.performed += ctx =>
        {
            if (physicsCheck.isGround)
            {
                speed = walkSpeed;
            }
        };

        inputControl.GamePlay.WalkButton.canceled += ctx =>
        {
            if (physicsCheck.isGround)
            {
                speed = runSpeed;
            }
        };
        #endregion
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
        if(!isHurt && !isAttack)
            Move();
    }

    private void Update()
    {
        inputDirction = inputControl.GamePlay.Movement.ReadValue<Vector2>();
        CheckSateForMaterial();
    }

    //�ƶ�����
    public void Move()
    {
        //�ƶ�
        if(!isCrouch)
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

        //��ת
        transform.localScale = new Vector3(faceDir, transform.localScale.y, transform.localScale.z);

        //�¶�
        isCrouch = inputDirction.y < -0.5f && physicsCheck.isGround;
        if(isCrouch)
        {
            //�޸���ײ���С��λ��
            coll.offset = new Vector2(-0.110259145f, 0.814663291f);
            coll.size = new Vector2(0.688477457f, 1.63378549f);
        }
        else
        {
            //��ԭ
            coll.offset = originalOfset;
            coll.size = originalSize;
        }
    }

    //��Ծ
    private void Jump(InputAction.CallbackContext context)
    {
        if (physicsCheck.isGround)
        {
            rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    //�������
    public void GetHurt(Transform attacker)
    {
        isHurt = true;
        rb.velocity = Vector2.zero;
        Vector2 dir = new Vector2(transform.position.x -attacker.position.x, 0).normalized;
        rb.AddForce(dir * hurtForce, ForceMode2D.Impulse);
    }

    //�������
    public void PlayerDead()
    {
        isDead = true;
        //�رռ�������
        inputControl.GamePlay.Disable();
    }

    /// <summary>
    /// ��ҹ���
    /// </summary>
    /// <param name="context"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void PlayerAttack(InputAction.CallbackContext context)
    { 
        isAttack = true;
        playerAnimation.PlayAttack();
    }

    /// <summary>
    /// ������
    /// </summary>
    public void CheckSateForMaterial()
    {
        coll.sharedMaterial = physicsCheck.isGround ? normal : wall;
    }
}
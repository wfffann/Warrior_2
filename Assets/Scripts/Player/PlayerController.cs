using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Processors;

public class PlayerController : MonoBehaviour
{
    //组件
    public InPutControls inputControl;
    private Rigidbody2D rb;
    private PhysicsCheck physicsCheck;
    private PlayerAnimation playerAnimation;
    private CapsuleCollider2D coll;

    public PhysicsMaterial2D normal;
    public PhysicsMaterial2D wall;

    //数值
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

        #region 强制走路
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

    //移动方面
    public void Move()
    {
        //移动
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

        //翻转
        transform.localScale = new Vector3(faceDir, transform.localScale.y, transform.localScale.z);

        //下蹲
        isCrouch = inputDirction.y < -0.5f && physicsCheck.isGround;
        if(isCrouch)
        {
            //修改碰撞体大小和位移
            coll.offset = new Vector2(-0.110259145f, 0.814663291f);
            coll.size = new Vector2(0.688477457f, 1.63378549f);
        }
        else
        {
            //还原
            coll.offset = originalOfset;
            coll.size = originalSize;
        }
    }

    //跳跃
    private void Jump(InputAction.CallbackContext context)
    {
        if (physicsCheck.isGround)
        {
            rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    //玩家受伤
    public void GetHurt(Transform attacker)
    {
        isHurt = true;
        rb.velocity = Vector2.zero;
        Vector2 dir = new Vector2(transform.position.x -attacker.position.x, 0).normalized;
        rb.AddForce(dir * hurtForce, ForceMode2D.Impulse);
    }

    //玩家死亡
    public void PlayerDead()
    {
        isDead = true;
        //关闭键盘输入
        inputControl.GamePlay.Disable();
    }

    /// <summary>
    /// 玩家攻击
    /// </summary>
    /// <param name="context"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void PlayerAttack(InputAction.CallbackContext context)
    { 
        isAttack = true;
        playerAnimation.PlayAttack();
    }

    /// <summary>
    /// 检测材质
    /// </summary>
    public void CheckSateForMaterial()
    {
        coll.sharedMaterial = physicsCheck.isGround ? normal : wall;
    }
}
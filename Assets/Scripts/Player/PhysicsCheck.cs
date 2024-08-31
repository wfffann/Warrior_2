using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsCheck : MonoBehaviour
{
    //组件
    public Vector2 bottomOffset;

    //数值
    public float checkRaduis;
    public LayerMask groundLayer;

    //bool
    public bool isGround;

    private void Update()
    {
        Check();
    }

    public void Check()
    {
        //地面检测
        isGround = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, checkRaduis, groundLayer);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere((Vector2)transform.position + bottomOffset, checkRaduis);
    }
}

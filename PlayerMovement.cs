﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    private Rigidbody2D rb;
    private BoxCollider2D coll;

    [Header("移动参数")]
    public float speed = 8f;
    public float crouchSpeedDivisor = 3f;

    [Header("跳跃参数")]
    public float jumpForce = 6.3f;
    public float jumpHoldForce = 1.9f;
    public float jumpHoldDuration = 0.1f;
    public float crouchJumpBoost = 2.5f;

    float jumpTime;

    [Header("状态")]
    public bool isCrouch;
    public bool isOnGround;
    public bool isJump;

    [Header("环境监测")]
    public LayerMask groundLayer;

    float xVelocity;

    //按键设置
    bool jumpPressd;
    bool jumpHeld;
    bool crouchHeld;

    //碰撞体尺寸
    Vector2 colliderStandSize;
    Vector2 colliderStandOffset;
    Vector2 colliderCrouchSize;
    Vector2 colliderCrouchOffset;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();

        colliderStandSize = coll.size;
        colliderStandOffset = coll.offset;
        colliderCrouchSize = new Vector2(coll.size.x, coll.size.y / 2f);
        colliderCrouchOffset = new Vector2(coll.offset.x, coll.offset.y / 2f);
    }

    
    void Update()
    {
        jumpPressd = Input.GetButtonDown("Jump");
        jumpHeld= Input.GetButton("Jump");
        crouchHeld= Input.GetButton("Crouch");
    }

    private void FixedUpdate()
    {
        PhysicsCheck();
        GroundMovement();
        MidAirMovement();
    }

    void PhysicsCheck()
    {
        if (coll.IsTouchingLayers(groundLayer))
            isOnGround = true;
        else isOnGround = false;
    }

    void GroundMovement()
    {
        if (crouchHeld && !isCrouch && isOnGround)
            Crouch();
        else if (!crouchHeld && isCrouch)
            StandUp();
        else if (!isOnGround && isCrouch)
            StandUp();
        xVelocity = Input.GetAxis("Horizontal");

        if (isCrouch)
            xVelocity /= crouchSpeedDivisor;

        rb.velocity = new Vector2(xVelocity * speed, rb.velocity.y);
        FlipDirection();
    }

    void MidAirMovement()
    {
        if (jumpPressd && isOnGround && !isJump)
        {
            if (isCrouch && isOnGround)
            {
                StandUp();
                rb.AddForce(new Vector2(0f, crouchJumpBoost), ForceMode2D.Impulse);
            }
            isOnGround = false;
            isJump = true;

            jumpTime = Time.time + jumpHoldDuration;
            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
        }
        else if (isJump)
        {
            if (jumpHeld)
                rb.AddForce(new Vector2(1f, jumpHoldForce), ForceMode2D.Impulse);
            if (jumpTime < Time.time)
                isJump = false;
        }
    }

    void FlipDirection()
    {
        if(xVelocity < 0)
            transform.localScale = new Vector2(-1, 1);
        if(xVelocity>0)
            transform.localScale = new Vector2(1, 1);
    }

    void Crouch()
    {
        isCrouch = true;
        coll.size = colliderCrouchSize;
        coll.offset = colliderCrouchOffset;

    }

    void StandUp()
    {
        isCrouch = false;
        coll.size = colliderStandSize;
        coll.offset = colliderStandOffset;
    }
}

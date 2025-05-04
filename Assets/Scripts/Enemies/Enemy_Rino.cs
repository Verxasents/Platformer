using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Rino : Enemy
{
    [Header("Rino details")]
    [SerializeField] private float maxSpeed;
    [SerializeField] private float speedUpRate = .6f;
    private float defaultSpeed;
    [SerializeField] private Vector2 impactPower;

    [Header("Effects")]
    [SerializeField] private ParticleSystem dustFx;
    [SerializeField] private Vector2 CameraImpulseDir;
    private CinemachineImpulseSource impulseSource;
    protected override void Start()
    {
        base.Start();

        canMove = false;
        defaultSpeed = moveSpeed;
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    protected override void Update()
    {
        base.Update();
        HandleCharge();
    }

    private void HitWallImpact()
    {
        dustFx.Play();
        impulseSource.m_DefaultVelocity = new Vector2(CameraImpulseDir.x * facingDir, CameraImpulseDir.y);
        impulseSource.GenerateImpulse();
    }

    private void HandleCharge()
    {
        if (canMove == false)
            return;
        HandleSpeedUp();

        rb.velocity = new Vector2(moveSpeed * facingDir, rb.velocity.y);

        if (isWallDetected)
            WallHit();

        if (!isGroundInFrontDetected)
        {
            TurnAround();
        }

    }

    private void HandleSpeedUp()
    {
        moveSpeed = moveSpeed + (Time.deltaTime * speedUpRate);

        if (moveSpeed >= maxSpeed)
            maxSpeed = moveSpeed;
    }

    private void TurnAround()
    {
        SpeedReset();
        canMove = false;
        rb.velocity = Vector2.zero;
        Flip();
        moveSpeed = defaultSpeed;
    }

    private void WallHit()
    {
        canMove = false;

        HitWallImpact();
        SpeedReset();

        anim.SetBool("hitWall", true);
        rb.velocity = new Vector2(impactPower.x * -facingDir, impactPower.y);
    }

    private void SpeedReset()
    {
        moveSpeed = defaultSpeed;
    }

    private void ChargeIsOver()
    {
        anim.SetBool("hitWall", false);
        Invoke(nameof(Flip), 1);
    }
    protected override void HandleCollision()
    {
        base.HandleCollision();

        if (isPlayerDetected && isGrounded)
            canMove = true;
  
    }

}

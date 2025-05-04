using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Chicken : Enemy
{
    [Header("Chicken details")]
    [SerializeField] private float aggroDuration;
    [SerializeField] private float detectionRange;
    private float aggroTimer;
    private bool playerDetected;
    private bool canFlip = true;

    protected override void Start()
    {
        base.Start();

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

            if (player == null)
                Debug.LogError("Player bulunamadý! Player tag’ini kontrol et.");
        }
    }

    protected override void Update()
    {

        base.Update();
        aggroTimer -= Time.deltaTime;
        //playerDetected = true;

        if (isDead)
            return;

        if (playerDetected)
        {
            canMove = true;
            aggroTimer = aggroDuration;
        }

        if (aggroTimer < 0)
            canMove = false;

        HandleMovement();

        if (isGrounded)
            HandleTurnAround();
    }

    private void HandleTurnAround()
    {
        if (!isGroundInFrontDetected || isWallDetected)
        {
            Flip();
            canMove = false;
            rb.velocity = Vector2.zero;
        }
    }

    private void HandleMovement()
    {
        if (canMove == false)
            return;

        HandleFlip(player.transform.position.x);

        rb.velocity = new Vector2(moveSpeed * facingDir, rb.velocity.y);
    }

    protected override void HandleCollision()
    {
        base.HandleCollision();

        playerDetected = Physics2D.Raycast(transform.position, Vector2.right * facingDir, detectionRange, whatIsPlayer);
    }

    protected override void HandleFlip(float xValue)
    {

        if (xValue < transform.position.x && facingRight || xValue > transform.position.x && !facingRight)
        {
            if (canFlip)
            {
                canFlip = false;
                Invoke(nameof(Flip), .3f);
            }

        }
    }
    protected override void Flip()
    {
        base.Flip();
        canFlip = true;
    }

}

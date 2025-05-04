using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Mushroom : Enemy
{
    
    protected override void Update()
    {
        base.Update();

        if (isDead)
            return;

        HandleMovement();

        if (isGrounded)
            HandleTurnAround();
    }
    private void HandleTurnAround()
    {
        if (!isGroundInFrontDetected || isWallDetected)
        {
            Flip();
            idleTimer = idleDuration;
            rb.velocity = Vector2.zero;
        }
    }

    private void HandleMovement()
    {
        if (idleTimer > 0)
            return;

           rb.velocity = new Vector2(moveSpeed * facingDir, rb.velocity.y);
    }

}

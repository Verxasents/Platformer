using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_BlueBird : Enemy
{
    [Header("Blue Bird Details")]
    [SerializeField] private float travelDistance = 8;
    [SerializeField] private float flyForce = 1.5f;

    private Vector3[] wayPoints = new Vector3[2];
    private int wayIndex;

    private bool inPlayerMode;

    protected override void Start()
    {
        base.Start();

        wayPoints[0] = new Vector3(transform.position.x - travelDistance / 2, transform.position.y);
        wayPoints[1] = new Vector3(transform.position.x + travelDistance / 2, transform.position.y);

        inPlayerMode = true;

        wayIndex = Random.Range(0,wayPoints.Length);
    }

    protected override void Update()
    {
        base.Update();

        HandleMovement();
    }

    private void FlyUp() => rb.velocity = new Vector2(rb.velocity.x, flyForce);

    private void HandleMovement()
    {
        if (canMove == false)
            return;

        transform.position = Vector2.MoveTowards(transform.position, wayPoints[wayIndex], moveSpeed * Time.deltaTime);
        HandleFlip(wayPoints[wayIndex].x);

        if (Vector2.Distance(transform.position, wayPoints[wayIndex]) < .1f)
        {
            wayIndex++;

            if(wayIndex >= wayPoints.Length)
                wayIndex = 0;
        }
    }
    protected override void HandleAnimator()
    {
        //
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        if (inPlayerMode == false)
        {
            float distance = travelDistance / 2;

            Vector3 leftPos = new Vector3(transform.position.x - distance, transform.position.y);
            Vector3 rightPos = new Vector3(transform.position.x + distance, transform.position.y);

            Gizmos.DrawLine(leftPos, rightPos);

            Gizmos.DrawWireSphere(leftPos, .5f);
            Gizmos.DrawWireSphere(rightPos, .5f);
        }
        else
        {


            Gizmos.DrawLine(transform.position, wayPoints[0]);
            Gizmos.DrawLine(transform.position, wayPoints[1]);

            Gizmos.DrawWireSphere(wayPoints[0], .5f);
            Gizmos.DrawWireSphere(wayPoints[1], .5f);
        }
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Bee : Enemy
{
    [Header("Bee details")]
    [SerializeField] private Enemy_Bullet_Bee bulletPrefab;
    [SerializeField] private Transform gunPoint;
    [SerializeField] private float bulletSpeed = 7;
    [SerializeField] private float bulletLifeTime = 2.5f;
    [SerializeField] private float attackCooldown = 1.5f;
    private float lastTimeAttacked;

    [SerializeField] private float offset = .25f;

    private List<Vector3> wayPoints = new List<Vector3>();
    private int wayIndex;

    private Transform target;

    protected override void Start()
    {
        base.Start();
        canMove = false;
        CreateWaypoints();

        float randomValue = Random.Range(0, .6f);
        Invoke(nameof(AllowMovement), randomValue);
    }

    private void CreateWaypoints()
    {
        wayPoints.Add(transform.position + new Vector3(offset, offset));
        wayPoints.Add(transform.position + new Vector3(offset, -offset));
        wayPoints.Add(transform.position + new Vector3(-offset, -offset));
        wayPoints.Add(transform.position + new Vector3(-offset, offset));
    }

    protected override void Update()
    {
        base.Update();

        HandleMovement();

        FindTargetIfEmpty();

        bool canAttack = Time.time > lastTimeAttacked + attackCooldown && target != null;

        if (canAttack)
            Attack();
    }

    private void FindTargetIfEmpty()
    {
        if (target == null)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, float.MaxValue, whatIsPlayer);

            if (hit.transform != null)
                target = hit.transform;
        }
    }

    private void HandleMovement()
    {
        if (canMove == false)
            return;

        if (isDead)
            return;

        transform.position = Vector2.MoveTowards(transform.position, wayPoints[wayIndex], moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, wayPoints[wayIndex]) < .1f)
        {
            wayIndex++;

            if(wayIndex >= wayPoints.Count - 1)
                wayIndex = 0;
                    
        }
    }

    private void Attack()
    {
        lastTimeAttacked = Time.time;
        anim.SetTrigger("attack");
    }

    private void CreateBullet()
    {
        Enemy_Bullet_Bee newBullet = Instantiate(bulletPrefab, gunPoint.position, Quaternion.identity);
        newBullet.SetupBullet(target, bulletSpeed, bulletLifeTime);

        target = null;
    
    }
    private void AllowMovement() => canMove = true;
    protected override void HandleAnimator()
    {

    }
}


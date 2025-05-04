using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap_Saw : MonoBehaviour
{
    private Animator anim;
    private SpriteRenderer sr;
    [SerializeField] private float moveSpeed = 3;
    [SerializeField] private float coolDown = 1;
    [SerializeField] private Transform[] WayPoint;

    private Vector3[] wayPointPosition;

    public int wayPointIndex = 1;
    public int moveDirection = 1;
    private bool canMove = true;


    private void Awake()
    {
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }


    private void Start()
    {
        UpdateWaypointsInfo();
        transform.position = wayPointPosition[0];
    }

    private void UpdateWaypointsInfo()
    {
        List < Trap_SawWaypoint> wayPointList = new List<Trap_SawWaypoint>(GetComponentsInChildren<Trap_SawWaypoint>());

        if (wayPointList.Count != WayPoint.Length)
        {
            WayPoint = new Transform[wayPointList.Count];

            for (int i = 0; i < wayPointList.Count; i++)
            {
                WayPoint[i] = wayPointList[i].transform;
            }
        }

        wayPointPosition = new Vector3[WayPoint.Length];

        for (int i = 0; i < WayPoint.Length; i++)
        {
            wayPointPosition[i] = WayPoint[i].position;
        }
    }

    private void Update()
    {
        anim.SetBool("active", canMove);


        if (canMove == false)
            return;

        transform.position = Vector2.MoveTowards(transform.position, wayPointPosition[wayPointIndex], moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, wayPointPosition[wayPointIndex]) < .1f)
        {
            if (wayPointIndex == WayPoint.Length - 1 || wayPointIndex == 0)
            {
                moveDirection = moveDirection * -1;
                StartCoroutine(StopMovement(coolDown));
            }

            wayPointIndex = wayPointIndex + moveDirection;
        }
    }

    private IEnumerator StopMovement(float delay)
    {
        canMove = false;

        yield return new WaitForSeconds(delay);

        canMove = true;
        sr.flipX = !sr.flipX;
    }
}



//eþþeðin zikins
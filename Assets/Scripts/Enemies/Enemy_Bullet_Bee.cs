using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Bullet_Bee : MonoBehaviour
{
    private Transform target;
    private List<Vector3> wayPoints = new List<Vector3>();
    private int wayIndex;
    [SerializeField] private GameObject pickupVfx;

    [SerializeField] private float wayPointUpdateCooldown;
    private float speed;

    public void SetupBullet(Transform newTarget, float newSpeed, float lifeDuration)
    {
        speed = newSpeed;
        target = newTarget;

        transform.up = transform.position - target.position;

        StartCoroutine(AddWayPointCo());
        Destroy(gameObject, lifeDuration);
    }

    private void Update()
    {
        if (wayPoints.Count <= 0)
            return;

        transform.position = Vector2.MoveTowards(transform.position, wayPoints[wayIndex], speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, wayPoints[wayIndex]) < .1f)
        {
            wayIndex++;
            transform.up = transform.position - wayPoints[wayIndex];
        }
    }

    private IEnumerator AddWayPointCo()
    {
        while (true)
        {
            AddWaypoint();
            yield return new WaitForSeconds(wayPointUpdateCooldown);
        }
    }

    private void AddWaypoint()
    {
        if (target == null)
            return;

        foreach (Vector3 wayPoint in wayPoints)
        {
            if (wayPoint == target.position)
                return;
        }
        wayPoints.Add(target.position);
    }

    private void OnDestroy()
    {
        GameObject newFx = Instantiate(pickupVfx, transform.position, Quaternion.identity);
        newFx.transform.localScale = new Vector3(.6f, .6f, .6f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}

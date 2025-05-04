using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap_FallingPlatform : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private BoxCollider2D[] colliders;

    [Header("Movement Settings")]
    [SerializeField] private float speed = .75f;
    [SerializeField] private float travelDistance;

    private Vector3[] wayPoints;
    private int wayPointIndex;
    private bool canMove = false;

    [Header("Fall Settings")]
    [SerializeField] private float impactSpeed = 3f;
    [SerializeField] private float impactDuration = .1f;
    [SerializeField] private float fallDelay = .5f;
    [SerializeField] private float gravityAfterFall = 3.5f;
    [SerializeField] private float dragAfterFall = .5f;

    private float impactTimer;
    private bool impactHappened;

    [Header("Animasyon Kullan")]
    [SerializeField] private bool useAnimation = true;

    private string deactivateTrigger = "deactivate";
    private string reactivateTrigger = "reactivate";

    [Header("Reset Ayarlarý")]
    [SerializeField] private bool enableReset = true;
    [SerializeField] private float resetDelay = 3f;


    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        colliders = GetComponents<BoxCollider2D>();
    }

    private IEnumerator Start()
    {
        SetupWaypoints();
        float randomDelay = Random.Range(0, .6f);

        yield return new WaitForSeconds(randomDelay);
        canMove = true;
    }

    private void SetupWaypoints()
    {
        wayPoints = new Vector3[2];
        float yOffset = travelDistance / 2;

        wayPoints[0] = transform.position + new Vector3(0, yOffset, 0);
        wayPoints[1] = transform.position + new Vector3(0, -yOffset, 0);
    }

    private void Update()
    {
        HandleImpact();
        HandleMovement();
    }

    private void HandleMovement()
    {
        if (!canMove)
            return;

        transform.position = Vector2.MoveTowards(transform.position, wayPoints[wayPointIndex], speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, wayPoints[wayPointIndex]) < .1f)
        {
            wayPointIndex++;
            if (wayPointIndex >= wayPoints.Length)
                wayPointIndex = 0;
        }
    }

    private void HandleImpact()
    {
        if (impactTimer < 0)
            return;

        impactTimer -= Time.deltaTime;
        transform.position = Vector2.MoveTowards(transform.position, transform.position + (Vector3.down * 10), impactSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (impactHappened)
            return;

        Player player = collision.gameObject.GetComponent<Player>();

        if (player != null)
        {
            Invoke(nameof(SwitchOffPlatform), fallDelay);
            impactTimer = impactDuration;
            impactHappened = true;
        }
    }

    private void SwitchOffPlatform()
    {
        if (useAnimation && !string.IsNullOrEmpty(deactivateTrigger))
            anim.SetTrigger(deactivateTrigger);

        canMove = false;
        rb.isKinematic = false;
        rb.gravityScale = gravityAfterFall;
        rb.drag = dragAfterFall;

        foreach (BoxCollider2D collider in colliders)
            collider.enabled = false;

        // Tik iþaretliyse geri gelsin
        if (enableReset)
            Invoke(nameof(ResetPlatform), resetDelay);
    }



    private void ResetPlatform()
    {
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        rb.gravityScale = 0;
        rb.drag = 0;

        foreach (BoxCollider2D collider in colliders)
            collider.enabled = true;

        transform.position = wayPoints[0];

        if (!string.IsNullOrEmpty(deactivateTrigger))
            anim.ResetTrigger(deactivateTrigger);

        if (!string.IsNullOrEmpty(reactivateTrigger))
            anim.SetTrigger(reactivateTrigger);

        canMove = true;
        impactHappened = false;
    }
}

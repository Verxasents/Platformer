using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap_Arrow : Trap_Trampoline
{
    [Header("Additional info")]
    [SerializeField] private float cooldown;
    [SerializeField] private float rotationspeed = 120;
    [SerializeField] private bool rotationRight;
    private int direction = -1;

    [Space]
    [SerializeField] private float scaleUpSpeed = 10;
    [SerializeField] private Vector3 targetScale;


    private void Start()
    {
        transform.localScale = new Vector3(.3f, .3f, .3f);
    }


    private void Update()
    {
        HandleScaleUp();
        HandleRotation();
    }

    private void HandleScaleUp()
    {
        if (transform.localScale.x < targetScale.x)
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, scaleUpSpeed * Time.deltaTime);
    }

    private void HandleRotation()
    {
        direction = rotationRight ? -1 : 1;
        transform.Rotate(0, 0, (rotationspeed * direction) * Time.deltaTime);
    }

    private void DestroyMe()
    {
        GameObject arrowPrefab = ObjectCreator.instance.arrowPrefab;

        ObjectCreator.instance.CreateObject(arrowPrefab, transform,false, cooldown);
        
        Destroy(gameObject);

    }
}

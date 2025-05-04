using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCameraTrigger : MonoBehaviour
{
    private LevelCamera levelCamera;

    private void Awake()
    {
        levelCamera = GetComponentInParent<LevelCamera>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();

        if (player != null)
        {
            levelCamera.EnableCamera(true);
            levelCamera.SetNewTarget(player.transform);

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();

        if (player != null)
        {
            levelCamera.EnableCamera(false);
        }

    }
}

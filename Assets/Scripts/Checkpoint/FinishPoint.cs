using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishPoint : MonoBehaviour
{
    private Animator anim => GetComponent<Animator>();
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();

        if (player != null)
        {
            AudioManager.instance.PlaySFX(2);

            anim.SetTrigger("activate");
            GameManager.instance.LevelFinished();
        }
        
    }
}

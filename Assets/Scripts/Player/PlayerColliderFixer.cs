using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerColliderFixer : MonoBehaviour
{
    private void Start()
    {
        FixColliderForSkin();
    }

    private void FixColliderForSkin()
    {
        CapsuleCollider2D col = GetComponent<CapsuleCollider2D>();

        if (col == null)
        {
            Debug.Log("Player'da CapsuleCollider2D yok!");
            return;
        }

        int skinId = SkinManager.instance.GetSkinId;

        switch (skinId)
        {
            case 0:
                // Vilen - varsayýlan ayarlarý kalsýn
                break;
            case 1:
            case 2:
            case 3:
            case 4:
                // Diðer 4 karakter
                col.offset = new Vector2(-0.25f, 0.06f);
                col.size = new Vector2(1.02f, 1.51f);

                // EnemyCheck pozisyonunu da ayarla
                Transform enemyCheck = transform.Find("EnemyCheck");
                if (enemyCheck != null)
                {
                    enemyCheck.localPosition = new Vector3(-0.25f, -0.5f, 0f); // Ayarlanabilir deðerler
                }
                break;
        }
    }
}
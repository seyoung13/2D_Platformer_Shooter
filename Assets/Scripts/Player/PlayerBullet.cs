using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("DestroyBorder"))
            gameObject.SetActive(false);

        if (collision.gameObject.CompareTag("Enemy") && gameObject.name != "rifle")
            gameObject.SetActive(false);
    }
}

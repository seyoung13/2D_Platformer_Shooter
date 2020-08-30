using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBasicBullet : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("DestroyBorder"))
            gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
            gameObject.SetActive(false);
    }
}

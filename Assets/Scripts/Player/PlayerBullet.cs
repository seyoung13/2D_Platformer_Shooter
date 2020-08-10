using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "DestroyBorder")
            gameObject.SetActive(false);

        if (collision.gameObject.tag == "Enemy" && gameObject.name != "rifle")
            gameObject.SetActive(false);
    }
}

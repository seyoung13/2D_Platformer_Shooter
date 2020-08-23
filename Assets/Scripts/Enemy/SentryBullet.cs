using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentryBullet : MonoBehaviour
{ 
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("DestroyBorder"))
            gameObject.SetActive(false);

        if (collision.gameObject.CompareTag("Player"))
        {
            int damage = GameObject.Find("Sentry").GetComponent<Sentry>().sentry_gun.damage;
            GameObject.Find("Player").GetComponent<KeyboardEvent>().SendMessage("BeDamaged", damage);
            gameObject.SetActive(false);
        }
       
    }
}

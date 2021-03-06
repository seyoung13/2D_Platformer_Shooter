﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentryBullet : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {   //적이 사라지고 맞으면 null reference
            int damage = GameObject.Find("Sentry").GetComponent<Sentry>().sentry_gun.damage;
            GameObject.Find("Player").GetComponent<Player>().SendMessage("BeDamaged", damage);
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("DestroyBorder"))
            gameObject.SetActive(false);
    }
}

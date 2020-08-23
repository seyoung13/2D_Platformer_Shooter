using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindEnemy : MonoBehaviour
{
    private Color last_color;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (gameObject.name == "SentryVision")
            {
                Sentry sentry = gameObject.transform.GetComponentInParent<Sentry>();
                sentry.is_find_player = true;
                last_color = sentry.sprite.color;
                sentry.sprite.color = Color.yellow;        
            }
                
        }
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (gameObject.name == "SentryVision")
            {
                Sentry sentry = gameObject.transform.GetComponentInParent<Sentry>();
                if (sentry != null)
                {
                    sentry.is_find_player = false;
                    sentry.sprite.color = last_color;
                }
            }

        }
    }
}

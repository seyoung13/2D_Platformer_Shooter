using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerExplosiveBullet : MonoBehaviour
{
    private ObjectManager object_manager;
    private SoundManager sound_player;
    private void Start()
    {
        object_manager = GameObject.Find("ObjectManager").GetComponent<ObjectManager>();
        sound_player = SoundManager.sound_player;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("DestroyBorder"))
            gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            GameObject explosion_effect = object_manager.MakeObject("ExplosionEffect");
            explosion_effect.transform.position = transform.position;
            sound_player.PlaySFX("Explosion");
            gameObject.SetActive(false);
        }
    }

}

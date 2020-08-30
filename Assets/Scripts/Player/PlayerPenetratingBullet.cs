using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPenetratingBullet : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("DestroyBorder"))
            gameObject.SetActive(false);
    }
}

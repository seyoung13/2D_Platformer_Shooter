using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionOrigin : MonoBehaviour
{   
    private float explosion_duration;

    private void OnEnable()
    {
        explosion_duration = 0;
    }

    void Update()
    {
        Debug.Log(transform.position);
        explosion_duration += Time.deltaTime;

        if (explosion_duration > 2.5f)
        {
            explosion_duration = 0;
            gameObject.SetActive(false);
        }
    }
}

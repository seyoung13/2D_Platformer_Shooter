using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public int explosion_order;

    private float explosion_start_count, max_explosion_scale, color_alpha;
    private Vector3 original_explosion_scale;
    private Vector3 explosion_origin_point;
    private SpriteRenderer sprite;

    private void Start()
    {
        original_explosion_scale = transform.localScale;
        sprite = gameObject.GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        explosion_start_count = 0;
        max_explosion_scale = Random.Range(2.0f, 4.0f);
        explosion_origin_point = gameObject.GetComponentInParent<Transform>().position;
        
        transform.position = transform.parent.transform.position +
            new Vector3(Random.Range(-2.0f, 2.0f), Random.Range(-2.0f, 2.0f), 0);
        transform.localScale = original_explosion_scale;

        color_alpha = 1.0f;
    }

    void Update()
    {
        sprite.color = new Color(1.0f, 1.0f, 1.0f, color_alpha);
        explosion_start_count += 0.2f * Time.deltaTime;

        if (explosion_start_count < explosion_order * Time.deltaTime)
            return;

        if (transform.localScale.x < max_explosion_scale)
            transform.localScale += new Vector3(8.0f, 8.0f, 0) * Time.deltaTime;
        else
        { 
            transform.localScale += new Vector3(1.0f, 1.0f, 0) * Time.deltaTime;
            if (color_alpha >= 0)
                color_alpha -= 0.5f * Time.deltaTime;
        }
    }
}

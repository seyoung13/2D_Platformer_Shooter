using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyBorder : MonoBehaviour
{
    public Transform camera_pos;
    private GameObject left, top, right, bot;

    void Start()
    {
        left = transform.Find("Left").gameObject;
        right = transform.Find("Right").gameObject;

        top = transform.Find("Top").gameObject;
        bot = transform.Find("Bot").gameObject;
    }

    void Update()
    {
        left.transform.position = new Vector2(camera_pos.transform.position.x - 22.0f, camera_pos.transform.position.y);
        right.transform.position = new Vector2(camera_pos.transform.position.x + 22.0f, camera_pos.transform.position.y);

        top.transform.position = new Vector2(camera_pos.transform.position.x, camera_pos.transform.position.y + 12.0f);
        bot.transform.position = new Vector2(camera_pos.transform.position.x, camera_pos.transform.position.y - 12.0f);
    }
}

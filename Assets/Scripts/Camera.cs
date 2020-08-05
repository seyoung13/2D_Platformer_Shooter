using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    public Transform player_pos;
    public UnityEngine.Camera camera_attribute;

    private void LateUpdate()
    {
        float view_height = 2 * camera_attribute.orthographicSize;
        float view_width = view_height * camera_attribute.aspect;

        Vector3 temp = transform.position;
        //카메라가 지형 밖으로 벗어나지 않게 함
        if (player_pos.position.x - view_width / 2 > -16.0f)
        {
            temp.x = player_pos.position.x;
            transform.position = temp;
        }
            if(player_pos.position.y - view_height/2 > -8.0f)
        {
            temp.y = player_pos.position.y;
            transform.position = temp;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardEvent : MonoBehaviour
{
    public SpriteRenderer sprite;
    public Collider2D tilemap_collider;

    private float speed = 5.0f, jumping_power = 20.0f;
    private bool is_jumping = true;
    private float collider_height, collider_width;
    private Vector2 move_direction, gun_direction;
    private Vector3 left_chest, right_chest, left_foot, right_foot;
    private RaycastHit2D left_chest_lower_ray, right_chest_lower_ray, left_chest_ray, left_foot_ray,
        right_chest_ray, right_foot_ray;
    private Color original_color;

    private void Start()
    {
        original_color = sprite.color;
        collider_width = GetComponent<Collider2D>().bounds.size.x;
        collider_height = GetComponent<Collider2D>().bounds.size.y;
    }
    
    private void Update()
    {
        InputKey();
        if (is_jumping)
            sprite.color = Color.green;
        else
            sprite.color = original_color;
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void InputKey()
    {
        //점프
        if (Input.GetButtonDown("Jump") && !is_jumping)
        {
            is_jumping = true;
            move_direction.y = jumping_power;
        }
    }

    private void Move()
    {
        MakeRay();

        if (left_chest_lower_ray.collider != null && right_chest_lower_ray.collider != null)
        {
            //착지 판정
            if ((left_chest_lower_ray.distance < collider_height / 2 || 
                right_chest_lower_ray.distance < collider_height / 2)
                && move_direction.y < 0)
            { 
                is_jumping = false;
                //착지 후 바닥을 뚫을 경우 y축 위치 보정
                transform.position = new Vector2(transform.position.x,
                    (left_chest_lower_ray.point.y + right_chest_lower_ray.point.y) / 2 
                    + collider_height / 2);
            }

            /*
            if (left_chest_lower_ray.collider != null)
                Debug.Log("LEFT: "+left_chest_lower_ray.point.ToString());
            if (right_chest_lower_ray.collider != null)
                Debug.Log("RIGHT: "+right_chest_lower_ray.point.ToString());
            */
        }
        else
        {
            is_jumping = true;
        }

        move_direction.x = Input.GetAxisRaw("Horizontal");

        if (is_jumping)
        {
            if (move_direction.y > -0.5 * jumping_power)
                move_direction.y -= 0.04f * jumping_power;
        }
        else if (!is_jumping)
        {   
            //경사 오르기
            //좌우로 ray를 쏴서 경사각을 구함
            if (move_direction.x > 0)
            {
                if (right_chest_ray.collider != null)
                    move_direction.y = right_chest_ray.point.y - right_foot_ray.point.y;
                else
                    move_direction.y = right_chest_lower_ray.point.y - left_chest_lower_ray.point.y;
            }
            else if (move_direction.x < 0)
            {   
                if (left_chest_ray.collider != null)
                    move_direction.y = left_chest_ray.point.y - left_foot_ray.point.y;
                else
                    move_direction.y = left_chest_lower_ray.point.y - right_chest_lower_ray.point.y;
            }
            //x축 이동이 너무 빠르면 y축 위치 보정
            transform.position = new Vector2(transform.position.x,
                (left_chest_lower_ray.point.y + right_chest_lower_ray.point.y) / 2
                + collider_height / 2);
        }

        transform.Translate(new Vector3(move_direction.x * speed, move_direction.y, 0.0f) * Time.deltaTime);
    }

    private void MakeRay()
    {
        //모서리에 걸쳐 있을때도 착지 판정을 하도록 collider 좌우에서 ray를 쏨
        //좌우 중앙
        left_chest = new Vector3(transform.position.x - collider_width / 2,
            transform.position.y, transform.position.z);
        right_chest = new Vector3(transform.position.x + collider_width / 2,
            transform.position.y, transform.position.z);
        //좌우 발
        left_foot = new Vector3(transform.position.x - collider_width / 2,
            transform.position.y - collider_height / 2, transform.position.z);
        right_foot = new Vector3(transform.position.x + collider_width / 2,
            transform.position.y - collider_height / 2, transform.position.z);

        /*
        Debug.DrawRay(left_chest, Vector3.down, new Color(1.0f, 0.0f, 0.0f));
        Debug.DrawRay(right_chest, Vector3.down, new Color(1.0f, 0.0f, 0.0f));

        Debug.DrawRay(left_chest, Vector3.left, new Color(1.0f, 0.0f, 0.0f));
        Debug.DrawRay(right_chest, Vector3.right, new Color(1.0f, 0.0f, 0.0f));
        Debug.DrawRay(left_foot, Vector3.left, new Color(1.0f, 0.0f, 0.0f));
        Debug.DrawRay(right_foot, Vector3.right, new Color(1.0f, 0.0f, 0.0f));
        */

        //아래로 쏘는 레이
        left_chest_lower_ray = Physics2D.Raycast(
            left_chest, Vector3.down, collider_height, LayerMask.GetMask("Platform"));
        right_chest_lower_ray = Physics2D.Raycast(
            right_chest, Vector3.down, collider_height, LayerMask.GetMask("Platform"));
        //좌우로 쏘는 ray
        left_chest_ray = Physics2D.Raycast(
            left_chest, Vector3.left, collider_width, LayerMask.GetMask("Platform"));
        left_foot_ray = Physics2D.Raycast(
            left_foot, Vector3.left, collider_width, LayerMask.GetMask("Platform"));
        right_chest_ray = Physics2D.Raycast(
            left_chest, Vector3.right, collider_width, LayerMask.GetMask("Platform"));
        right_foot_ray = Physics2D.Raycast(
            left_foot, Vector3.right, collider_width, LayerMask.GetMask("Platform"));
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardEvent : MonoBehaviour
{
    public Rigidbody2D rigid;
    public SpriteRenderer sprite;
    public Collider2D tilemap_collider;

    private float max_speed = 5.0f;
    private float jumping_power = 20.0f;
    private bool is_jumping = true;
    private Vector2 move_direction, gun_direction;
    private float collider_height, collider_width;

    private void Start()
    {
        collider_width = GetComponent<Collider2D>().bounds.size.x;
        collider_height = GetComponent<Collider2D>().bounds.size.y;
    }
    
    private void Update()
    {
        InputKey();
        if (is_jumping)
            sprite.color = Color.red;
        else
            sprite.color = Color.green;
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
            //rigid.AddForce(Vector2.up * jumping_power, ForceMode2D.Impulse);
        }
    }

    private void Move()
    {
        //모서리에 걸쳐 있을때도 착지 판정을 하도록 collider 좌우에서 ray를 쏨
        //좌우 중앙
        Vector3 player_leftside_mid = new Vector3(transform.position.x - collider_width / 2, 
            transform.position.y, transform.position.z);
        Vector3 player_rightside_mid = new Vector3(transform.position.x + collider_width / 2,
            transform.position.y, transform.position.z);
        //좌우 발
        Vector3 player_leftside_foot = new Vector3(transform.position.x - collider_width / 2, 
            transform.position.y - collider_height / 2, transform.position.z);
        Vector3 player_rightside_foot = new Vector3(transform.position.x + collider_width / 2, 
            transform.position.y - collider_height / 2, transform.position.z);

        //Debug.DrawRay(player_leftside_mid, Vector3.down, new Color(1.0f, 0.0f, 0.0f));
        //Debug.DrawRay(player_rightside_mid, Vector3.down, new Color(1.0f, 0.0f, 0.0f));

        //Debug.DrawRay(player_leftside_mid, Vector3.left, new Color(1.0f, 0.0f, 0.0f));
        //Debug.DrawRay(player_rightside_mid, Vector3.right, new Color(1.0f, 0.0f, 0.0f));
        //Debug.DrawRay(player_leftside_foot, Vector3.left, new Color(1.0f, 0.0f, 0.0f));
        //Debug.DrawRay(player_rightside_foot, Vector3.right, new Color(1.0f, 0.0f, 0.0f));

        //아래로 쏘는 레이
        RaycastHit2D leftside_down_raycast = Physics2D.Raycast(
            player_leftside_mid, Vector3.down, collider_height, LayerMask.GetMask("Platform"));
        RaycastHit2D rightside_down_raycast = Physics2D.Raycast(
            player_rightside_mid, Vector3.down, collider_height, LayerMask.GetMask("Platform"));

        if (leftside_down_raycast.collider != null && rightside_down_raycast.collider != null)
        {
            //착지 판정
            if ((leftside_down_raycast.distance < collider_height / 2 || 
                rightside_down_raycast.distance < collider_height / 2)
                && move_direction.y < 0)
            { 
                is_jumping = false;
                //착지 후 바닥을 뚫을 경우 y축 위치 보정
                transform.position = new Vector2(transform.position.x,
                    (leftside_down_raycast.point.y + rightside_down_raycast.point.y) / 2 
                    + collider_height / 2);
            }

           // if (leftside_down_raycast.collider != null)
               // Debug.Log("LEFT: "+leftside_down_raycast.point.ToString());
           // if (rightside_down_raycast.collider != null)
                //Debug.Log("RIGHT: "+rightside_down_raycast.point.ToString());
        }
        else
        {
            is_jumping = true;
        }

        //좌우로 쏘는 ray
        RaycastHit2D leftside_mid_raycast = Physics2D.Raycast(
            player_leftside_mid, Vector3.left, collider_width, LayerMask.GetMask("Platform"));
        RaycastHit2D leftside_foot_raycast = Physics2D.Raycast(
            player_leftside_foot, Vector3.left, collider_width, LayerMask.GetMask("Platform"));
        RaycastHit2D rightside_mid_raycast = Physics2D.Raycast(
            player_leftside_mid, Vector3.right, collider_width, LayerMask.GetMask("Platform"));
        RaycastHit2D rightside_foot_raycast = Physics2D.Raycast(
            player_leftside_foot, Vector3.right, collider_width, LayerMask.GetMask("Platform"));

        move_direction.x = Input.GetAxisRaw("Horizontal") * max_speed;

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
                if (rightside_mid_raycast.collider != null)
                    move_direction.y = rightside_mid_raycast.point.y - rightside_foot_raycast.point.y;
                else
                    move_direction.y = rightside_down_raycast.point.y - leftside_down_raycast.point.y;

                //x축 이동이 너무 빠르면 y축 위치 보정
                transform.position = new Vector2(transform.position.x,
                    (leftside_down_raycast.point.y + rightside_down_raycast.point.y) / 2
                    + collider_height / 2);
            }
            else if (move_direction.x < 0)
            {   
                if (leftside_mid_raycast.collider != null)
                    move_direction.y = leftside_mid_raycast.point.y - leftside_foot_raycast.point.y;
                else
                    move_direction.y = leftside_down_raycast.point.y - rightside_down_raycast.point.y;

                //x축 이동이 너무 빠르면 y축 위치 보정
                transform.position = new Vector2(transform.position.x,
                    (leftside_down_raycast.point.y + rightside_down_raycast.point.y) / 2
                    + collider_height / 2);
            }
        }

        transform.Translate(move_direction * Time.deltaTime);
    }
}

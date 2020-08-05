using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardEvent : MonoBehaviour
{
    public Rigidbody2D rigid;
    public SpriteRenderer sprite;

    private float max_speed = 5.0f;
    private float jumping_power = 15.0f;
    private bool is_jumping = false;

    private void Update()
    {
        InputKey();
        if (is_jumping)
            sprite.color = Color.red;
        else
            sprite.color = Color.green;
    }

    void FixedUpdate()
    {
        Move();
    }

    private void InputKey()
    {
        //좌우 이동 키를 놓았을 때 멈추기
        if (Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x, rigid.velocity.y);
        }

        //점프
        if (Input.GetButtonDown("Jump") && !is_jumping)
        {
            is_jumping = true;
            rigid.AddForce(Vector2.up * jumping_power, ForceMode2D.Impulse);
        }
    }

    private void Move()
    {
        //좌우 이동
        float horizontal = Input.GetAxisRaw("Horizontal");
        rigid.AddForce(Vector2.right * horizontal * 4.0f, ForceMode2D.Impulse);
        if (horizontal != 0 || is_jumping)
            rigid.gravityScale = 2;
        else if (horizontal == 0 && !is_jumping)
            rigid.gravityScale = 0;

        //최대 속도 제한
        if (rigid.velocity.x > max_speed)
            rigid.velocity = new Vector2(max_speed, rigid.velocity.y);
        else if (rigid.velocity.x < -max_speed)
            rigid.velocity = new Vector2(-max_speed, rigid.velocity.y);

        //모서리에 걸쳐 있을때도 착지 판정을 하도록 collider 좌우에서 ray를 쏨
        Vector3 player_collider_leftside_pos = new Vector3(transform.position.x - GetComponent<Collider2D>().bounds.size.x/2,
            transform.position.y, transform.position.z);
        Vector3 player_collider_rightside_pos = new Vector3(transform.position.x + GetComponent<Collider2D>().bounds.size.x/2,
            transform.position.y, transform.position.z);

        //Debug.DrawRay(player_collider_leftside, Vector3.down, new Color(1.0f, 0.0f, 0.0f));
        //Debug.DrawRay(player_collider_rightside, Vector3.down, new Color(1.0f, 0.0f, 0.0f));

        RaycastHit2D player_leftside_raycast = Physics2D.Raycast(player_collider_leftside_pos, Vector3.down, 1, LayerMask.GetMask("Platform"));
        RaycastHit2D player_rightside_raycast = Physics2D.Raycast(player_collider_rightside_pos, Vector3.down, 1, LayerMask.GetMask("Platform"));

        if (player_leftside_raycast.collider != null || player_rightside_raycast.collider != null)
        {
            if ((player_leftside_raycast.distance < 0.95f || player_rightside_raycast.distance < 0.95f)
                && rigid.velocity.y < 0)
                is_jumping = false;
        }
        else
        {   //점프 키를 안누르고 떨어지는 것도 점프로 취급
            if (rigid.velocity.y < 0)
                is_jumping = true;
        }
    }
}

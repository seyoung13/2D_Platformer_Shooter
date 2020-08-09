using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sentry : MonoBehaviour
{
    public float patrol_distance, patrol_time;
    public Transform player_pos;
    public SpriteRenderer sprite;

    private bool is_find_player = false;
    private float collider_height, collider_width, accumulated_distance;
    private float last_direction_x = 1.0f, speed = 2.0f, fire_delay = 3.0f;
    private Vector2 move_direction = new Vector2(-1.0f, 0.0f);
    private Vector3 patrol_start_point, left_chest, right_chest, left_foot, right_foot;
    private RaycastHit2D left_chest_lower_ray, right_chest_lower_ray, left_chest_ray, left_foot_ray,
        right_chest_ray, right_foot_ray;
    private Color last_color;

    private int hp = 50;

    private void Awake()
    {   
        Invoke("Patrol", patrol_time);

        last_color = sprite.color;

    }

    private void Start()
    {
        collider_width = GetComponent<Collider2D>().bounds.size.x;
        collider_height = GetComponent<Collider2D>().bounds.size.y;
        patrol_start_point = transform.position;
    }

    private void FixedUpdate()
    {
        Move();
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
    private void Move()
    {
        MakeRay();

        transform.position = new Vector2(transform.position.x,
                    (left_chest_lower_ray.point.y + right_chest_lower_ray.point.y) / 2
                    + collider_height / 2);

        if (move_direction.x > 0)
        {
            if (right_chest_ray.collider != null)
                move_direction.y = right_chest_ray.point.y - right_foot_ray.point.y;
            else
                move_direction.y = right_chest_lower_ray.point.y - left_chest_lower_ray.point.y;

            //x축 이동이 너무 빠르면 y축 위치 보정
            transform.position = new Vector2(transform.position.x,
                (left_chest_lower_ray.point.y + right_chest_lower_ray.point.y) / 2
                + collider_height / 2);
        }
        else if (move_direction.x < 0)
        {
            if (left_chest_ray.collider != null)
                move_direction.y = left_chest_ray.point.y - left_foot_ray.point.y;
            else
                move_direction.y = left_chest_lower_ray.point.y - right_chest_lower_ray.point.y;

            //x축 이동이 너무 빠르면 y축 위치 보정
            transform.position = new Vector2(transform.position.x,
                (left_chest_lower_ray.point.y + right_chest_lower_ray.point.y) / 2
                + collider_height / 2);
        }

        //순찰 거리 제한
        if (System.Math.Abs(transform.position.x - patrol_start_point.x) > patrol_distance)
        {
            if (transform.position.x > patrol_start_point.x)
                move_direction.x = -1;
            else
                move_direction.x = 1;
            last_direction_x = move_direction.x;
        }

        transform.Translate(new Vector3(move_direction.x * speed, 0.0f, 0.0f) * Time.deltaTime);
    }

    private void Patrol()
    {
        int next_action = UnityEngine.Random.Range(0, 4);
        
        //휴식 또는 순찰
        if (next_action == 0 && move_direction.x != 0)
        {
            last_direction_x = move_direction.x;
            move_direction.x = 0;
        }
        else
        {
            move_direction.x = last_direction_x;
        }

        Invoke("Patrol", patrol_time);
    }

    private void Chase()
    {
        
    }

    private void BeDamaged(int damage)
    {
        hp -= damage;

        Invoke("ReturnColor", 0.1f);

        if (hp <= 0)
            gameObject.SetActive(false);
    }

    private void ReturnColor()
    {
        sprite.color = last_color;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "PlayerBullet")
        {
            BeDamaged(GameObject.Find("Player").GetComponent<MouseEvent>().player_weapon.damage);
        }
    }

}

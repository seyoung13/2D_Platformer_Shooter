using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardEvent : MonoBehaviour
{
    public int hp;
    public float invincible_time;
    public SpriteRenderer sprite;

    private float collider_height, collider_width;
    private float run_speed, jumping_power = 10.0f;
    private bool is_jumping, is_left_blocked, is_left_descent, is_right_blocked, is_right_descent;
    private Vector2 move_direction, face_direction;
    private Vector3 left_chest, right_chest, center_chest, center_foot;
    private RaycastHit2D horizontal_left_chest_ray, horizontal_left_foot_ray, horizontal_right_chest_ray, 
        horizontal_right_foot_ray, vertical_left_chest_ray, vertical_right_chest_ray;
    private Color original_color, last_color, invincible_color;

    private void Start()
    {
        is_jumping = true;
        original_color = sprite.color;
        invincible_color = Color.gray;
        move_direction = new Vector2(0.0f, -1.0f);
        collider_width = GetComponent<Collider2D>().bounds.size.x;
        collider_height = GetComponent<Collider2D>().bounds.size.y;
    }
    
    private void Update()
    {
        InputKey();
        if (sprite.color != invincible_color)
        {
            if (is_jumping)
                sprite.color = Color.green;
            else
                sprite.color = original_color;
        }
    }

    private void FixedUpdate()
    {
        MakeRay();
        DetectPlatform();
        DetectWall();

        Move();
    }

    private void InputKey()
    {
        move_direction.x = Input.GetAxisRaw("Horizontal");

        //점프
        if (Input.GetButtonDown("Jump") && !is_jumping)
        {
            move_direction.y = 1;
            is_jumping = true;
            jumping_power = 10.0f;
            Invoke("Descent", 0.75f);
        }
    }
    private void Move()
    {
        if ((move_direction.x < 0 && is_left_blocked) || (move_direction.x > 0 && is_right_blocked))
        {
            run_speed = 0;
        }
        else
            run_speed = 5.0f;

        float delta_x = 1, delta_y = 0;
        //이동 로직
        if (is_jumping)
        {
            if (move_direction.y == 0)
                move_direction.y = -1;

            if (move_direction.y > 0 && jumping_power > 0.0f)
                jumping_power -= 0.2f;
            else if (move_direction.y <0 && jumping_power < 10.0f)
                jumping_power += 0.2f;
        }
        else if (!is_jumping)
        {
            move_direction.y = 0;
        }

        if (move_direction.x < 0 && !is_left_blocked && !is_jumping)
        {
            if (is_left_descent)
                delta_y = vertical_left_chest_ray.point.y - vertical_right_chest_ray.point.y;
            else
                delta_y = 0;
        }
        else if (move_direction.x > 0 && !is_right_blocked && !is_jumping)
        {
            if (is_right_descent)
                delta_y = vertical_right_chest_ray.point.y - vertical_left_chest_ray.point.y;
            else
                delta_y = 0;
        }

        if (!is_jumping)
            transform.Translate(new Vector3(move_direction.x * delta_x * run_speed,  delta_y * run_speed, 0.0f) *
            Time.deltaTime);
        else
            transform.Translate((new Vector3(move_direction.x * delta_x * run_speed, move_direction.y * jumping_power, 0.0f) *
            Time.deltaTime));
    }

    private void MakeRay()
    {
        left_chest = new Vector3(transform.position.x - collider_width / 2,
            transform.position.y, transform.position.z);
        right_chest = new Vector3(transform.position.x + collider_width / 2,
            transform.position.y, transform.position.z);
        center_chest = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        center_foot = new Vector3(transform.position.x,
            transform.position.y - collider_height / 2 + 0.1f, transform.position.z);

        Debug.DrawRay(left_chest, Vector3.down, new Color(0.0f, 1.0f, 0.0f));
        Debug.DrawRay(right_chest, Vector3.down, new Color(0.0f, 1.0f, 0.0f));

        vertical_left_chest_ray = Physics2D.Raycast(left_chest, 
            Vector3.down, collider_height * 2, LayerMask.GetMask("Platform"));
        vertical_right_chest_ray = Physics2D.Raycast(right_chest, 
            Vector3.down, collider_height * 2, LayerMask.GetMask("Platform"));

        horizontal_left_chest_ray = Physics2D.Raycast(
            center_chest, Vector3.left, collider_width * 2, LayerMask.GetMask("Platform"));
        horizontal_left_foot_ray = Physics2D.Raycast(
            center_foot, Vector3.left, collider_width * 2, LayerMask.GetMask("Platform"));
        horizontal_right_chest_ray = Physics2D.Raycast(
            center_chest, Vector3.right, collider_width * 2, LayerMask.GetMask("Platform"));
        horizontal_right_foot_ray = Physics2D.Raycast(
            center_foot, Vector3.right, collider_width * 2, LayerMask.GetMask("Platform"));

        Debug.DrawRay(center_chest, Vector3.left, new Color(1.0f, 0.0f, 0.0f));
        Debug.DrawRay(center_foot, Vector3.left, new Color(1.0f, 0.0f, 0.0f));
        Debug.DrawRay(center_chest, Vector3.right, new Color(1.0f, 0.0f, 0.0f));
        Debug.DrawRay(center_foot, Vector3.right, new Color(1.0f, 0.0f, 0.0f));
    }

    private void DetectPlatform()
    {
        if (move_direction.y > 0)
            return;

        if (vertical_left_chest_ray.collider != null && vertical_right_chest_ray.collider != null)
        {
            if (vertical_left_chest_ray.distance <= collider_height / 2 + 0.1f ||
                vertical_right_chest_ray.distance <= collider_height / 2 + 0.1f)
                is_jumping = false;
            else if (vertical_left_chest_ray.distance > collider_height / 2 + 0.1f &&
               vertical_right_chest_ray.distance > collider_height / 2 + 0.1f)
                is_jumping = true;
            //내리막길이 60도 이하일 땐 추락 대신 걸어 내려감
            else if (Mathf.Abs(vertical_left_chest_ray.point.y - vertical_right_chest_ray.point.y) >
                    Mathf.Tan(60.0f * Mathf.PI / 180.0f))
                is_jumping = true;
        }
        else if (vertical_left_chest_ray.collider != null && vertical_right_chest_ray.collider == null)
        {
            if (vertical_left_chest_ray.distance <= collider_height / 2 + 0.1f)
                is_jumping = false;
            else if (vertical_left_chest_ray.distance > collider_height / 2 + 0.1f)
                is_jumping = true;
        }
        else if (vertical_left_chest_ray.collider == null && vertical_right_chest_ray.collider != null)
        {
            if (vertical_right_chest_ray.distance <= collider_height / 2 + 0.1f)
                is_jumping = false;
            else if (vertical_right_chest_ray.distance > collider_height / 2 + 0.1f)
                is_jumping = true;
        }
        else
            is_jumping = true;
    }

    private void DetectWall()
    {
        if (horizontal_left_chest_ray.collider != null && horizontal_left_foot_ray.collider != null)
        {
            if (horizontal_left_foot_ray.distance <= collider_width / 2 + 0.1f)
            {
                if (horizontal_left_chest_ray.point.x - horizontal_left_foot_ray.point.x == 0)
                    is_left_blocked = true;
                //경사가 60 이상이면 못 올라감
                else if (Mathf.Abs(horizontal_left_chest_ray.point.y - horizontal_left_foot_ray.point.y) /
                    Mathf.Abs(horizontal_left_chest_ray.point.x - horizontal_left_foot_ray.point.x) >=
                    Mathf.Tan(60.0f * Mathf.PI / 180.0f))
                    is_left_blocked = true;
                else
                {
                    is_left_blocked = false;
                }
            }
            else
                is_left_blocked = false;
        }
        else if (horizontal_left_chest_ray.collider == null && horizontal_left_foot_ray.collider == null)
            is_left_blocked = false;

        if (horizontal_right_chest_ray.collider != null && horizontal_right_foot_ray.collider != null)
        {
            if (horizontal_right_foot_ray.distance <= collider_width / 2 + 0.1f)
            {
                if (horizontal_right_chest_ray.point.x - horizontal_right_foot_ray.point.x == 0)
                    is_right_blocked = true;
                //경사가 60 이상이면 못 올라감
                else if (Mathf.Abs(horizontal_right_chest_ray.point.y - horizontal_right_foot_ray.point.y) /
                    Mathf.Abs(horizontal_right_chest_ray.point.x - horizontal_right_foot_ray.point.x) >=
                    Mathf.Tan(60.0f * Mathf.PI / 180.0f))
                    is_right_blocked = true;
                else
                {
                    is_right_blocked = false;
                }
            }
            else
                is_right_blocked = false;
        }
        else if (horizontal_right_chest_ray.collider == null && horizontal_right_foot_ray.collider == null)
            is_right_blocked = false;

        if (is_left_blocked)
            is_left_descent = false;
        else
        {
            if (Mathf.Abs(vertical_left_chest_ray.point.y - vertical_right_chest_ray.point.y) >=
                    Mathf.Tan(60.0f * Mathf.PI / 180.0f))
                is_left_descent = false;
            else
                is_left_descent = true;

        }

        if (is_right_blocked)
            is_right_descent = false;
        else
        {
            if (Mathf.Abs(vertical_left_chest_ray.point.y - vertical_right_chest_ray.point.y) >=
                    Mathf.Tan(60.0f * Mathf.PI / 180.0f))
                is_right_descent = false;
            else
                is_right_descent = true;

        }
    }

    private void Descent()
    {
        move_direction.y = -1;
    }

    public void BeDamaged(int damage)
    {
        hp -= damage;

        last_color = sprite.color;
        sprite.color = invincible_color;
        gameObject.tag = "Invincible";
        Invoke("ReturnColor", invincible_time);

        if (hp <= 0)
        {
            gameObject.SetActive(false);
            hp = 100;
        }
    }

    private void ReturnColor()
    {
        sprite.color = last_color;
        gameObject.tag = "Player";
    }
}

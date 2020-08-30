using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardEvent : MonoBehaviour
{
    public Weapon[] weapons;

    private Player player;
    private float collider_height, collider_width;
    private Vector2 move_direction;
    private Vector3 left_chest, right_chest, center_chest, center_foot;
    private RaycastHit2D horizontal_left_chest_ray, horizontal_left_foot_ray, horizontal_right_chest_ray, 
        horizontal_right_foot_ray, vertical_left_chest_ray, vertical_right_chest_ray;
    

    private void Start()
    {
        player = GetComponent<Player>();

        move_direction = new Vector2(0.0f, -1.0f);
        collider_width = GetComponent<Collider2D>().bounds.size.x;
        collider_height = GetComponent<Collider2D>().bounds.size.y;
    }
    
    private void Update()
    {
        InputKey();
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
        if (Input.GetButtonDown("Jump") && !player.is_jumping)
        {
            move_direction.y = 1;
            player.is_jumping = true;
            player.jumping_power = 10.0f;
            Invoke("Descent", 0.75f);
        }

        SelectWeapon();
    }
    private void Move()
    {
        if ((move_direction.x < 0 && player.is_left_blocked) || (move_direction.x > 0 && player.is_right_blocked))
        {
            player.run_speed = 0;
        }
        else
            player.run_speed = 5.0f;

        float delta_x = 1, delta_y = 0;
        //이동 로직
        if (player.is_jumping)
        {
            if (move_direction.y == 0)
                move_direction.y = -1;

            if (move_direction.y > 0 && player.jumping_power > 0.0f)
                player.jumping_power -= 0.2f;
            else if (move_direction.y <0 && player.jumping_power < 10.0f)
                player.jumping_power += 0.2f;
        }
        else if (!player.is_jumping)
        {
            move_direction.y = 0;
        }

        if (move_direction.x < 0 && !player.is_left_blocked && !player.is_jumping)
        {
            if (player.is_left_descent)
                delta_y = vertical_left_chest_ray.point.y - vertical_right_chest_ray.point.y;
            else
                delta_y = 0;
        }
        else if (move_direction.x > 0 && !player.is_right_blocked && !player.is_jumping)
        {
            if (player.is_right_descent)
                delta_y = vertical_right_chest_ray.point.y - vertical_left_chest_ray.point.y;
            else
                delta_y = 0;
        }

        if (!player.is_jumping)
            transform.Translate(new Vector3(move_direction.x * delta_x * player.run_speed,  delta_y * player.run_speed, 0.0f) *
            Time.deltaTime);
        else
            transform.Translate((new Vector3(move_direction.x * delta_x * player.run_speed, move_direction.y * player.jumping_power, 0.0f) *
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
                player.is_jumping = false;
            else if (vertical_left_chest_ray.distance > collider_height / 2 + 0.1f &&
               vertical_right_chest_ray.distance > collider_height / 2 + 0.1f)
                player.is_jumping = true;
            //내리막길이 60도 이하일 땐 추락 대신 걸어 내려감
            else if (Mathf.Abs(vertical_left_chest_ray.point.y - vertical_right_chest_ray.point.y) >
                    Mathf.Tan(60.0f * Mathf.PI / 180.0f))
                player.is_jumping = true;
        }
        else if (vertical_left_chest_ray.collider != null && vertical_right_chest_ray.collider == null)
        {
            if (vertical_left_chest_ray.distance <= collider_height / 2 + 0.1f)
                player.is_jumping = false;
            else if (vertical_left_chest_ray.distance > collider_height / 2 + 0.1f)
                player.is_jumping = true;
        }
        else if (vertical_left_chest_ray.collider == null && vertical_right_chest_ray.collider != null)
        {
            if (vertical_right_chest_ray.distance <= collider_height / 2 + 0.1f)
                player.is_jumping = false;
            else if (vertical_right_chest_ray.distance > collider_height / 2 + 0.1f)
                player.is_jumping = true;
        }
        else
            player.is_jumping = true;
    }

    private void DetectWall()
    {
        if (horizontal_left_chest_ray.collider != null && horizontal_left_foot_ray.collider != null)
        {
            if (horizontal_left_foot_ray.distance <= collider_width / 2 + 0.1f)
            {
                if (horizontal_left_chest_ray.point.x - horizontal_left_foot_ray.point.x == 0)
                    player.is_left_blocked = true;
                //경사가 60 이상이면 못 올라감
                else if (Mathf.Abs(horizontal_left_chest_ray.point.y - horizontal_left_foot_ray.point.y) /
                    Mathf.Abs(horizontal_left_chest_ray.point.x - horizontal_left_foot_ray.point.x) >=
                    Mathf.Tan(60.0f * Mathf.PI / 180.0f))
                    player.is_left_blocked = true;
                else
                {
                    player.is_left_blocked = false;
                }
            }
            else
                player.is_left_blocked = false;
        }
        else if (horizontal_left_chest_ray.collider == null && horizontal_left_foot_ray.collider == null)
            player.is_left_blocked = false;

        if (horizontal_right_chest_ray.collider != null && horizontal_right_foot_ray.collider != null)
        {
            if (horizontal_right_foot_ray.distance <= collider_width / 2 + 0.1f)
            {
                if (horizontal_right_chest_ray.point.x - horizontal_right_foot_ray.point.x == 0)
                    player.is_right_blocked = true;
                //경사가 60 이상이면 못 올라감
                else if (Mathf.Abs(horizontal_right_chest_ray.point.y - horizontal_right_foot_ray.point.y) /
                    Mathf.Abs(horizontal_right_chest_ray.point.x - horizontal_right_foot_ray.point.x) >=
                    Mathf.Tan(60.0f * Mathf.PI / 180.0f))
                    player.is_right_blocked = true;
                else
                {
                    player.is_right_blocked = false;
                }
            }
            else
                player.is_right_blocked = false;
        }
        else if (horizontal_right_chest_ray.collider == null && horizontal_right_foot_ray.collider == null)
            player.is_right_blocked = false;

        if (player.is_left_blocked)
            player.is_left_descent = false;
        else
        {
            if (Mathf.Abs(vertical_left_chest_ray.point.y - vertical_right_chest_ray.point.y) >=
                    Mathf.Tan(60.0f * Mathf.PI / 180.0f))
                player.is_left_descent = false;
            else
                player.is_left_descent = true;

        }

        if (player.is_right_blocked)
            player.is_right_descent = false;
        else
        {
            if (Mathf.Abs(vertical_left_chest_ray.point.y - vertical_right_chest_ray.point.y) >=
                    Mathf.Tan(60.0f * Mathf.PI / 180.0f))
                player.is_right_descent = false;
            else
                player.is_right_descent = true;

        }
    }

    private void Descent()
    {
        move_direction.y = -1;
    }

    private void SelectWeapon()
    {
        //Handgun
        if (Input.GetKeyDown(KeyCode.Alpha1) && player.weapon != weapons[0])
        {
            player.weapon = weapons[0];
            player.final_accuracy = weapons[0].max_accuracy/2;
            player.curr_fire_delay = weapons[0].delay;
        }

        //Machinegun
        if (Input.GetKeyDown(KeyCode.Alpha2) && player.weapon != weapons[1])
        {
            player.weapon = weapons[1];
            player.final_accuracy = weapons[1].max_accuracy / 2;
            player.curr_fire_delay = weapons[1].delay;
        }

        //Rifle
        if (Input.GetKeyDown(KeyCode.Alpha3) && player.weapon != weapons[2])
        {
            player.weapon = weapons[2];
            player.final_accuracy = weapons[2].max_accuracy / 2;
            player.curr_fire_delay = weapons[2].delay;
        }

        //Shotgun
        if (Input.GetKeyDown(KeyCode.Alpha4) && player.weapon != weapons[3])
        {
            player.weapon = weapons[3];
            player.final_accuracy = weapons[3].max_accuracy / 2;
            player.curr_fire_delay = weapons[3].delay;
        }

        //Launcher
        if (Input.GetKeyDown(KeyCode.Alpha5) && player.weapon != weapons[4])
        {
            player.weapon = weapons[4];
            player.final_accuracy = weapons[4].max_accuracy / 2;
            player.curr_fire_delay = weapons[4].delay;
        }
    }
}

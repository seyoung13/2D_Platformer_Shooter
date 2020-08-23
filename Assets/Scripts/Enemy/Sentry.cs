using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sentry : MonoBehaviour
{   
    [HideInInspector] public bool is_find_player = false;
    public int hp;
    public float patrol_distance, patrol_time, max_chase_time;
    public Transform player_transform;
    public SpriteRenderer sprite;
    public Weapon sentry_gun;
    public ObjectManager object_manager;

    private float collider_height, collider_width;
    private float run_speed = 3.0f, jumping_power = 10.0f,
        curr_fire_delay = 100.0f, curr_chase_time, rest_time = 0.0f, turn_delay =0.0f;
    private bool is_jumping, is_left_blocked, is_left_descent, is_right_blocked, is_right_descent;
    private Vector2 move_direction, last_direction;
    private Vector3 patrol_start_point, next_patrol_point, left_chest, right_chest, center_chest, center_foot;
    private RaycastHit2D horizontal_left_chest_ray, horizontal_left_foot_ray, horizontal_right_chest_ray,
        horizontal_right_foot_ray, vertical_left_chest_ray, vertical_right_chest_ray;
    private Color original_color;
    private GameObject vision;

    private SelectorNode root_node;
    private SequenceNode chase_sequence, shoot_sequence, patrol_sequence;
    private LeafNode find_node, follow_node, aim_node, fire_node, get_next_pos_node, wander_node, rest_node;

    private void Awake()
    {   
        original_color = sprite.color;
    }

    private void Start()
    {
        move_direction = new Vector2(-1.0f, 0.0f);
        last_direction = move_direction;

        is_jumping = true;
        collider_width = GetComponent<Collider2D>().bounds.size.x;
        collider_height = GetComponent<Collider2D>().bounds.size.y;
        patrol_start_point = transform.position;
        next_patrol_point.x = patrol_start_point.x - patrol_distance;

        vision = transform.Find("SentryVision").gameObject;

        rest_node = new LeafNode(Rest, "Rest");
        get_next_pos_node = new LeafNode(GetNextPatrolPosition, "GetNextPosition");
        wander_node = new LeafNode(Wander, "Wander");

        patrol_sequence = new SequenceNode(new List<BehaviorTree>
        {
            get_next_pos_node, wander_node    
        });

        find_node = new LeafNode(FindPlayer, "Find");
        follow_node = new LeafNode(FollowPlayer, "Follow");
        aim_node = new LeafNode(Aim, "Stop");
        fire_node = new LeafNode(Fire, "Fire");

        shoot_sequence = new SequenceNode(new List<BehaviorTree>
        {
            aim_node, fire_node
        });

        chase_sequence = new SequenceNode(new List<BehaviorTree>
        {
            find_node, follow_node, shoot_sequence
        });

        root_node = new SelectorNode(new List<BehaviorTree>
        { 
            chase_sequence, patrol_sequence
        });


    }

    private void Update()
    {
        BulletDelayCount();
        root_node.Run();
        Debug.Log("curr_node: " + root_node.GetName());
        Debug.Log(is_find_player);
        Debug.Log(curr_chase_time);
        Debug.Log(turn_delay);

    }

    private void FixedUpdate()
    {
        MakeRay();
        DetectPlatform();
        DetectWall();
        
        Move();
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

    private void Move()
    {
        if ((move_direction.x < 0 && is_left_blocked) || (move_direction.x > 0 && is_right_blocked))
        {
            run_speed = 0;
        }
        else
            run_speed = 3.0f;

        float delta_x = 1, delta_y = 0;
        //이동 로직
        if (is_jumping)
        {
            if (move_direction.y == 0)
                move_direction.y = -1;

            if (move_direction.y > 0 && jumping_power > 0.0f)
                jumping_power -= 0.2f;
            else if (move_direction.y < 0 && jumping_power < 10.0f)
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

        vision.transform.localScale = new Vector3(last_direction.x, 1, 1);

        if (!is_jumping)
            transform.Translate(new Vector3(move_direction.x * delta_x * run_speed, delta_y * run_speed, 0.0f) *
            Time.deltaTime);
        else
            transform.Translate((new Vector3(move_direction.x * delta_x * run_speed, move_direction.y * jumping_power, 0.0f) *
            Time.deltaTime));
    }

    private NodeState FindPlayer()
    {
        curr_chase_time += Time.deltaTime;

        if (is_find_player)
        {
            curr_chase_time = 0;
            return NodeState.SUCCESS;
        }

        if (curr_chase_time > max_chase_time)
        {
            curr_chase_time = 0;
            return NodeState.FAILURE;
        }
        else 
            return NodeState.SUCCESS;
    }

    private NodeState FollowPlayer()
    {
        turn_delay += Time.deltaTime;

        if ((player_transform.position - transform.position).sqrMagnitude > 36.0f)
        {
            if (turn_delay > 1.0f)
            {
                if (player_transform.position.x < transform.position.x)
                    move_direction.x = -1;
                else
                    move_direction.x = 1;

                turn_delay = 0;
            }
            last_direction.x = move_direction.x;

            return NodeState.RUNNING;
        }
        else
            return NodeState.SUCCESS;
    }

    private NodeState Aim()
    {
        if (move_direction.x != 0)
            last_direction.x = move_direction.x;
        move_direction.x = 0;

        if (curr_fire_delay > 4.0f)
            return NodeState.SUCCESS;
        else
            return NodeState.RUNNING;
    }

    private NodeState Fire()
    {
        float total_x_accuracy = Random.Range(-sentry_gun.accuracy, sentry_gun.accuracy);
        float total_y_accuracy = Random.Range(-sentry_gun.accuracy, sentry_gun.accuracy);

        GameObject bullet = object_manager.MakeObject("SentryBullet");
        bullet.transform.position = transform.position;
        Rigidbody2D bullet_ridigbody = bullet.GetComponent<Rigidbody2D>();
        bullet_ridigbody.AddForce(new Vector2(player_transform.position.x - transform.position.x + total_x_accuracy,
            player_transform.position.y - transform.position.y + total_y_accuracy).normalized * sentry_gun.velocity,
            ForceMode2D.Impulse);

        curr_fire_delay = 0;

        return NodeState.SUCCESS;
    }

    private NodeState GetNextPatrolPosition()
    {
        Vector3 left_point = new Vector3(patrol_start_point.x - patrol_distance, 
            patrol_start_point.y, patrol_start_point.z);
        Vector3 right_point = new Vector3(patrol_start_point.x + patrol_distance, 
            patrol_start_point.y, patrol_start_point.z);

        if ((left_point - transform.position).sqrMagnitude < (right_point - transform.position).sqrMagnitude) 
        {
            next_patrol_point = right_point;
            move_direction.x = 1;
        }
        else
        {
            next_patrol_point = left_point;
            move_direction.x = -1;
        }

        last_direction.x = move_direction.x;

        return NodeState.SUCCESS;
    }

    private NodeState Wander()
    {
        if ((next_patrol_point - transform.position).sqrMagnitude < 1)
            return NodeState.SUCCESS;
        else
            return NodeState.RUNNING;
    }

    private NodeState Rest()
    {
        int rest_decision = 0;
        if (rest_time == 0)
            rest_decision = Random.Range(0, 5);

        if (rest_decision == 0)
            return NodeState.SUCCESS;

        if (move_direction.x != 0)
            last_direction.x = move_direction.x;
        move_direction.x = 0;
        rest_time += Time.deltaTime;

        if (rest_time > 2.0f)
        {
            rest_time = 0;
            move_direction.x = last_direction.x;
            return NodeState.SUCCESS;
        }
        else
            return NodeState.RUNNING;
    }


    private void BeDamaged(int damage)
    {
        hp -= damage;

        original_color = sprite.color;
        sprite.color = Color.white;
        Invoke("ReturnColor", 0.05f);

        if (hp <= 0)
        {
            gameObject.SetActive(false);
            hp = 100;
        }
    }

    private void ReturnColor()
    {
        sprite.color = original_color;
    }

    private void BulletDelayCount()
    {
        curr_fire_delay += Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerBullet"))
        {
            BeDamaged(GameObject.Find("Player").GetComponent<MouseEvent>().player_weapon.damage);
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int hp;
    public float invincible_time, run_speed, jumping_power;
    [HideInInspector] public float final_accuracy, curr_fire_delay;
    public SpriteRenderer sprite;
    public Weapon weapon;
    [HideInInspector] public bool is_jumping, is_left_blocked, is_left_descent, is_right_blocked, is_right_descent;
    [HideInInspector] public Color original_color, last_color, invincible_color;

    private void Awake()
    {
        is_jumping = true;
        original_color = sprite.color;
        invincible_color = Color.gray;

        final_accuracy = weapon.max_accuracy/2;
        curr_fire_delay = weapon.delay;

        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Stage1")
            SoundManager.sound_player.PlayBGM("Stage1");
    }

    private void Update()
    {
        if (sprite.color != invincible_color)
        {
            if (is_jumping)
                sprite.color = Color.green;
            else
                sprite.color = original_color;
        }
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

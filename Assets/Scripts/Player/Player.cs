using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public int max_hp;
    [HideInInspector] public int curr_hp;
    public float max_weapon_switch_time, invincible_time, run_speed, jumping_power;
    [HideInInspector] public float curr_weapon_switch_time, final_accuracy, curr_fire_delay, collider_height, collider_width;
    public SpriteRenderer sprite;
    public Weapon weapon;
    [HideInInspector] public bool is_jumping, is_reloading, is_switching_weapon,
        is_left_blocked, is_left_descent, is_right_blocked, is_right_descent;
    [HideInInspector] public Color original_color, last_color, invincible_color;
    [HideInInspector] public GameObject hp_bar, ammo_bar, curr_reload_bar, max_reload_bar,
        curr_switch_bar, max_switch_bar;

    private void Awake()
    {
        is_jumping = true;
        is_reloading = false;
        original_color = sprite.color;
        invincible_color = Color.gray;

        collider_width = GetComponent<Collider2D>().bounds.size.x;
        collider_height = GetComponent<Collider2D>().bounds.size.y;

        final_accuracy = weapon.max_accuracy/2;
        curr_fire_delay = weapon.delay;
        curr_weapon_switch_time = 0;

        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Stage1")
            SoundManager.sound_player.PlayBGM("Stage1");

        curr_hp = max_hp;
        hp_bar = GameObject.Find("CurrHPBar");
        ammo_bar = GameObject.Find("CurrAmmoBar");
        curr_reload_bar = GameObject.Find("CurrReloadBar");
        max_reload_bar = GameObject.Find("MaxReloadBar");
        curr_switch_bar = GameObject.Find("CurrSwitchBar");
        max_switch_bar = GameObject.Find("MaxSwitchBar");
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
        
        hp_bar.GetComponent<Image>().fillAmount = (float) curr_hp / max_hp;
        hp_bar.GetComponentInChildren<Text>().text = curr_hp.ToString() + " / " + max_hp.ToString();
        ammo_bar.GetComponent<Image>().fillAmount = (float) weapon.curr_magazine / weapon.max_magazine;
        ammo_bar.GetComponentInChildren<Text>().text = weapon.curr_magazine.ToString() + " / " + weapon.max_magazine.ToString();
    }

    public void BeDamaged(int damage)
    {
        curr_hp -= damage;

        last_color = sprite.color;
        sprite.color = invincible_color;
        gameObject.tag = "Invincible";
        Invoke("ReturnColor", invincible_time);

        if (curr_hp <= 0)
        {
            gameObject.SetActive(false);
            curr_hp = 100;
        }
    }

    private void ReturnColor()
    { 
        sprite.color = last_color;
        gameObject.tag = "Player";
    }
}

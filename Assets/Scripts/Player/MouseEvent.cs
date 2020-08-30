using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseEvent : MonoBehaviour
{
    public ObjectManager object_manager;
    public Transform top_reticle_transform, bot_reticle_transform;

    private int curr_fired_bullet;
    private Player player;
    private float mouse_radian, mouse_degree, mouse_radius, top_radian, bot_radian;
    private Vector2 mouse_distance, top_reticle_distance, bot_reticle_distance;
    private Vector3 mouse_pos;
    private SoundManager sound_player;

    private void Start()
    {
        player = GetComponent<Player>();
        curr_fired_bullet = 0;
        sound_player = SoundManager.sound_player;
    }

    private void Update()
    {
        Debug.Log(player.weapon.name);

        InputKey();
        BulletDelayCount();
        mouse_pos = UnityEngine.Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void FixedUpdate()
    {
        ReticleMove();
    }

    private void InputKey()
    {   
        if (Input.GetMouseButton(0))
        {
           Fire();
        }

        //조준
        if (Input.GetMouseButton(1) && !player.is_jumping)
        {
            if (player.final_accuracy > player.weapon.min_accuracy / 2)
                player.final_accuracy -= 50 * Time.deltaTime;
        }
        //조준 취소
        else
        {
            if (player.final_accuracy < player.weapon.max_accuracy / 2)
                player.final_accuracy += 100 * Time.deltaTime;
        }
    }

    private void Fire()
    {
        if (!Input.GetMouseButton(0))
            return;

        if (player.curr_fire_delay < player.weapon.delay)
            return;

        switch(player.weapon.name)
        {
            case "Handgun":
                FireHandgun();
                break;
            case "Machinegun":
                FireMachinegun();
                break;
            case "Rifle":
                FireRifle();
                break;
            case "Shotgun":
                FireShotgun();
                break;
            case "Launcher":
                FireLauncher();
                break;
        }
        sound_player.PlaySFX(player.weapon.name);

        player.curr_fire_delay = 0;
    }

    private void FireHandgun()
    {
        Vector2 bullet_spread = new Vector2(Random.Range(bot_reticle_distance.x, top_reticle_distance.x),
               Random.Range(bot_reticle_distance.y, top_reticle_distance.y));

        GameObject bullet = object_manager.MakeObject(player.weapon.name + "Bullet");
        bullet.transform.position = transform.position;
        Rigidbody2D bullet_ridigbody = bullet.GetComponent<Rigidbody2D>();
        bullet_ridigbody.AddForce(new Vector2(bullet_spread.x - player.transform.position.x,
            bullet_spread.y - player.transform.position.y).normalized * player.weapon.velocity,
            ForceMode2D.Impulse);
    }

    private void FireMachinegun()
    {   
        Vector2 bullet_spread = new Vector2(Random.Range(bot_reticle_distance.x, top_reticle_distance.x),
               Random.Range(bot_reticle_distance.y, top_reticle_distance.y));

        GameObject bullet = object_manager.MakeObject(player.weapon.name + "Bullet");
        bullet.transform.position = transform.position;
        Rigidbody2D bullet_ridigbody = bullet.GetComponent<Rigidbody2D>();
        bullet_ridigbody.AddForce(new Vector2(bullet_spread.x - player.transform.position.x,
            bullet_spread.y - player.transform.position.y).normalized * player.weapon.velocity,
            ForceMode2D.Impulse);

        curr_fired_bullet += 1;
        if (curr_fired_bullet < player.weapon.bullet_per_shot)
            Invoke("FireMachinegun", player.weapon.delay / player.weapon.bullet_per_shot);
        else
        {
            curr_fired_bullet = 0;
            return;
        }    
    }

    private void FireRifle()
    {
        Vector2 bullet_spread = new Vector2(Random.Range(bot_reticle_distance.x, top_reticle_distance.x),
               Random.Range(bot_reticle_distance.y, top_reticle_distance.y));

        GameObject bullet = object_manager.MakeObject(player.weapon.name + "Bullet");
        bullet.transform.position = transform.position;
        Rigidbody2D bullet_ridigbody = bullet.GetComponent<Rigidbody2D>();
        bullet_ridigbody.AddForce(new Vector2(bullet_spread.x - player.transform.position.x,
            bullet_spread.y - player.transform.position.y).normalized * player.weapon.velocity,
            ForceMode2D.Impulse);
    }

    private void FireShotgun()
    {
        for (int i = 0; i < player.weapon.bullet_per_shot; i++)
        {
            Vector2 bullet_spread = new Vector2(Random.Range(bot_reticle_distance.x, top_reticle_distance.x),
              Random.Range(bot_reticle_distance.y, top_reticle_distance.y));
            GameObject bullet = object_manager.MakeObject(player.weapon.name + "Bullet");
            bullet.transform.position = transform.position;
            Rigidbody2D bullet_ridigbody = bullet.GetComponent<Rigidbody2D>();
            bullet_ridigbody.AddForce(new Vector2(bullet_spread.x - player.transform.position.x,
                bullet_spread.y - player.transform.position.y).normalized * player.weapon.velocity,
                ForceMode2D.Impulse);
        }
    }

    private void FireLauncher()
    {
        Vector2 bullet_spread = new Vector2(Random.Range(bot_reticle_distance.x, top_reticle_distance.x),
               Random.Range(bot_reticle_distance.y, top_reticle_distance.y));

        GameObject bullet = object_manager.MakeObject(player.weapon.name + "Bullet");
        bullet.transform.position = transform.position;
        Rigidbody2D bullet_ridigbody = bullet.GetComponent<Rigidbody2D>();
        bullet_ridigbody.AddForce(new Vector2(bullet_spread.x - player.transform.position.x,
            bullet_spread.y - player.transform.position.y).normalized * player.weapon.velocity,
            ForceMode2D.Impulse);
    }

    private void BulletDelayCount()
    {
        player.curr_fire_delay += Time.deltaTime;
    }

    private void ReticleMove()
    {   
        //마우스와 플레이어 사이의 거리
        mouse_distance = new Vector2(mouse_pos.x - player.transform.position.x, 
            mouse_pos.y - player.transform.position.y);
        
        mouse_radian = Mathf.Atan2(mouse_distance.y, mouse_distance.x);
        mouse_degree = mouse_radian * Mathf.Rad2Deg;
        mouse_radius = mouse_distance.y / Mathf.Sin(mouse_radian);

        top_radian = (mouse_degree + player.final_accuracy) * Mathf.Deg2Rad;
        top_reticle_distance = new Vector2(mouse_radius * Mathf.Cos(top_radian) + player.transform.position.x,
            mouse_radius * Mathf.Sin(top_radian) + player.transform.position.y);

        bot_radian = (mouse_degree - player.final_accuracy) * Mathf.Deg2Rad;
        bot_reticle_distance = new Vector2(mouse_radius * Mathf.Cos(bot_radian) + player.transform.position.x,
            mouse_radius * Mathf.Sin(bot_radian) + player.transform.position.y);
        
        top_reticle_transform.position = new Vector3(top_reticle_distance.x, top_reticle_distance.y, top_reticle_transform.position.z);
        bot_reticle_transform.position = new Vector3(bot_reticle_distance.x , bot_reticle_distance.y, bot_reticle_transform.position.z);
        
        top_reticle_transform.rotation = Quaternion.Euler(new Vector3(0, 0, top_radian * Mathf.Rad2Deg));
        bot_reticle_transform.rotation = Quaternion.Euler(new Vector3(0, 0, bot_radian * Mathf.Rad2Deg));
    }

}

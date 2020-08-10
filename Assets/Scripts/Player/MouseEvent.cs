using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseEvent : MonoBehaviour
{ 
    public ObjectManager object_manager;
    public Transform player_pos;
    public Transform crosshair_pos;
    public Weapon player_weapon;

    private GameObject bullet;
    private float curr_fire_delay;
    private Vector3 mouse_pos;

    private void Awake()
    {
        curr_fire_delay = 100.0f;
    }

    private void Update()
    {
        mouse_pos = UnityEngine.Camera.main.ScreenToWorldPoint(Input.mousePosition);

        InputKey();
        BulletDelayCount();
        CrosshairMove();
    }

    private void InputKey()
    {   
        //발사
        if (Input.GetMouseButton(0))
        {
           Fire();
        }

        //조준
        if (Input.GetMouseButton(1))
        {
            Aim();
        }
        //조준 취소
        else
        {

        }
    }
    
    private void Fire()
    {
        //플레이어 무기 확인
        switch (player_weapon.name)
        {
            case "Pistol":
                this.bullet = GameObject.Find("pistol_bullet");
                break;
        }

        if (!Input.GetMouseButton(0))
            return;

        if (curr_fire_delay < player_weapon.delay)
            return;

        float total_x_accuracy = Random.Range(-player_weapon.accuracy, player_weapon.accuracy);
        float total_y_accuracy = Random.Range(-player_weapon.accuracy, player_weapon.accuracy);

        //총알 풀링
        GameObject bullet = object_manager.MakeObject(player_weapon.name+"Bullet");
        bullet.transform.position = transform.position;
        Rigidbody2D bullet_ridigbody = bullet.GetComponent<Rigidbody2D>();
        bullet_ridigbody.AddForce(new Vector2(crosshair_pos.position.x - player_pos.position.x + total_x_accuracy, 
            crosshair_pos.position.y-player_pos.position.y+total_y_accuracy).normalized * player_weapon.velocity,
            ForceMode2D.Impulse);

        curr_fire_delay = 0;
    }

    private void Aim()
    {
        Debug.Log("Aiming.");
    }

    private void BulletDelayCount()
    {
        curr_fire_delay += Time.deltaTime;
    }

    private void CrosshairMove()
    {
        crosshair_pos.position = new Vector3(mouse_pos.x, mouse_pos.y, crosshair_pos.position.z);
    }

    public Weapon GetPlayerWeaponInfo()
    {
        return player_weapon;
    }
}

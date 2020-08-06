﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon
{
    private string name;
    private int damage;
    private float delay, accuracy;

    public Weapon(string name, int damage, float delay, float accuracy)
    {
        this.name = name;
        this.damage = damage;
        this.delay = delay;
        this.accuracy = accuracy;
    }

    public string GetName()
    {
        return name;
    }
    public int GetDamage()
    {
        return damage;
    }

    public float GetDelay()
    {
        return delay;
    }

    public float GetAccuracy()
    {
        return accuracy;
    }
}

public class MouseEvent : MonoBehaviour
{ 
    public ObjectManager object_manager;
    public Transform player_pos;
    public Transform crosshair_pos;

    private Weapon player_weapon;
    private Weapon pistol;
    private GameObject bullet;
    private float curr_fire_delay;
    private Vector3 mouse_pos;

    private void Awake()
    {
        curr_fire_delay = 0.2f;

        pistol = new Weapon("pistol", 1, 0.2f, 0.2f);
        player_weapon = pistol;
    }

    private void Update()
    {
        mouse_pos = UnityEngine.Camera.main.ScreenToWorldPoint(Input.mousePosition);

        InputKey();
        BulletDelay();
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
            Debug.Log("Not Aiming.");
        }
    }

    private void Fire()
    {
        //플레이어 무기 확인
        switch (player_weapon.GetName())
        {
            case "pistol":
                this.bullet = GameObject.Find("pistol_bullet1");
                break;
        }

        if (!Input.GetMouseButton(0))
            return;

        if (curr_fire_delay < player_weapon.GetDelay())
            return;

        float ran = Random.Range(-0.2f, 0.2f);
        //총알 풀링
        GameObject bullet = object_manager.MakeObject(player_weapon.GetName());
        bullet.transform.position = this.transform.position;
        Rigidbody2D bullet_ridigbody = bullet.GetComponent<Rigidbody2D>();
        bullet_ridigbody.AddForce(new Vector2(crosshair_pos.position.x -player_pos.position.x + ran, 
            crosshair_pos.position.y-player_pos.position.y+ran).normalized * 20.0f, ForceMode2D.Impulse);

        curr_fire_delay = 0;
    }

    private void Aim()
    {
        Debug.Log("Aiming.");
    }

    private void BulletDelay()
    {
        curr_fire_delay += Time.deltaTime;
    }

    private void CrosshairMove()
    {
        crosshair_pos.position = new Vector3(mouse_pos.x, mouse_pos.y, crosshair_pos.position.z);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public GameObject handgun_bullet_prefab;
    public GameObject machinegun_bullet_prefab;
    public GameObject rifle_bullet_prefab;
    public GameObject shotgun_bullet_prefab;
    public GameObject launcher_bullet_prefab;

    public GameObject explosion_effect_prefab;

    public GameObject sentry_bullet_prefab;

    private GameObject[] handgun_bullets;
    private GameObject[] machinegun_bullets;
    private GameObject[] rifle_bullets;
    private GameObject[] shotgun_bullets;
    private GameObject[] launcher_bullets;

    private GameObject[] explosion_effects;

    private GameObject[] sentry_bullets;
    //오브젝트 생성할 때 사용할 빈 인스턴스
    private GameObject[] target_object;

    private void Awake()
    {
        handgun_bullets = new GameObject[3];
        machinegun_bullets = new GameObject[10];
        rifle_bullets = new GameObject[2];
        shotgun_bullets = new GameObject[16];
        launcher_bullets = new GameObject[2];

        explosion_effects = new GameObject[6];

        sentry_bullets = new GameObject[24];

        Generate();
    }

    private void Generate()
    {
        for (int i = 0; i < handgun_bullets.Length; i++)
        {   
            handgun_bullets[i] = Instantiate(handgun_bullet_prefab);
            handgun_bullets[i].SetActive(false);
        }

        for (int i = 0; i < machinegun_bullets.Length; i++)
        {
            machinegun_bullets[i] = Instantiate(machinegun_bullet_prefab);
            machinegun_bullets[i].SetActive(false);
        }

        for (int i = 0; i < rifle_bullets.Length; i++)
        {
            rifle_bullets[i] = Instantiate(rifle_bullet_prefab);
            rifle_bullets[i].SetActive(false);
        }

        for (int i = 0; i < shotgun_bullets.Length; i++)
        {
            shotgun_bullets[i] = Instantiate(shotgun_bullet_prefab);
            shotgun_bullets[i].SetActive(false);
        }

        for (int i = 0; i < launcher_bullets.Length; i++)
        {
            launcher_bullets[i] = Instantiate(launcher_bullet_prefab);
            launcher_bullets[i].SetActive(false);
        }

        for (int i = 0; i < explosion_effects.Length; i++)
        {
            explosion_effects[i] = Instantiate(explosion_effect_prefab);
            explosion_effects[i].SetActive(false);
        }

        for (int i = 0; i < sentry_bullets.Length; i++)
        {
            sentry_bullets[i] = Instantiate(sentry_bullet_prefab);
            sentry_bullets[i].SetActive(false);
        }
    }

    public GameObject MakeObject(string object_name)
    {

        switch (object_name)
        {  
            case "HandgunBullet":
                target_object = handgun_bullets;
                break;
            case "MachinegunBullet":
                target_object = machinegun_bullets;
                break;
            case "RifleBullet":
                target_object = rifle_bullets;
                break;
            case "ShotgunBullet":
                target_object = shotgun_bullets;
                break;
            case "LauncherBullet":
                target_object = launcher_bullets;
                break;

            case "ExplosionEffect":
                target_object = explosion_effects;
                break;

            case "SentryBullet":
                target_object = sentry_bullets;
                break;
        }

        for (int i = 0; i < target_object.Length; i++)
            if (!target_object[i].activeSelf)
            {
                target_object[i].SetActive(true);
                return target_object[i];
            }

        return null;
    }
}

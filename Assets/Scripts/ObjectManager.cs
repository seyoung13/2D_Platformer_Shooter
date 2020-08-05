using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public GameObject pistol_bullet_prefab;

    private GameObject[] pistol_bullet;

    //오브젝트 생성할 때 사용할 빈 인스턴스
    private GameObject[] target_object;

    private void Awake()
    {
        pistol_bullet = new GameObject[12];

        Generate();
    }

    private void Generate()
    {
        for (int i = 0; i < pistol_bullet.Length; i++)
        {   
            pistol_bullet[i] = Instantiate(pistol_bullet_prefab);
            pistol_bullet[i].SetActive(false);
        }
    }

    public GameObject MakeObject(string object_name)
    {

        switch (object_name)
        {  
            case "pistol":
                target_object = pistol_bullet;
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

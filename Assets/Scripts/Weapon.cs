using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{ 
    public enum BulletAttribution { basic, penetrating, explosive };
    public BulletAttribution bullet_atrribution;
    public int damage, bullet_per_shot, max_magazine;
    [HideInInspector] public int curr_magazine;
    public float delay, max_accuracy, min_accuracy, velocity, max_reload_time;
    [HideInInspector] public float curr_reload_time;
}

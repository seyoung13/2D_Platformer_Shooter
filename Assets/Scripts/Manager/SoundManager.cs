using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager sound_player;

    public AudioClip handgun_fire_sound;
    public AudioClip machinegun_fire_sound;
    public AudioClip rifle_fire_sound;
    public AudioClip shotgun_fire_sound;
    public AudioClip launcher_fire_sound;
    public AudioClip missile_explode_sound;

    private AudioSource audio_source;

    private void Awake()
    {   
        if (sound_player == null)
            sound_player = this;
    }

    void Start()
    {
        audio_source = gameObject.AddComponent<AudioSource>();
    }

    public void PlayHandgunFire()
    {
        audio_source.PlayOneShot(handgun_fire_sound);
    }

    public void PlayMachinegunFire()
    {
        audio_source.PlayOneShot(machinegun_fire_sound);
    }

    public void PlayRifleFire()
    {
        audio_source.PlayOneShot(rifle_fire_sound);
    }

    public void PlayShotgunFire()
    {
        audio_source.PlayOneShot(shotgun_fire_sound);
    }

    public void PlayLauncherFire()
    {
        audio_source.PlayOneShot(launcher_fire_sound);
    }

    public void PlayMissileExplode()
    {
        audio_source.PlayOneShot(missile_explode_sound);
    }
}

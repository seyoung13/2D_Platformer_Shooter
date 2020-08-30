using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
}


public class SoundManager : MonoBehaviour
{
    public static SoundManager sound_player;

    public Sound[] bgm, sfx;

    public AudioSource bgm_player;
    public AudioSource[] sfx_player;

    private void Awake()
    {   
        if (sound_player == null)
            sound_player = this;
    }

    public void PlayBGM(string bgm_name)
    {
        for(int i=0; i<bgm.Length; i++)
        {
            if (bgm_name == bgm[i].name)
            {
                bgm_player.clip = bgm[i].clip;
                bgm_player.Play();
            }
        }
    }

    public void StopBGM()
    {
        bgm_player.Stop();
    }

    public void PlaySFX(string sfx_name)
    {
        for (int i = 0; i < sfx.Length; i++)
        {
            if (sfx_name == sfx[i].name)
            {
                for (int j = 0; j < sfx_player.Length; j++)
                {
                    if (!sfx_player[j].isPlaying)
                    {
                        sfx_player[j].clip = sfx[i].clip;
                        sfx_player[j].Play();
                        return;
                    }
                }
                Debug.Log("모든 sfx 플레이어가 재생 중.");
                return;
            }
        }
        Debug.Log(sfx_name + "이란 이름의 sfx가 없음.");
    }
}

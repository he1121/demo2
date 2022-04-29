using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;



public class AudioManager : MonoBehaviour
{
    static AudioManager current;

    public AudioClip 环境音效;
    public AudioClip 背景音乐;

    public AudioClip[] 玩家行走音效;
    public AudioClip 玩家跳跃音效;
    public AudioClip 玩家降落音效;
    public AudioClip 玩家二段跳音效;
    public AudioClip[] 玩家攻击音效;
    public AudioClip 玩家受击音效;
    public AudioClip 怪物受击音效;


    AudioSource 环境音源;
    AudioSource 背景音源;
    AudioSource 玩家音源;
    AudioSource 怪物音源;

    public AudioMixerGroup mixerGroup;


    private void Awake()
    {
        current = this;
        DontDestroyOnLoad(gameObject);

        环境音源 = gameObject.AddComponent<AudioSource>();
        背景音源 = gameObject.AddComponent<AudioSource>();
        玩家音源 = gameObject.AddComponent<AudioSource>();
        怪物音源 = gameObject.AddComponent<AudioSource>();
        PlayBackgroundAudio();

    }


    void PlayBackgroundAudio()
    {
        current.背景音源.clip = current.背景音乐;
        current.背景音源.loop = true;
        current.背景音源.outputAudioMixerGroup = mixerGroup;

        current.背景音源.Play();
    }
    public static void PauseBackgroundAudio()
    {

        current.背景音源.clip = current.背景音乐;
        current.背景音源.loop = true;
        current.背景音源.Pause();
    }

    public static void PlayWalkAudio()
    {
        int index = Random.Range(0, current.玩家行走音效.Length);
        current.玩家音源.clip = current.玩家行走音效[index];
        current.玩家音源.Play();
    }

    public static void PlayJumpAudio()
    {
        current.玩家音源.clip = current.玩家跳跃音效;
        current.玩家音源.Play();
    }

    public static void PlayFallAudio()
    {
        current.玩家音源.clip = current.玩家降落音效;
        current.玩家音源.Play();
    }

    public static void PlayDoubleJumpAudio()
    {
        current.玩家音源.clip = current.玩家二段跳音效;
        current.玩家音源.Play();
    }

    public static void PlayAttackJumpAudio()
    {
        int index = Random.Range(0, current.玩家攻击音效.Length);
        current.玩家音源.clip = current.玩家攻击音效[index];
        current.玩家音源.Play();
    }

    public static void PlayerDamageAudio()
    {
        current.玩家音源.clip = current.玩家受击音效;
        current.玩家音源.Play();
    }

    public static void EnemyDamageAudio()
    {
        current.怪物音源.clip = current.怪物受击音效;
        current.怪物音源.Play();
    }
}

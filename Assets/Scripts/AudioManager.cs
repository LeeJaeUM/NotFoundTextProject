using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("#BGM")]
    public AudioClip bgmClip;
    public float bgmVolume;
    AudioSource bgmPlayer;

    [Header("#SFX")]
    public AudioClip[] sfxClips;
    public float sfxVolume;
    public int channels;
    AudioSource[] sfxPlayers;
    int channelIndex;

    [Header("Obj_Transform")]
    public Transform listenerPosition; // 듣는 사람의 위치 (주로 카메라)
    public Transform[] soundSourcePosition = new Transform[5]; // 소리를 발생시키는 오브젝트의 위치
    public float maxVolumeDistance = 15f; // 최대 볼륨을 갖는 거리

    public enum Sfx
    {
        Knock,
        Knock_Power,
        Choke,
        SellPhone,
        Bell,
    }

    private void Awake()
    {
        instance = this;
        Init();
    }
    private void Start()
    {
        PlayBgm(true);
    }

    private void Init()
    {
        //배경음 플레이어 초기화
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform;
        bgmPlayer = bgmObject.AddComponent<AudioSource>();
        bgmPlayer.playOnAwake = false;
        bgmPlayer.loop = true;
        bgmPlayer.volume = bgmVolume;
        bgmPlayer.clip = bgmClip;

        //효과음 플레이어 초기화
        GameObject sfxObject = new GameObject("SfxPlayer");
        sfxObject.transform.parent = transform;
        sfxPlayers = new AudioSource[channels];

        for (int index = 0; index < sfxPlayers.Length; index++)
        {
            sfxPlayers[index] = sfxObject.AddComponent<AudioSource>();
            sfxPlayers[index].playOnAwake = false;
            sfxPlayers[index].volume = sfxVolume;
        }
    }

    public void PlayBgm(bool isPlay)
    {
        if (isPlay)
        {
            bgmPlayer.Play();
        }
        else
        {
            bgmPlayer.Stop();
        }
    }

    public void PlaySfx(Sfx sfx)
    {
        for (int index = 0; index < sfxPlayers.Length; index++)
        {
            int loopIndex = (index + channelIndex) % sfxPlayers.Length;
            //플레이 중이면 반복 넘김
            if (sfxPlayers[loopIndex].isPlaying)
                continue;

            channelIndex = loopIndex;
            sfxPlayers[loopIndex].clip = sfxClips[(int)sfx];
            sfxPlayers[loopIndex].Play();
            StartCoroutine(DirAudio(loopIndex));
            break;
        }
    }


    IEnumerator DirAudio(int index)
    {
        Debug.Log($"{index}번째거 음악실애");
        while (true)
        {

            if (listenerPosition != null && soundSourcePosition != null && sfxPlayers[index] != null)
            {
                float distance = Vector3.Distance(soundSourcePosition[index].position, listenerPosition.position);
                float volume = 1f; // 초기 볼륨

                // 소리가 최대 거리를 벗어난 경우
                if (distance > maxVolumeDistance)
                {
                    volume = 0f; // 소리가 들리지 않음
                }
                else
                {
                    // 소리가 최대 거리 내에 있는 경우, 거리에 따라 볼륨 조절
                    volume = 1f - (distance / maxVolumeDistance);
                }

                sfxPlayers[index].volume = volume; // 볼륨 설정
            }
            yield return null;
        }
    }
}
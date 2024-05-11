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
    public Transform listenerPosition; // ��� ����� ��ġ (�ַ� ī�޶�)
    public Transform[] soundSourcePosition = new Transform[5]; // �Ҹ��� �߻���Ű�� ������Ʈ�� ��ġ
    public float maxVolumeDistance = 15f; // �ִ� ������ ���� �Ÿ�

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
        //����� �÷��̾� �ʱ�ȭ
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform;
        bgmPlayer = bgmObject.AddComponent<AudioSource>();
        bgmPlayer.playOnAwake = false;
        bgmPlayer.loop = true;
        bgmPlayer.volume = bgmVolume;
        bgmPlayer.clip = bgmClip;

        //ȿ���� �÷��̾� �ʱ�ȭ
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
            //�÷��� ���̸� �ݺ� �ѱ�
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
        Debug.Log($"{index}��°�� ���ǽǾ�");
        while (true)
        {

            if (listenerPosition != null && soundSourcePosition != null && sfxPlayers[index] != null)
            {
                float distance = Vector3.Distance(soundSourcePosition[index].position, listenerPosition.position);
                float volume = 1f; // �ʱ� ����

                // �Ҹ��� �ִ� �Ÿ��� ��� ���
                if (distance > maxVolumeDistance)
                {
                    volume = 0f; // �Ҹ��� �鸮�� ����
                }
                else
                {
                    // �Ҹ��� �ִ� �Ÿ� ���� �ִ� ���, �Ÿ��� ���� ���� ����
                    volume = 1f - (distance / maxVolumeDistance);
                }

                sfxPlayers[index].volume = volume; // ���� ����
            }
            yield return null;
        }
    }
}
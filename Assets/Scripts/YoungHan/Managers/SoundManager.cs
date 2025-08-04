using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sound;
using UnityEngine.SceneManagement;

//�Ҹ��� ������ �з��� ���� �����̽�
namespace Sound
{
    //�������
    public enum Music
    {
        Start,
        End
    }

    //ȿ����
    public enum Effect
    {
        Start,
        End
    }
}

/// <summary>
/// ������ ������ǰ� ȿ������ �Ѱ��ϴ� �Ŵ���, �̱����� ��� ���� Ŭ����
/// </summary>
[RequireComponent(typeof(AudioSource))]
public sealed class SoundManager : Manager<SoundManager>
{
    private bool _hasAudioSource1 = false;

    private AudioSource _audioSource1;

    //��������� ����ϴ� ����� �ҽ� ��ü1
    private AudioSource getMusicAudio1
    {
        get
        {
            if(_hasAudioSource1 == false)
            {
                _hasAudioSource1 = true;
                _audioSource1 = GetComponent<AudioSource>();
                _audioSource1.playOnAwake = false;
                _audioSource1.loop = true;
            }
            return _audioSource1;
        }
    }

    private bool _hasAudioSource2 = false;

    private AudioSource _audioSource2;

    //��������� ����ϴ� ����� �ҽ� ��ü2
    private AudioSource getMusicAudio2 {
        get
        {
            if (_hasAudioSource2 == false)
            {
                _hasAudioSource2 = true;
                AudioSource[] audioSources = GetComponents<AudioSource>();
                AudioSource audioSource = null;
                int length = audioSources.Length;
                for(int i = 0; i < length; i++)
                {
                    if(audioSources[i] != getMusicAudio1)
                    {
                        bool success = true;
                        int count = _effectAudioList.Count;
                        for(int j = 0; j < count; j++)
                        {
                            if(audioSources[i] == _effectAudioList[j])
                            {
                                success = false;
                                break;
                            }
                        }
                        if(success == true)
                        {
                            audioSource = audioSources[i];
                        }
                    }
                    if(audioSource != null)
                    {
                        break;
                    }
                }
                _audioSource2 = audioSource != null ? audioSource : gameObject.AddComponent<AudioSource>();
                _audioSource2.playOnAwake = false;
                _audioSource2.loop = true;
            }
            return _audioSource2;
        }
    }

    //ȿ�������� ����ϴ� ����� �ҽ� ��ü��
    private List<AudioSource> _effectAudioList = new List<AudioSource>();

    //����� ������� Ŭ����
    [SerializeField, Header("����� ������� Ŭ����")]
    private AudioClip[] _musicClips = new AudioClip[(int)Music.End];

    //����� ȿ���� Ŭ����
    [SerializeField, Header("����� ȿ���� Ŭ����")]
    private AudioClip[] _effectClips = new AudioClip[(int)Effect.End];

    //������� ����
    [SerializeField, Header("������� ����"), Range(0, 1)]
    private float _musicVolume;
    public static float musicVolume {
        get
        {
            return instance._musicVolume;
        }
        set
        {
            instance._musicVolume = Mathf.Clamp01(value);
            if (instance.IsCoroutineWorking() == false)
            {
                if (instance.getMusicAudio1.isPlaying == true)
                {
                    instance.getMusicAudio1.volume = instance._musicVolume;
                }
                if (instance.getMusicAudio2.isPlaying == true)
                {
                    instance.getMusicAudio2.volume = instance._musicVolume;
                }
            }
        }
    }

    //ȿ���� ����
    [SerializeField, Header("ȿ���� ����"), Range(0, 1)]
    private float _effectVolume;

    public static float effectVolume {
        get
        {
            return instance._effectVolume;
        }
        set
        {
            instance._effectVolume = Mathf.Clamp01(value);
            int count = instance._effectAudioList.Count;
            for(int i = 0; i < count; i++)
            {
                instance._effectAudioList[i].volume = instance._effectVolume;
            }
        }
    }

    /// <summary>
    /// �ʱ�ȭ �Լ�: �̱��� ��ü�� ������ �����Ѵ�.
    /// </summary>
    protected override void Initialize()
    {
        _destroyOnLoad = false;
    }

    /// <summary>
    /// ��������� �÷��� �����ִ� �Լ�(�ε����� ����� ������� ���� ��Ŵ)
    /// </summary>
    /// <param name="music"></param>
    /// <param name="immediately"></param>
    public static void Play(Music music, bool immediately = false)
    {
        int index = (int)music;
        int length = instance._musicClips.Length;
        if (index >= 0 && index < length)
        {
            Play(instance._musicClips[index], immediately);
        }
        else if(music == Music.End)
        {
            Stop(immediately);
        }
    }

    /// <summary>
    /// ��������� �÷��� �����ִ� �Լ�
    /// </summary>
    /// <param name="audioClip"></param>
    /// <param name="immediately"></param>
    public static void Play(AudioClip audioClip, bool immediately)
    {
        if (audioClip != null)
        {
            instance.StopCoroutine(DoSmoothlyPlay(null, null), true);
            if (immediately == true)
            {
                if (instance.getMusicAudio1.isPlaying == false)
                {
                    Play(instance.getMusicAudio1, instance.getMusicAudio2);
                }
                else
                {
                    Play(instance.getMusicAudio2, instance.getMusicAudio1);
                }
                void Play(AudioSource crescendo, AudioSource diminuendo)
                {
                    diminuendo.Stop();
                    crescendo.clip = audioClip;
                    crescendo.volume = musicVolume;
                    crescendo.Play();
                }
            }
            else
            {
                if (instance.getMusicAudio1.isPlaying == false)
                {
                    instance.StartCoroutine(DoSmoothlyPlay(instance.getMusicAudio1, instance.getMusicAudio2), true);
                }
                else
                {
                    instance.StartCoroutine(DoSmoothlyPlay(instance.getMusicAudio2, instance.getMusicAudio1), true);
                }
            }
            IEnumerator DoSmoothlyPlay(AudioSource crescendo, AudioSource diminuendo)
            {
                crescendo.clip = audioClip;
                crescendo.Play();
                float volume = 0;
                while (volume < musicVolume)
                {
                    crescendo.volume = volume;
                    diminuendo.volume = Mathf.Clamp01(musicVolume - volume);
                    volume += Time.deltaTime;
                    yield return null;
                }
                crescendo.volume = musicVolume;
                diminuendo.volume = 0;
                diminuendo.Stop();
            }
        }
    }

    /// <summary>
    /// ��������� ���� �����ִ� �Լ�
    /// </summary>
    public static void Stop(bool immediately = false)
    {
        instance.StopCoroutine(DoSmoothlyStop(), true);
        if (immediately == true)
        {
            instance.getMusicAudio1.Stop();
            instance.getMusicAudio2.Stop();
        }
        else
        {
            instance.StartCoroutine(DoSmoothlyStop(), true);
        }
        IEnumerator DoSmoothlyStop()
        {
            float audio1Value = instance.getMusicAudio1.isPlaying == true ? instance.getMusicAudio1.volume : 0;
            float audio2Value = instance.getMusicAudio2.isPlaying == true ? instance.getMusicAudio2.volume : 0;
            while (audio1Value > 0 || audio2Value > 0)
            {
                float deltaTime = Time.deltaTime;
                if (audio1Value > 0)
                {
                    audio1Value -= deltaTime;
                    instance.getMusicAudio1.volume = audio1Value;
                }
                else if (instance.getMusicAudio1.isPlaying == true)
                {
                    instance.getMusicAudio1.Stop();
                }
                if (audio2Value > 0)
                {
                    audio2Value -= deltaTime;
                    instance.getMusicAudio2.volume = audio2Value;
                }
                else if (instance.getMusicAudio2.isPlaying == true)
                {
                    instance.getMusicAudio2.Stop();
                }
                yield return null;
            }
        }
    }

    /// <summary>
    /// ȿ������ �÷��� �����ִ� �Լ�(�ε����� ����� ȿ������ ������� ����)
    /// </summary>
    /// <param name="effect"></param>
    public static void Play(Effect effect)
    {
        int index = (int)effect;
        int length = instance._effectClips.Length;
        if (index >= 0 && index < length)
        {
            Play(instance._effectClips[index]);
        }
    }

    /// <summary>
    /// ȿ������ �÷��� �����ִ� �Լ�
    /// </summary>
    /// <param name="audioClip"></param>
    public static void Play(AudioClip audioClip)
    {
        if (audioClip != null)
        {
            int count = instance._effectAudioList.Count;
            for (int i = 0; i < count; i++)
            {
                if (instance._effectAudioList[i].isPlaying == false)
                {
                    instance._effectAudioList[i].PlayOneShot(audioClip);
                    return;
                }
            }
            AudioSource audioSource = instance.gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.loop = false;
            audioSource.volume = effectVolume;
            audioSource.PlayOneShot(audioClip);
            instance._effectAudioList.Add(audioSource);
        }
    }

    /// <summary>
    /// ��� ȿ�������� ���� �����ִ� �Լ�
    /// </summary>
    public static void Stop()
    {
        int count = instance._effectAudioList.Count;
        for (int i = 0; i < count; i++)
        {
            instance._effectAudioList[i].Stop();
        }
    }
}
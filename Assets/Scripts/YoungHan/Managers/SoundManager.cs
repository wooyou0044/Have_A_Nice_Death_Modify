using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sound;
using UnityEngine.SceneManagement;

//소리의 유형을 분류한 네임 스페이스
namespace Sound
{
    //배경음악
    public enum Music
    {
        Start,
        End
    }

    //효과음
    public enum Effect
    {
        Start,
        End
    }
}

/// <summary>
/// 게임의 배경음악과 효과음을 총괄하는 매니저, 싱글턴을 상속 받은 클래스
/// </summary>
[RequireComponent(typeof(AudioSource))]
public sealed class SoundManager : Manager<SoundManager>
{
    private bool _hasAudioSource1 = false;

    private AudioSource _audioSource1;

    //배경음악을 담당하는 오디오 소스 객체1
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

    //배경음악을 담당하는 오디오 소스 객체2
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

    //효과음들을 담당하는 오디오 소스 객체들
    private List<AudioSource> _effectAudioList = new List<AudioSource>();

    //사용할 배경음악 클립들
    [SerializeField, Header("사용할 배경음악 클립들")]
    private AudioClip[] _musicClips = new AudioClip[(int)Music.End];

    //사용할 효과음 클립들
    [SerializeField, Header("사용할 효과음 클립들")]
    private AudioClip[] _effectClips = new AudioClip[(int)Effect.End];

    //배경음악 볼륨
    [SerializeField, Header("배경음악 볼륨"), Range(0, 1)]
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

    //효과음 볼륨
    [SerializeField, Header("효과음 볼륨"), Range(0, 1)]
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
    /// 초기화 함수: 싱글턴 객체의 삭제를 방지한다.
    /// </summary>
    protected override void Initialize()
    {
        _destroyOnLoad = false;
    }

    /// <summary>
    /// 배경음악을 플레이 시켜주는 함수(인덱스를 벗어나면 배경음을 정지 시킴)
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
    /// 배경음악을 플레이 시켜주는 함수
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
    /// 배경음악을 정지 시켜주는 함수
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
    /// 효과음을 플레이 시켜주는 함수(인덱스를 벗어나면 효과음이 재생되지 않음)
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
    /// 효과음을 플레이 시켜주는 함수
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
    /// 모든 효과음들을 정지 시켜주는 함수
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
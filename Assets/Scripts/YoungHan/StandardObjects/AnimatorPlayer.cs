using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 원하는 애니메이션을 동작시킬 수 있는 클래스
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public sealed class AnimatorPlayer : MonoBehaviour
{
    private bool _hasSpriteRenderer = false;

    private SpriteRenderer _spriteRenderer = null;

    private SpriteRenderer getSpriteRenderer
    {
        get
        {
            if (_hasSpriteRenderer == false)
            {
                _hasSpriteRenderer = true;
                _spriteRenderer = GetComponent<SpriteRenderer>();
            }
            return _spriteRenderer;
        }
    }

    private bool _hasAnimator = false;

    private Animator _animator = null;

    public Animator animator
    {
        get
        {
            if (_hasAnimator == false)
            {
                _hasAnimator = true;
                _animator = GetComponent<Animator>();
            }
            return _animator;
        }
    }

    private IEnumerator _coroutine = null;

    private List<AnimationClip> _animationClips = new List<AnimationClip>();

    /// <summary>
    /// 애니메이션 진행이 다 끝났는지 확인하는 프로퍼티
    /// </summary>
    public bool isEndofFrame
    {
        get
        {
            return _coroutine == null;
        }
    }

    private void OnDisable()
    {
        Stop();
    }

    private void OnDestroy()
    {
        Stop();
    }

    private void OnApplicationQuit()
    {
        Stop();
    }

    /// <summary>
    /// 강제로 애니메이션을 멈추게 만드는 함수
    /// </summary>
    public void Stop()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }
        _animationClips.Clear();
    }

    /// <summary>
    /// 스프라이트 렌더러를 반전 시켜주는 함수
    /// </summary>
    public void Flip()
    {
        getSpriteRenderer.flipX = !getSpriteRenderer.flipX;
    }

    /// <summary>
    /// 스프라이트 렌더러를 인자값에 따라 뒤집어주는 함수
    /// </summary>
    /// <param name="flip"></param>
    public void Flip(bool flip)
    {
        getSpriteRenderer.flipX = flip;
    }

    /// <summary>
    /// 특정 애니메이션을 재생 시키는 함수
    /// </summary>
    /// <param name="animationClip">재생할 애니메이션 클립</param>
    /// <param name="force">true일 경우 기존에 진행 중인 애니메이션 재생을 취소하고 새롭게 재생</param>
    public void Play(AnimationClip animationClip, bool force = true)
    {
        Play(animationClip, null, false, force);
    }

    /// <summary>
    /// 특정 애니메이션 두 개를 순차적으로 재생 시키는 함수
    /// </summary>
    /// <param name="first"></param>
    /// <param name="second"></param>
    public void Play(AnimationClip first, AnimationClip second)
    {
        Play(first, second, false, true);
    }

    /// <summary>
    /// 특정 애니메이션 두 개를 순차적으로 재생 시키는 함수
    /// </summary>
    /// <param name="first">재생할 첫 번째 애니메이션 클립</param>
    /// <param name="second">재생할 두 번째 애니메이션 클립</param>
    /// <param name="flip">스프라이트 렌더러를 일시적으로 반전 시켰다가 원래대로 되돌림</param>
    /// <param name="force">true일 경우 기존에 진행 중인 애니메이션 재생을 취소하고 새롭게 재생</param>
    public void Play(AnimationClip first, AnimationClip second, bool flip, bool force = true)
    {
        if (gameObject.activeInHierarchy == false || Application.isPlaying == false)
        {
            return;
        }
        if (_coroutine != null)
        {
            if (force == false)
            {
                return;
            }
            else if (_animationClips.Count > 0)
            {
                _animationClips.Clear();
            }
            StopCoroutine(_coroutine);
        }
        _coroutine = DoPlay();
        StartCoroutine(_coroutine);
        IEnumerator DoPlay()
        {
            if(flip == false)
            {
                Flip(false);
            }
            if (first != null)
            {
                if (flip == true)
                {
                    Flip(true);
                }
                string name = first.name;
                animator.Play(name, 0, 0f);
                yield return null;
                Func<bool> func = () =>
                {
                    AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                    return stateInfo.normalizedTime < 1.0f && stateInfo.IsName(name) == true;
                };
                yield return new WaitWhile(func);
            }
            if (second != null)
            {
                if (flip == true)
                {
                    Flip(false);
                }
                string name = second.name;
                animator.Play(name, 0, 0f);
                yield return null;
                Func<bool> func = () =>
                {
                    AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                    return stateInfo.normalizedTime < 1.0f && stateInfo.IsName(name) == true;
                };
                yield return new WaitWhile(func);
            }
            _coroutine = null;
            if (_animationClips.Count > 0)
            {
                AnimationClip animationClip = _animationClips[0];
                _animationClips.RemoveAt(0);
                Play(animationClip, false);
            }
        }
    }

    /// <summary>
    /// 애니메이션 플레이를 예약한다.
    /// </summary>
    /// <param name="animationClip"></param>
    public void Reserve(AnimationClip animationClip)
    {
        if (_coroutine == null)
        {
            Play(animationClip);
        }
        else
        {
            _animationClips.Add(animationClip);
        }
    }

    /// <summary>
    /// 현재 이미지가 뒤집어졌는지 아닌지 확인하는 함수
    /// </summary>
    /// <returns></returns>
    public bool IsFlip()
    {
        return getSpriteRenderer.flipX;
    }

    /// <summary>
    /// 특정 애니메이션이 진행 중인지 확인하는 함수
    /// </summary>
    /// <param name="text">특정 애니메이션 이름</param>
    /// <returns>애니메이션 이름이 일치하면 참을 반환</returns>
    public bool IsPlaying(string text)
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(text);
    }

    /// <summary>
    /// 특정 애니메이션이 진행 중인지 확인하는 함수
    /// </summary>
    /// <param name="animationClip">특정 애니메이션 클립</param>
    /// <returns>애니메이션 클립이 일치하면 참을 반환</returns>
    public bool IsPlaying(AnimationClip animationClip)
    {
        return GetCurrentClips() == animationClip;
    }

    /// <summary>
    /// 현재 애니메이션 클립이 무엇인지 반환하는 함수
    /// </summary>
    /// <returns>현재 재생 중인 애니메이션 클립을 반환함</returns>
    public AnimationClip GetCurrentClips()
    {
        AnimatorClipInfo[] clipInfos = animator.GetCurrentAnimatorClipInfo(0);
        int length = clipInfos != null ? clipInfos.Length : 0;
        if (length > 0)
        {
            return clipInfos[0].clip;
        }
        return null;
    }
}
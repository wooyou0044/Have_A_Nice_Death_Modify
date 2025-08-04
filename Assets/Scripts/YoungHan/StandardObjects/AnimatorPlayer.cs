using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���ϴ� �ִϸ��̼��� ���۽�ų �� �ִ� Ŭ����
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
    /// �ִϸ��̼� ������ �� �������� Ȯ���ϴ� ������Ƽ
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
    /// ������ �ִϸ��̼��� ���߰� ����� �Լ�
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
    /// ��������Ʈ �������� ���� �����ִ� �Լ�
    /// </summary>
    public void Flip()
    {
        getSpriteRenderer.flipX = !getSpriteRenderer.flipX;
    }

    /// <summary>
    /// ��������Ʈ �������� ���ڰ��� ���� �������ִ� �Լ�
    /// </summary>
    /// <param name="flip"></param>
    public void Flip(bool flip)
    {
        getSpriteRenderer.flipX = flip;
    }

    /// <summary>
    /// Ư�� �ִϸ��̼��� ��� ��Ű�� �Լ�
    /// </summary>
    /// <param name="animationClip">����� �ִϸ��̼� Ŭ��</param>
    /// <param name="force">true�� ��� ������ ���� ���� �ִϸ��̼� ����� ����ϰ� ���Ӱ� ���</param>
    public void Play(AnimationClip animationClip, bool force = true)
    {
        Play(animationClip, null, false, force);
    }

    /// <summary>
    /// Ư�� �ִϸ��̼� �� ���� ���������� ��� ��Ű�� �Լ�
    /// </summary>
    /// <param name="first"></param>
    /// <param name="second"></param>
    public void Play(AnimationClip first, AnimationClip second)
    {
        Play(first, second, false, true);
    }

    /// <summary>
    /// Ư�� �ִϸ��̼� �� ���� ���������� ��� ��Ű�� �Լ�
    /// </summary>
    /// <param name="first">����� ù ��° �ִϸ��̼� Ŭ��</param>
    /// <param name="second">����� �� ��° �ִϸ��̼� Ŭ��</param>
    /// <param name="flip">��������Ʈ �������� �Ͻ������� ���� ���״ٰ� ������� �ǵ���</param>
    /// <param name="force">true�� ��� ������ ���� ���� �ִϸ��̼� ����� ����ϰ� ���Ӱ� ���</param>
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
    /// �ִϸ��̼� �÷��̸� �����Ѵ�.
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
    /// ���� �̹����� ������������ �ƴ��� Ȯ���ϴ� �Լ�
    /// </summary>
    /// <returns></returns>
    public bool IsFlip()
    {
        return getSpriteRenderer.flipX;
    }

    /// <summary>
    /// Ư�� �ִϸ��̼��� ���� ������ Ȯ���ϴ� �Լ�
    /// </summary>
    /// <param name="text">Ư�� �ִϸ��̼� �̸�</param>
    /// <returns>�ִϸ��̼� �̸��� ��ġ�ϸ� ���� ��ȯ</returns>
    public bool IsPlaying(string text)
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(text);
    }

    /// <summary>
    /// Ư�� �ִϸ��̼��� ���� ������ Ȯ���ϴ� �Լ�
    /// </summary>
    /// <param name="animationClip">Ư�� �ִϸ��̼� Ŭ��</param>
    /// <returns>�ִϸ��̼� Ŭ���� ��ġ�ϸ� ���� ��ȯ</returns>
    public bool IsPlaying(AnimationClip animationClip)
    {
        return GetCurrentClips() == animationClip;
    }

    /// <summary>
    /// ���� �ִϸ��̼� Ŭ���� �������� ��ȯ�ϴ� �Լ�
    /// </summary>
    /// <returns>���� ��� ���� �ִϸ��̼� Ŭ���� ��ȯ��</returns>
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
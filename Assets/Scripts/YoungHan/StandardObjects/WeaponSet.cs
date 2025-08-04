using System;
using System.Collections;
using UnityEngine;

public class WeaponSet : MonoBehaviour
{
    private bool _hasTransform = false;

    private Transform _transform = null;

    private Transform getTransform
    {
        get
        {
            if (_hasTransform == false)
            {
                _hasTransform = true;
                _transform = transform;
            }
            return _transform;
        }
    }

    private enum State: byte
    {
        None,
        Combo1,
        Combo2,
        Combo3,
        Combo4,
    }

    [SerializeField, Header("³´ Á¤º¸")]
    private Scythe _scytheInfo = null;
    [SerializeField, Header("ÄÞº¸ È½¼ö")]
    private State _state;
    [SerializeField, Range(0, 5)]
    private float _comboMoveValue = 0.15f;
    [SerializeField, Range(0, 5)]
    private float _comboMoveDelay = 0.4f;
    [SerializeField, Range(0, 5)]
    private float _comboLastDelay = 0.9f;
    [SerializeField, Range(0, 5)]
    private float _comboStandDelay = 0.5f;
    [SerializeField, Range(0, 1)]
    private float _jumpStartDelay = 0.1f;
    [SerializeField, Range(0, 1)]
    private float _jumpFallingDelay = 0.3f;
    [SerializeField, Range(0, 1)]
    private float _jumpLandingDelay = 1.0f;

    [SerializeField]
    private float _concentrationTime = 0;
    [SerializeField]
    private AnimationClip _concenteStartClip;
    [SerializeField]
    private AnimationClip _concenteLoopClip;
    [SerializeField]
    private AnimationClip _restAttackClip;
    [SerializeField]
    private Skill _restAttackSkill;
    [SerializeField]
    private GameObject _restAttackGameObject;
    [SerializeField]
    private string[] _restAttackTags;
    private IEnumerator _coroutine = null;

    public bool TryScythe(Player player, bool pressed, Action action1, Action<GameObject, Vector2, Transform> action2, Action<Strike, Strike.Area, GameObject> action3, Func<Projectile, Projectile> func)
    {
        if (player != null && player.isAlive == true)
        {
            if (_scytheInfo != null)
            {
                if (pressed == true)
                {
                    AnimatorPlayer animatorPlayer = player.animatorPlayer;
                    bool hasAnimatorPlayer = animatorPlayer != null;
                    Animator animator = hasAnimatorPlayer == true ? animatorPlayer.animator : null;
                    if (animator != null)
                    {
                        if (_concentrationTime == 0)
                        {
                            switch (player.direction)
                            {
                                case Player.Direction.Center:
                                case Player.Direction.Forward:
                                case Player.Direction.Backward:
                                    if (_state < State.Combo4 && player.CompareConstraints(RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation) == false &&
                                        _scytheInfo.TryUse(getTransform, player.isGrounded == true ? Weapon.Attack.Move : Weapon.Attack.Stand, action2, action3, func, animator) == true)
                                    {
                                        if (hasAnimatorPlayer == true)
                                        {
                                            animatorPlayer.Flip(false);
                                            animatorPlayer.Stop();
                                        }
                                        if (_coroutine != null)
                                        {
                                            StopCoroutine(_coroutine);
                                        }
                                        _coroutine = DoPlay();
                                        StartCoroutine(_coroutine);
                                        IEnumerator DoPlay()
                                        {
                                            _state++;
                                            switch (_state)
                                            {
                                                case State.Combo1:
                                                case State.Combo2:
                                                case State.Combo3:
                                                    if (player.isGrounded == false)
                                                    {
                                                        player.Dash(Vector2.zero, _comboStandDelay);
                                                        yield return new WaitUntil(() => player!= null && player.isGrounded == true);
                                                    }
                                                    else
                                                    {
                                                        player.Dash(_comboMoveValue, _comboMoveDelay, 0);
                                                        yield return new WaitForSeconds(_comboMoveDelay);
                                                        player.Dash(Vector2.zero);
                                                    }
                                                    break;
                                                case State.Combo4:
                                                    yield return new WaitForSeconds(_comboLastDelay);
                                                    action1?.Invoke();
                                                    break;
                                            }
                                            player?.Recover();
                                            _state = State.None;
                                            _coroutine = null;
                                        }
                                    }
                                    break;
                            }
                        }
                        else if(_concentrationTime > _comboStandDelay && player.isGrounded == true && _coroutine == null && _state == State.None)
                        {
                            _coroutine = DoPlay();
                            StartCoroutine(_coroutine);
                            IEnumerator DoPlay()
                            {
                                animatorPlayer.Play(_concenteStartClip, _concenteLoopClip);
                                if(_restAttackGameObject != null)
                                {
                                    _restAttackGameObject.SetActive(true);
                                }
                                while(player != null && player.isGrounded == true)
                                {
                                    yield return null;
                                }
                                _coroutine = null;
                            }
                        }
                    }
                    _concentrationTime += Time.deltaTime;
                }
                else
                {
                    if (_coroutine != null && _state == State.None)
                    {
                        if (_restAttackGameObject != null)
                        {
                            _restAttackGameObject.SetActive(false);
                        }
                        AnimatorPlayer animatorPlayer = player.animatorPlayer;
                        if (animatorPlayer != null)
                        {
                            AnimationClip animationClip = animatorPlayer.GetCurrentClips();
                            if(animationClip == _concenteStartClip || animationClip == _concenteLoopClip)
                            {
                                StopCoroutine(_coroutine);
                                _coroutine = DoPlay();
                                StartCoroutine(_coroutine);
                                IEnumerator DoPlay()
                                {
                                    animatorPlayer.Play(_restAttackClip);
                                    player.Dash(Vector2.up, _jumpStartDelay);
                                    yield return new WaitForSeconds(_jumpStartDelay);
                                    player?.Dash(Vector2.zero, _jumpFallingDelay);
                                    yield return new WaitForSeconds(_jumpFallingDelay);
                                    player?.Dash(Vector2.down);
                                    yield return new WaitUntil(() => (player != null && player.isGrounded == true));
                                    _restAttackSkill?.Use(getTransform, null, _restAttackTags, action2, action3, func);
                                    action1?.Invoke();
                                    yield return new WaitForSeconds(_jumpLandingDelay);
                                    player?.Recover();
                                    _coroutine = null;
                                    _concentrationTime = 0;
                                }
                            }
                        }
                        else
                        {
                            player.Recover();
                            StopCoroutine(_coroutine);
                            _coroutine = null;
                            _concentrationTime = 0;
                        }
                    }
                    else if(_concentrationTime > 0)
                    {
                        _concentrationTime = 0;
                    }
                }
            }
        }
        else
        {
            if(_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }
            if (_concentrationTime > 0)
            {
                _concentrationTime = 0;
            }
        }
        return _coroutine != null;
    }

    public bool IsInvincibleState(AnimatorPlayer animatorPlayer)
    {
        if(animatorPlayer != null && animatorPlayer.GetCurrentClips() == _restAttackClip)
        {
            return true;
        }
        return false;
    }
}
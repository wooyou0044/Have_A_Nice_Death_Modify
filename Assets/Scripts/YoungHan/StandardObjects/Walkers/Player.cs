using System;
using UnityEngine;

/// <summary>
/// 유저가 조종하는 플레이어 클래스
/// </summary>
[RequireComponent(typeof(WeaponSet))]
public sealed class Player : Runner, IHittable
{
    private static readonly float MinimumDropVelocity = -0.49f;

    private bool _hasWeaponSet = false;

    private WeaponSet _weaponSet = null;

    private WeaponSet getWeaponSet
    {
        get
        {
            if(_hasWeaponSet == false)
            {
                _hasWeaponSet = true;
                _weaponSet = GetComponent<WeaponSet>();
            }
            return _weaponSet;
        }
    }

    [Space(10f), Header("애니메이션 클립")]
    [SerializeField]
    private AnimationClip _idleClip = null;
    [SerializeField]
    private AnimationClip _idleToRunClip = null;
    [SerializeField]
    private AnimationClip _idleUturnClip = null;
    [SerializeField]
    private AnimationClip _waitingClip = null;
    [SerializeField]
    private AnimationClip _runClip = null;
    [SerializeField]
    private AnimationClip _runToIdleClip = null;
    [SerializeField]
    private AnimationClip _runUturnClip = null;
    [SerializeField]
    private AnimationClip _jumpStartClip = null;
    [SerializeField]
    private AnimationClip _jumpFallingClip = null;
    [SerializeField]
    private AnimationClip _jumpLandingClip = null;
    [SerializeField]
    private AnimationClip _dashClip = null;
    [SerializeField]
    private AnimationClip _zipUpClip = null;
    [SerializeField]
    private AnimationClip _hitClip = null;
    [SerializeField]
    private AnimationClip _deadClip = null;
    [SerializeField]
    private AnimatorPlayer _animatorPlayer = null;

    public AnimatorPlayer animatorPlayer
    {
        get
        {
            return _animatorPlayer;
        }
    }

    public enum Direction
    {
        Center,
        Forward,
        Backward
    }

    [SerializeField]
    private Direction _direction = Direction.Center;

    public Direction direction
    {
        get
        {
            return _direction;
        }
    }

    [Space(10f), Header("체력")]
    //활성 체력'으로, 남아있는 체력을 의미한다.
    [SerializeField, Range(0, byte.MaxValue)]
    private byte _remainLife;

    public byte remainLife {
        get
        {
            return _remainLife;
        }
    }

    //회색 체력은 '상처'로, 일반적인 회복제로 회복할 수 있는 체력이다.
    [SerializeField, Range(0, byte.MaxValue)]
    private byte _lossLife;

    public byte lossLife {
        get
        {
            return _lossLife;
        }
    }
    //최대치의 체력을 의미한다.
    [SerializeField, Range(0, byte.MaxValue)]
    private byte _maxLife;

    public byte maxLife {
        get
        {
            return _maxLife;
        }
    }

    public bool isAlive
    {
        get
        {
            return _remainLife > 0 || _maxLife <= _remainLife;
        }
    }

    [Space(10f), Header("마력")]
    //'활성 마력'으로, 남아있는 마력을 의미한다.
    [SerializeField, Range(0, byte.MaxValue)]
    private byte _remainMana;

    public byte remainMana {
        get
        {
            return _remainMana;
        }
    }

    //최대치의 마력을 의미한다.
    [SerializeField, Range(0, byte.MaxValue)]
    private byte _maxMana;

    public byte maxMana {
        get
        {
            return _maxMana;
        }
    }

    [SerializeField]
    private GameObject _healObject = null;

    private bool _stopping = false;

    private Action _shakeAction = null;
    private Action<bool> _engageAction = null;
    private Action<IHittable, int> _reportAction = null;
    private Action<Strike, Strike.Area, GameObject> _useAction = null;
    private Action<GameObject, Vector2, Transform> _effectAction = null;
    private Func<bool> _fallFunction = null;
    private Func<bool, bool> _ladderFunction = null;
    private Func<Projectile, Projectile> _projectileFunction = null;

    private void PlayMove(bool straight)
    {
        if (_animatorPlayer != null)
        {
            AnimationClip animationClip = _animatorPlayer.GetCurrentClips();
            if (straight == false)
            {
                if (animationClip == _idleClip || animationClip == _waitingClip)
                {
                    _animatorPlayer.Play(_idleUturnClip, _idleClip, true);
                }
                else if (animationClip == _runToIdleClip || animationClip == _idleToRunClip || animationClip == _idleUturnClip || animationClip == _runClip)
                {
                    _animatorPlayer.Play(_runUturnClip, _runClip, true);
                }
            }
            else if (animationClip == _idleClip || animationClip == _waitingClip)
            {
                _animatorPlayer.Play(_idleToRunClip, _runClip, false);
            }
        }
    }

    private void PlayMove(Vector2 rotation)
    {
        if ((Vector2)getTransform.rotation.eulerAngles == rotation)
        {
            PlayMove(true);
        }
        else
        {
            PlayMove(false);
            getTransform.rotation = Quaternion.Euler(rotation);
        }
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        if (_lossLife > _maxLife)
        {
            _lossLife = _maxLife;
        }
        if(_remainLife > _maxLife - _lossLife)
        {
            _remainLife = (byte)(_maxLife - _lossLife);
        }
        _reportAction?.Invoke(this, 0);
        if (_remainMana > _maxMana)
        {
            _remainMana = _maxMana;
        }
    }
#endif

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        bool isGrounded = this.isGrounded;
        base.OnCollisionEnter2D(collision);
        if (isGrounded != this.isGrounded && _stopping == false)
        {
            _animatorPlayer?.Play(_jumpLandingClip, _idleClip, false);
            _engageAction?.Invoke(true);
            if(getRigidbody2D.constraints == (RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation))
            {
                getRigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
            }
        }
    }

    protected override void OnCollisionStay2D(Collision2D collision)
    {
        base.OnCollisionStay2D(collision);
        if (isGrounded == true && _stopping == false && getRigidbody2D.velocity.y == 0 && _animatorPlayer != null && _animatorPlayer.GetCurrentClips() == _jumpFallingClip)
        {
            _animatorPlayer?.Play(_jumpLandingClip, _idleClip, false);
        }
    }

    protected override void OnCollisionExit2D(Collision2D collision)
    {
        base.OnCollisionExit2D(collision);
        if (isGrounded == false && getRigidbody2D.velocity.y < MinimumDropVelocity && _stopping == false)
        {
            _animatorPlayer?.Play(_jumpFallingClip);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (_animatorPlayer != null && _animatorPlayer.IsPlaying(_zipUpClip) == true)
        {
            _animatorPlayer.Play(_jumpFallingClip);
        }
    }

    private bool CanMoveState()
    {
        RigidbodyConstraints2D rigidbodyConstraints2D = getRigidbody2D.constraints;
        if (rigidbodyConstraints2D != (RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation) &&
              rigidbodyConstraints2D != (RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation) &&
              rigidbodyConstraints2D != RigidbodyConstraints2D.FreezeAll && _animatorPlayer != null)
        {
            return true;
        }
        return false;
    }

    public override void MoveLeft()
    {
        if (isAlive == true && CanMoveState() == true && _stopping == false && Time.timeScale > 0)
        {
            base.MoveLeft();
            PlayMove(LeftRotation);
        }
    }

    public override void MoveRight()
    {
        if (isAlive == true && CanMoveState() == true && _stopping == false && Time.timeScale > 0)
        {
            base.MoveRight();
            PlayMove(RightRotation);
        }
    }

    public override void MoveStop()
    {
        _direction = Direction.Center;
        if (isAlive == true)
        {
            if(_animatorPlayer != null && _animatorPlayer.IsPlaying(_runClip) == true)
            {
                _animatorPlayer.Play(_runToIdleClip, _idleClip, false);
            }
            base.MoveStop();
        }
    }

    public override void Jump()
    {
        if (isAlive == true)
        {
            if (Time.timeScale == 0)
            {
                _engageAction?.Invoke(false);
            }
            else if(_stopping == false)
            {
                if (_direction == Direction.Backward && _fallFunction != null && _fallFunction.Invoke() == true)
                {
                    _animatorPlayer.Play(_jumpStartClip, _jumpFallingClip, false);
                }
                else
                {
                    float velocity = getRigidbody2D.velocity.y;
                    base.Jump();
                    if (velocity != getRigidbody2D.velocity.y || (_animatorPlayer != null && _animatorPlayer.IsPlaying(_zipUpClip) == true))
                    {
                        getRigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
                        _engageAction?.Invoke(true);
                        _animatorPlayer.Play(_jumpStartClip, _jumpFallingClip, false);
                    }
                }
            }
        }
    }

    public override void Dash(float value, float delay, float coolTime)
    {
        if (isAlive == true)
        {
            base.Dash(value, delay, coolTime);
        }
    }

    public void Initialize(Action shake, Action<bool> engage, Action<IHittable, int>report, Action<GameObject, Vector2, Transform>effect, Action<Strike, Strike.Area, GameObject>use, Func<bool>fall, Func<bool, bool>ladder, Func<Projectile, Projectile>projectile)
    {
        _shakeAction = shake;
        _engageAction = engage;
        _reportAction = report;
        _effectAction = effect;
        _useAction = use;
        _fallFunction = fall;
        _ladderFunction = ladder;
        _projectileFunction = projectile;
    }

    public void MoveUp()
    {
        if (isAlive == true)
        {
            _direction = Direction.Forward;
            if (_stopping == false && _animatorPlayer != null)
            {
                AnimationClip animationClip = _animatorPlayer.GetCurrentClips();
                if (animationClip != _zipUpClip && animationClip != _dashClip && _ladderFunction != null && _ladderFunction.Invoke(true) == true)
                {
                    getRigidbody2D.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
                    _animatorPlayer.Play(_zipUpClip);
                    RecoverJumpCount();
                }
            }
        }
    }
    
    public void MoveDown()
    {
        if (isAlive == true)
        {
            _direction = Direction.Backward;
            if (_stopping == false && _animatorPlayer != null && _animatorPlayer.IsPlaying(_zipUpClip) == true && _ladderFunction != null && _ladderFunction.Invoke(false) == true)
            {
                getRigidbody2D.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
                _animatorPlayer.Play(_jumpFallingClip);
            }
        }
    }

    public void Dash(Direction direction)
    {
        if (isAlive == true && CanDash() == true && _stopping == false && Time.timeScale > 0)
        {
            switch(direction)
            {
                case Direction.Center:
                    Dash(new Vector2(getTransform.forward.normalized.z, 0));
                    break;
                case Direction.Forward:
                    PlayMove(RightRotation);
                    Dash(new Vector2(1, 0));
                    break;
                case Direction.Backward:
                    PlayMove(LeftRotation);
                    Dash(new Vector2(-1, 0));
                    break;
            }
            _engageAction?.Invoke(true);
            if (isGrounded == true)
            {
                _animatorPlayer?.Play(_dashClip, _idleClip, false);
            }
            else
            {
                _animatorPlayer?.Play(_dashClip, _jumpFallingClip, false);
            }
        }
    }

    public void AttackScythe(bool pressed)
    {
        if (Time.timeScale > 0)
        {
            _stopping = getWeaponSet.TryScythe(this, pressed, _shakeAction, _effectAction, _useAction, _projectileFunction);
        }
    }

    public void Attack1()
    {

    }

    public void Attack2()
    {

    }

    public void AttackWide()
    {

    }

    public void Interact()
    {
        _engageAction?.Invoke(false);
    }

    public void Recover()
    {
        if(isAlive == true)
        {
            if(isGrounded == true)
            {
                _animatorPlayer?.Play(_idleClip);
            }
            else
            {
                _animatorPlayer?.Play(_jumpFallingClip);
            }
        }
    }

    public void Heal(byte value = 0)
    {
        if(value > 0)
        {
            GameManager.anima += value;
        }
        else if(GameManager.anima > 0)
        {
            float recover = maxLife * 0.15f;
            Hit(new Strike((int)recover, 0));
            _effectAction?.Invoke(_healObject, getCollider2D.bounds.center, getTransform);
            GameManager.anima--;
        }
    }

    public void Hit(Strike strike)
    {
        if (isAlive == true && ((_animatorPlayer != null && _animatorPlayer.GetCurrentClips() != _dashClip) || getWeaponSet.IsInvincibleState(_animatorPlayer) == false))
        {
            int result = strike.result;
            //피 채우는 용도라면
            if (result > 0)
            {
                //최대 체력 보다 값이 넘을 경우는 최대 체력이 된다.
                if (_remainLife + result > _maxLife - _lossLife)
                {
                    _remainLife = (byte)(_maxLife - _lossLife);
                }
                //그렇지 않으면 값을 더해준다.
                else
                {
                    _remainLife += (byte)result;
                }
            }
            //피 깎는 용도라면
            else if(result < 0)
            {
                if (-result < _remainLife)
                {
                    _animatorPlayer?.Play(_hitClip, _idleClip);
                    _remainLife -= (byte)-result;
                }
                //사망
                else
                {
                    _animatorPlayer?.Play(_deadClip);
                    _remainLife = 0;
                }
            }
            _reportAction?.Invoke(this, result);
        }
    }

    public Collider2D GetCollider2D()
    {
        return getCollider2D;
    }
}
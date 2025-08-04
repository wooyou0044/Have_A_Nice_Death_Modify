using System.Collections;
using UnityEngine;

/// <summary>
/// ��Ŀ�� ��� ���� �� �� �ִ� ���� Ŭ����
/// </summary>
public class Runner : Walker
{
    //������
    [SerializeField, Header("���� ����"), Range(0, byte.MaxValue)]
    protected float _jumpValue = 5;

    //�ִ� ���� �ѵ�
    [SerializeField, Header("�ִ� ���� �ѵ�"), Range(1, 10)]
    protected byte _jumpLimit = 1;

    //���� ���� Ƚ��
    [SerializeField]
    private byte _jumpCount = 0;

    private IEnumerator _jumpCoroutine = null;

    [SerializeField, Header("�뽬 ����"), Range(0, 100)]
    protected float _dashValue = 50;
    [SerializeField, Header("�뽬 ���� �ð�"), Range(0, 5)]
    private float _dashDelay = 0.2f;
    [SerializeField, Header("�뽬 ��Ÿ��"), Range(0, 5)]
    protected float _dashCoolTime = 1.5f;

    [SerializeField]
    private bool _isDashed = false;

    private IEnumerator _dashCoroutine = null;

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        if (isGrounded == true)
        {
            RecoverJumpCount();
        }
        else
        {
            _jumpCount = 0;
        }
    }
#endif

    /// <summary>
    /// �ٴڿ� �浹�ϴ��� ���θ� Ȯ���ϴ� �޼���
    /// </summary>
    /// <param name="collision"></param>
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
        if(isGrounded == true)
        {
            if(_jumpCoroutine != null)
            {
                StopCoroutine(_jumpCoroutine);
                _jumpCoroutine = null;
            }
            RecoverJumpCount();
        }
    }

    protected override void OnCollisionStay2D(Collision2D collision)
    {
        bool isGrounded = this.isGrounded;
        base.OnCollisionStay2D(collision);
        if(isGrounded != this.isGrounded)
        {
            RecoverJumpCount();
        }
    }

    public override void MoveLeft()
    {
        if (_dashCoroutine == null)
        {
            base.MoveLeft();
        }
    }

    public override void MoveRight()
    {
        if (_dashCoroutine == null)
        {
            base.MoveRight();
        }
    }

    public override void MoveStop()
    {
        if (_dashCoroutine == null)
        {
            base.MoveStop();
        }
    }

    //������ �ϰ� ����� �޼���
    public virtual void Jump()
    {
        if (_jumpCount > 0 && _jumpCoroutine == null && getRigidbody2D.gravityScale > 0)
        {
            if (_dashCoroutine != null)
            {
                StopCoroutine(_dashCoroutine);
            }
            _jumpCoroutine = DoJumpAndDelay();
            StartCoroutine(_jumpCoroutine);
            IEnumerator DoJumpAndDelay()
            {
                _jumpCount--;
                getRigidbody2D.velocity = new Vector2(0, _jumpValue);
                while (getRigidbody2D.velocity.y > 0)
                {
                    yield return null; // ������ ������ ���
                }
                _jumpCoroutine = null;
            }
        }
    }

    public virtual void Dash(float value, float delay, float coolTime)
    {
        Dash(new Vector2(getTransform.forward.normalized.z * value, 0), delay, coolTime);
    }
  
    protected void RecoverJumpCount()
    {
        _jumpCount = _jumpLimit;
    }

    public void Dash(Vector2 direction)
    {
        Dash(direction, _dashDelay);
    }

    //�뽬�� �ϰ� ����� �޼���
    public void Dash(Vector2 direction, float value)
    {
        Dash(direction, value, _dashCoolTime);
    }

    public void Dash(Vector2 direction, float value, float coolTime)
    {
        if (gameObject.activeInHierarchy == false)
        {
            return;
        }
        if (_dashCoroutine != null)
        {
            StopCoroutine(_dashCoroutine);
        }
        _dashCoroutine = DoDashAndDelay();
        StartCoroutine(_dashCoroutine);
        IEnumerator DoDashAndDelay()
        {
            _isDashed = true;
            if (direction == Vector2.zero)
            {
                getRigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
            }
            else if (direction == Vector2.up || direction == Vector2.down)
            {
                getRigidbody2D.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
            }
            else if (direction == Vector2.left || direction == Vector2.right)
            {
                getRigidbody2D.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
            }
            getRigidbody2D.velocity = direction * _dashValue;
            yield return new WaitForSeconds(value);
            getRigidbody2D.constraints = RigidbodyConstraints2D.None | RigidbodyConstraints2D.FreezeRotation;
            getRigidbody2D.velocity += Vector2.down;
            _dashCoroutine = null;
            yield return new WaitForSeconds(coolTime);
            _isDashed = false;
        }
    }

    public bool CanDash()
    {
        if(_dashCoroutine == null && _isDashed == false)
        {
            return true;
        }
        return false;
    }

    public bool CompareConstraints(RigidbodyConstraints2D rigidbodyConstraints2D)
    {
        return getRigidbody2D.constraints == rigidbodyConstraints2D;
    }
}
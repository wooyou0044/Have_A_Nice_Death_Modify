using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 위치를 지정할 수 있고 이동할 수 있는 워커 클래스
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]

public class Walker : MonoBehaviour, IMovable
{
    //왼쪽 방향 오일러 벡터
    protected static readonly Vector2 LeftRotation = new Vector2(0, 180);
    //오른쪽 방향 오일러 벡터
    protected static readonly Vector2 RightRotation = new Vector2(0, 0);

    private bool _hasTransform = false;

    private Transform _transform = null;

    protected Transform getTransform
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

    private bool _hasRigidbody2D = false;

    private Rigidbody2D _rigidbody2D = null;

    protected Rigidbody2D getRigidbody2D
    {
        get
        {
            if (_hasRigidbody2D == false)
            {
                _hasRigidbody2D = true;
                _rigidbody2D = GetComponent<Rigidbody2D>();
                _rigidbody2D.freezeRotation = true;
            }
            return _rigidbody2D;
        }
    }

    private bool _hasCollider2D = false;

    private Collider2D _collider2D = null;

    protected Collider2D getCollider2D
    {
        get
        {
            if (_hasCollider2D == false)
            {
                _hasCollider2D = true;
                _collider2D = GetComponent<Collider2D>();
            }
            return _collider2D;
        }
    }

    private Collision2D _groundCollision2D = null;

    private List<Collision2D> _leftCollision2D = new List<Collision2D>();

    private List<Collision2D> _rightCollision2D = new List<Collision2D>();

#if UNITY_EDITOR
    //땅에 착륙했는지 유무를 판단하는 프로퍼티
    [SerializeField]
    private bool _isGrounded = false;
#endif

    //땅에 닿아있는지를 반환하는 프로퍼티
    public bool isGrounded
    {
        get
        {
            return _groundCollision2D != null;
        }
    }

    //이동 속도
    [SerializeField, Header("이동 속도"), Range(0, float.MaxValue)]
    protected float _movingSpeed = 10;

#if UNITY_EDITOR

    //기즈모 표시 색깔
    [SerializeField, Header("기즈모 표시 색깔")]
    private Color _gizmoColor = Color.black;

    protected virtual void OnDrawGizmos()
    {
        Bounds bounds = getCollider2D.bounds;
        float radius = bounds.size.x * 0.5f;
        float minX = bounds.center.x + (radius * Mathf.Cos(Mathf.PI * -0.75f));
        float maxX = bounds.center.x + (radius * Mathf.Cos(Mathf.PI * -0.25f));
        float centerY = bounds.min.y + radius + (radius * Mathf.Sin(Mathf.PI * -0.25f));
        Debug.DrawLine(new Vector2(minX, centerY), new Vector2(maxX, centerY), _gizmoColor);
        Debug.DrawLine(new Vector2(minX, centerY), new Vector2(minX, bounds.min.y), _gizmoColor);
        Debug.DrawLine(new Vector2(maxX, centerY), new Vector2(maxX, bounds.min.y), _gizmoColor);
        Debug.DrawLine(new Vector2(minX, bounds.min.y), new Vector2(maxX, bounds.min.y), _gizmoColor);
    }

    protected virtual void OnValidate()
    {
        if(_isGrounded == false && _groundCollision2D != null)
        {
            _groundCollision2D = null;
        }
    }
#endif

    /// <summary>
    /// 바닥에 충돌하는지 여부를 확인하는 메서드
    /// </summary>
    /// <param name="collision"></param>
    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        Bounds bounds = getCollider2D.bounds;
        float radius = bounds.size.x * 0.5f;
        float minX = bounds.center.x + (radius * Mathf.Cos(Mathf.PI * -0.75f));
        float maxX = bounds.center.x + (radius * Mathf.Cos(Mathf.PI * -0.25f));
        float centerY = bounds.min.y + radius + (radius * Mathf.Sin(Mathf.PI * -0.25f));
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector2 point = collision.contacts[i].point;
            if (point.x > minX && point.x < maxX && point.y < centerY)
            {
#if UNITY_EDITOR
                _isGrounded = true;
#endif
                _groundCollision2D = collision;
                break;
            }
        }
    }

    /// <summary>
    /// 양쪽에 충돌체가 있는지 여부를 확인하는 메서드
    /// </summary>
    /// <param name="collision"></param>
    protected virtual void OnCollisionStay2D(Collision2D collision)
    {       
        Bounds bounds = getCollider2D.bounds;
        float radius = bounds.size.x * 0.5f;
        float minX = bounds.center.x + (radius * Mathf.Cos(Mathf.PI * -0.75f));
        float maxX = bounds.center.x + (radius * Mathf.Cos(Mathf.PI * -0.25f));
        float centerY = bounds.min.y + radius + (radius * Mathf.Sin(Mathf.PI * -0.25f));
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector2 point = collision.contacts[i].point;
            if (point.y >= centerY)
            {
                if (bounds.min.x - IMovable.OverlappingDistance < point.x && point.x < bounds.center.x && _leftCollision2D.Contains(collision) == false)
                {
                    _leftCollision2D.Add(collision);
                }
                if (bounds.center.x < point.x && point.x < bounds.max.x + IMovable.OverlappingDistance && _rightCollision2D.Contains(collision) == false)
                {
                    _rightCollision2D.Add(collision);
                }
            }
            else if (point.x > minX && point.x < maxX)
            {
#if UNITY_EDITOR
                _isGrounded = true;
#endif
                _groundCollision2D = collision;
            }
        }
    }

    /// <summary>
    /// 지면 충돌체나 양쪽 충돌체와 떨어졌는지 확인하는 메서드
    /// </summary>
    /// <param name="collision"></param>
    protected virtual void OnCollisionExit2D(Collision2D collision)
    {
        _leftCollision2D.Remove(collision);
        _rightCollision2D.Remove(collision);
        if(_groundCollision2D == collision)
        {
#if UNITY_EDITOR
            _isGrounded = false;
#endif
            _groundCollision2D = null;
        }
    }

    protected virtual void OnTriggerStay2D(Collider2D collider)
    {
        if (_groundCollision2D != null && _groundCollision2D.collider == collider)
        {
            Bounds bounds = getCollider2D.bounds;
            float radius = bounds.size.x * 0.5f;
            float minX = bounds.center.x + (radius * Mathf.Cos(Mathf.PI * -0.75f));
            float maxX = bounds.center.x + (radius * Mathf.Cos(Mathf.PI * -0.25f));
            float centerY = bounds.min.y + radius + (radius * Mathf.Sin(Mathf.PI * -0.25f));
            for (int i = 0; i < _groundCollision2D.contactCount; i++)
            {
                Vector2 point = _groundCollision2D.contacts[i].point;
                if(point.y < centerY)
                {
#if UNITY_EDITOR
                    _isGrounded = false;
#endif
                    _groundCollision2D = null;
                    break;
                }
            }
        }
    }


    /// <summary>
    /// 오른쪽으로 이동 시키는 메서드
    /// </summary>
    public virtual void MoveRight()
    {
        if (_rightCollision2D.Count == 0)
        {
            getRigidbody2D.velocity = new Vector2(+_movingSpeed, getRigidbody2D.velocity.y);
        }
    }

    /// <summary>
    /// 왼쪽으로 이동 시키는 메서드
    /// </summary>
    public virtual void MoveLeft()
    {
        if (_leftCollision2D.Count == 0)
        {
            getRigidbody2D.velocity = new Vector2(-_movingSpeed, getRigidbody2D.velocity.y);
        }
    }

    /// <summary>
    /// 이동을 중지 시키는 메서드
    /// </summary>
    public virtual void MoveStop()
    {
        getRigidbody2D.velocity = new Vector2(0, getRigidbody2D.velocity.y);
    }
}
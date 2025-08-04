using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Projectile : MonoBehaviour
{
    private bool _hasTransform = false;
    private Transform _transform = null;
    private Transform getTransform {
        get
        {
            if (_hasTransform == false)
            {
                _transform = transform;
                _hasTransform = true;
            }
            return _transform;
        }
    }

    private bool _hasCollider2D = false;

    private Collider2D _collider2D = null;

    private Collider2D getCollider2D
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

    [SerializeField, Header("발사체 접착성")]
    private bool _adhesion = false;
    [SerializeField, Header("발사 지연 시간"),Range(0, byte.MaxValue)]
    private float _dealyTime = 0;
    [SerializeField, Header("날아가는 시간"), Range(0, byte.MaxValue)]
    private float _flyingTime = 0.01f;
    [SerializeField, Header("날아가는 속도"), Range(0, byte.MaxValue)]
    private float _flyingSpeed = 1;
    [SerializeField, Header("타격 대상 태그")]
    private string[] _tags;
    [SerializeField, Header("타격 세기")]
    private Strike _strike;
    [SerializeField, Header("오프셋")]
    private Vector2 _offset;
    [SerializeField, Header("폭발 모양")]
    private Shape _shape = null;
    [SerializeField, Header("맞는 대상 모두에게 나타날 효과 오브젝트")]
    private GameObject _hitObject = null;
    [SerializeField, Header("해당 객체가 소멸하면서 나타날 효과 오브젝트")]
    private GameObject _explosionObject = null;

    private bool _ignition = false;
    private Action<GameObject, Vector2, Transform> _effectAction = null;
    private Action<Strike, Strike.Area, GameObject> _strikeAction = null;

#if UNITY_EDITOR
    [SerializeField, Header("폭발 유효 거리 표시 시간"), Range(0, byte.MaxValue)]
    private float _gizmoDuration = 1;
    [SerializeField, Header("폭발 유효 거리 표시 색깔")]
    private Color _gizmoColor = Color.red;

    /// <summary>
    /// 타격 유효 거리를 에디터 런타임에 표시하는 함수
    /// </summary>
    private void OnDrawGizmos()
    {
        if(_shape != null && Application.isPlaying == false)
        {
            Strike.PolygonArea area = _shape.GetPolygonArea(getTransform, null);
            area.Show(_gizmoColor);
        }
    }
#endif

    private void OnCollisionStay2D(Collision2D collision)
    {
        Explode(collision.collider);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Explode(collision);
    }

    /// <summary>
    /// 폭발하는 함수
    /// </summary>
    /// <param name="collider2D"></param>
    private void Explode(Collider2D collider2D = null)
    {
        if (_ignition == true && (collider2D == null || _tags.Contains(collider2D.tag) == true))
        {
            _effectAction?.Invoke(_explosionObject, getTransform.position, null);
            if (_shape != null)
            {
                _strikeAction?.Invoke(_strike, _shape.GetPolygonArea(getTransform, _tags), _hitObject);
            }
            else if (_strikeAction != null)
            {
#if UNITY_EDITOR
                Shape.GetPolygonArea(getCollider2D, _tags)?.Show(_gizmoColor, _gizmoDuration);
#endif
                ContactFilter2D contactFilter = new ContactFilter2D();
                List<Collider2D> others = new List<Collider2D>();
                Physics2D.OverlapCollider(getCollider2D, contactFilter, others);
                int count = others != null ? others.Count : 0;
                for (int i = 0; i < count; i++)
                {
                    IHittable hittable = others[i].GetComponent<IHittable>();
                    if (hittable != null && _tags.Contains(hittable.tag) == true)
                    {
                        _strikeAction.Invoke(_strike, new Strike.TargetArea(new IHittable[] { hittable }), _hitObject);
                    }
                }
            }
            _strikeAction = null;
            _effectAction = null;
            StopAllCoroutines();
            _ignition = false;
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 특정 방향에서 발사체를 쏠때 사용하는 함수
    /// </summary>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <param name="action1"></param>
    /// <param name="action2"></param>
    private void Shot(Vector2 position, Quaternion rotation, Action<GameObject, Vector2, Transform> action1, Action<Strike, Strike.Area, GameObject> action2)
    {
        gameObject.SetActive(true);
        Vector2 rotatedOffset = rotation * _offset;
        getTransform.position = position + rotatedOffset;
        getTransform.rotation = rotation;
        _strikeAction = action2;
        _effectAction = action1;
        StartCoroutine(DoProject());
        IEnumerator DoProject()
        {
            _ignition = false;
            yield return new WaitForSeconds(_dealyTime);
            _ignition = true;
            float duration = _flyingTime;
            while (duration >= 0 /*|| _flyingTime == 0*/)
            {
                float deltaTime = Time.deltaTime;
                Vector2 direction = getTransform.right.normalized;
                getTransform.Translate(direction * _flyingSpeed * deltaTime, Space.World);
                duration -= deltaTime;
                yield return null;
            }
            Explode();
        }
    }

    /// <summary>
    /// 발사체를 쏘는 함수
    /// </summary>
    /// <param name="user"></param>
    /// <param name="target"></param>
    /// <param name="action1"></param>
    /// <param name="action2"></param>
    public void Shot(Transform user, IHittable target, Action<GameObject, Vector2, Transform> action1, Action<Strike, Strike.Area, GameObject> action2, Action action3 = null)
    {
        Shot(user, target, action1, action2, action3, _flyingTime);
    }

    /// <summary>
    /// 발사체를 쏘는 함수
    /// </summary>
    /// <param name="user"></param>
    /// <param name="target"></param>
    /// <param name="action1"></param>
    /// <param name="action2"></param>
    /// <param name="flyingTime"></param>
    public void Shot(Transform user, IHittable target, Action<GameObject, Vector2, Transform> action1, Action<Strike, Strike.Area, GameObject> action2, Action action3, float flyingTime)
    {
        _flyingTime = flyingTime;
        if (user != null)
        {
            if (_adhesion == true)
            {
                getTransform.parent = user;
            }
            if (target != null)
            {
                Bounds bounds = target.GetCollider2D().bounds;
                Vector2 direction = ((Vector2)bounds.center - (Vector2)user.position).normalized;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                if (user.position.x <= bounds.center.x)
                {
                    Shot(user.position, Quaternion.Euler(0, 0, +angle), action1, action2);
                }
                else
                {
                    Shot(user.position, Quaternion.Euler(180, 0, -angle), action1, action2);
                }
            }
            else
            {
                Shot(user.position, user.rotation, action1, action2);
            }
        }
        else if (target != null)
        {
            if (_adhesion == true)
            {
                getTransform.parent = target.transform;
            }
            Shot(target.transform.position, target.transform.rotation, action1, action2);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
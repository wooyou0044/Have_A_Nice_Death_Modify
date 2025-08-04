using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

[DisallowMultipleComponent]
[RequireComponent(typeof(BossMovement))]
public class GargoyleBrain : MonoBehaviour
{
    private bool _hasBossMovement = false;

    private BossMovement _bossMovement = null;

    private BossMovement getBossMovement
    {
        get
        {
            if(_hasBossMovement == false)
            {
                _hasBossMovement = true;
                _bossMovement = GetComponent<BossMovement>();
            }
            return _bossMovement;
        }
    }

    [SerializeField, Header("��ų �غ� �ð�"), Range(0, byte.MaxValue)]
    private float _preparationTime = 2f;
    [SerializeField, Header("��ų �޽� �ð�"), Range(0, byte.MaxValue)]
    private float _waitingTime = 0.2f;
    [SerializeField, Header("�޺� �뽬 �ð�"), Range(0, byte.MaxValue)]
    private float _dashTime = 0.6f;
    [SerializeField, Header("��ų �޽� �ð�"), Range(0, byte.MaxValue)]
    public float _rechargeTime = 3f;
    [SerializeField, Header("��ų ���� ����")]
    public Skill _skill = Skill.Dash;
    [SerializeField, Header("Ȱ�� ���� ����")]
    private float _leftBoundary;
    [SerializeField, Header("Ȱ�� ���� ������")]
    private float _rightBoundary;

    public enum Skill
    {
        Scratching, //������
        Dash,       //����
        Stone,      //�� ������
        Fall,       //����
    }

#if UNITY_EDITOR

    [SerializeField, Header("���� Ȯ�� ����")]
    private float _rayLength = 10f;

    private void OnDrawGizmos()
    {
        Debug.DrawRay(new Vector2(_leftBoundary, 0), Vector2.up * _rayLength, Color.red);
        Debug.DrawRay(new Vector2(_rightBoundary, 0), Vector2.up * _rayLength, Color.red);
        Debug.DrawRay(new Vector2(_leftBoundary, 0), Vector2.down * _rayLength, Color.red);
        Debug.DrawRay(new Vector2(_rightBoundary, 0), Vector2.down * _rayLength, Color.red);
    }

    private void OnValidate()
    {
        if (transform.position.x < _leftBoundary)
        {
            _leftBoundary = transform.position.x;
        }
        if (transform.position.x > _rightBoundary)
        {
            _rightBoundary = transform.position.x;
        }
    }
#endif

    private void OnEnable()
    {
        Trace(FindObjectOfType<Player>());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public void Trace(IHittable hittable)
    {
        StartCoroutine(DoPlay());
        IEnumerator DoPlay()
        {
            Collider2D collider2D = getBossMovement.GetCollider2D();
            while (getBossMovement.isAlive == true)
            {
                yield return new WaitForSeconds(_preparationTime);
                float probability = Random.Range(0, 100);
                if (probability < 50)
                {
                    _skill = Skill.Scratching;
                }
                else
                {
                    _skill = Skill.Dash;
                }
                switch (_skill)
                {
                    case Skill.Scratching:
                        if ((hittable.transform.position.x < transform.position.x && transform.eulerAngles.y == 0) ||
                       (transform.position.x < hittable.transform.position.x && transform.eulerAngles.y == 180))
                        {
                            getBossMovement.UTurn();
                            yield return new WaitForSeconds(_waitingTime);
                        }
                        yield return StartCoroutine(DoComboAttack1());
                        if(getBossMovement.target != null)
                        {
                            getBossMovement.ComboAttack2();
                            getBossMovement.Dash(transform.up * _dashTime);
                            yield return new WaitForSeconds(_waitingTime);
                            getBossMovement.Dash(Vector2.zero, _waitingTime);
                            yield return new WaitForSeconds(_waitingTime);
                            if(transform.eulerAngles.y == 0 && hittable.transform.position.x < transform.position.x)
                            {
                                transform.rotation = Quaternion.Euler(new Vector2(0, 180));
                            }
                            else if (transform.eulerAngles.y == 180 && hittable.transform.position.x > transform.position.x)
                            {
                                transform.rotation = Quaternion.Euler(new Vector2(0, 0));
                            }
                            getBossMovement.Dash((hittable.transform.position - transform.position).normalized);
                            while(getBossMovement.isGrounded == false)
                            {
                                yield return null;
                            }
                            getBossMovement.Dash(Vector2.zero);
                        }
                        else
                        {
                            if ((hittable.transform.position.x < transform.position.x && transform.eulerAngles.y == 0) ||
                     (transform.position.x < hittable.transform.position.x && transform.eulerAngles.y == 180))
                            {
                                getBossMovement.UTurn();
                                yield return new WaitForSeconds(_waitingTime);
                            }
                            yield return StartCoroutine(DoComboAttack1());
                        }
                        IEnumerator DoComboAttack1()
                        {
                            getBossMovement.FollowPlayer(hittable.transform.position.x);
                            while (true)
                            {
                                yield return null;
                                if ((transform.eulerAngles.y == 180 && hittable.GetCollider2D().bounds.max.x > collider2D.bounds.min.x) ||
                                    (transform.eulerAngles.y == 0 && collider2D.bounds.max.x > hittable.GetCollider2D().bounds.min.x))
                                {
                                    break;
                                }
                            }
                            getBossMovement.MoveStop();
                            getBossMovement.ComboAttack1();
                            yield return new WaitForSeconds(_dashTime);
                            getBossMovement.Dash(new Vector2(transform.forward.normalized.z * _waitingTime, 0));
                            yield return new WaitForSeconds(_waitingTime);
                            getBossMovement.Dash(Vector2.zero);
                            yield return new WaitForSeconds(_dashTime + _waitingTime);
                        }
                        break;
                    case Skill.Dash:
                        if((hittable.transform.position.x < transform.position.x && transform.eulerAngles.y == 0) ||
                            (transform.position.x < hittable.transform.position.x && transform.eulerAngles.y == 180))
                        {
                            getBossMovement.UTurn();
                            yield return new WaitForSeconds(_waitingTime);
                        }
                        getBossMovement.FlyMove();  //�������� �̵��ϴ� �Լ� �ʿ�
                        getBossMovement.AdjustRotation();
                        while ((transform.eulerAngles.y == 0 && collider2D.bounds.max.x < _rightBoundary) ||
                           (transform.eulerAngles.y == 180 && collider2D.bounds.min.x > _leftBoundary))
                        {
                            yield return null;
                        }
                        getBossMovement.UTurn();
                        yield return new WaitForSeconds(_waitingTime);
                        getBossMovement.DropDiagonalMove(); //�缱 ����
                        getBossMovement.AdjustRotation();
                        //System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                        //stopwatch.Start();
                        if (transform.eulerAngles.y == 180)
                        {
                            getBossMovement.UseDashSkill(2.278f);
                        }
                        else
                        {
                            getBossMovement.UseDashSkill(2.017f);
                        }
                        while (getBossMovement.isGrounded == false)  //���� �� �� ���� ��ٸ�
                        {
                            yield return null;
                        }
                        getBossMovement.UTurn();
                        yield return new WaitForSeconds(_waitingTime);
                        getBossMovement.MoveOppositeEndPoint(); //���� ������ �� �ݴ�������� ����
                        while ((transform.eulerAngles.y == 0 && collider2D.bounds.max.x < _rightBoundary) ||
                            (transform.eulerAngles.y == 180 && collider2D.bounds.min.x > _leftBoundary))
                        {
                            yield return null;
                        }
                        //stopwatch.Stop();
                        //Debug.Log(stopwatch.ElapsedMilliseconds);
                        getBossMovement.UTurn();
                        yield return new WaitForSeconds(_waitingTime);
                        getBossMovement.MoveStop();
                        break;
                    case Skill.Stone:
                        break;
                    case Skill.Fall:
                        break;
                }
                yield return new WaitForSeconds(_rechargeTime);
            }
        }
    }
}
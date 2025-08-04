using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;

public class Spooksmen_AI : Walker, IHittable, ILootable
{
    LayerMask playerLayer;

    [SerializeField] AnimationClip uTurnClip;
    [SerializeField] AnimationClip idleClip;
    [SerializeField] AnimationClip surprisedClip;
    [SerializeField] AnimationClip hitClip;
    [SerializeField] AnimationClip stunClip;
    [SerializeField] AnimationClip deathClip;

    [Header("���� ����")]
    [SerializeField] float hp = 200.0f;

    [Header("�÷��̾� ����")]
    // �����ϴ� ����
    [SerializeField] float sightRange;
    // �÷��̾� �������� �� �ѵ� ����ǥ ������Ʈ
    [SerializeField] GameObject surprisedMark;
    [SerializeField] Transform surprisedPos;
    [SerializeField] LayerMask playerMask;

    [Header("��ȸ")]
    [SerializeField] float wonderRange;

    [Header("����")]
    [SerializeField] float attackRange;
    [SerializeField] float attackCoolTime;
    [SerializeField] GameObject bangMark;
    [SerializeField] GameObject attackMark;
    [SerializeField] Skill[] frontAttack;
    [SerializeField] Skill backAttack;
    [SerializeField] Skill upAttack;

    [Header("������")]
    [SerializeField] GameObject stunMark;
    [SerializeField] Transform stunPos;
    [SerializeField] GameObject[] deathGhost;

    [Header("����ϴ� ������")]
    [SerializeField] MonoBehaviour dropItem;

    Animator myAnimator;
    AnimatorPlayer myPlayer;

    Collider2D player;
    Collider2D attackPlayer;
    Collider2D damageCollider;
    Rigidbody2D myRigid;

    Transform targetPos;
    // �����ϱ� ���� �÷��̾ �ִ� ��ġ ���� ����
    Transform attackPos;
    // attack�� �� ���� Effect ���� ��ġ ���� ����
    Transform attackMarkPos;

    // ������� ��ġ
    Vector2 createdPos;
    Vector2 destination;

    GameObject surprised;
    GameObject bang;
    GameObject stun;
    GameObject attack;
    GameObject[] death;

    float homeDistance;
    float attackElapsedTime;
    float hitElapsedTime;

    int frontAttackNum;
    int comboHitNum;

    bool isHome;
    bool isAttacking;
    bool isDetect;
    bool isFrontAttack;
    bool isHit;
    bool isComboHit;
    bool isAttackFinish;

    enum AttackState
    {
        Default,
        FrontAttack,
        BackAttack,
        UpAttack,
    }
    AttackState state;

    public bool isAlive
    {
        get
        {
            return (hp > 0) ? true : false;
        }
    }

    private void Awake()
    {
        myAnimator = GetComponent<Animator>();
        myPlayer = GetComponent<AnimatorPlayer>();
        damageCollider = GetComponent<Collider2D>();
        myRigid = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        // ��� ����Ʈ ����
        surprised = Instantiate(surprisedMark, surprisedPos);
        surprised.SetActive(false);

        // ����ǥ ����
        bang = Instantiate(bangMark, surprisedPos);
        //bangMark.transform.position = new Vector2(0, 0);
        bang.SetActive(false);

        // ���� �� ����
        stun = Instantiate(stunMark, stunPos);
        stun.SetActive(false);

        // ���ݸ�ũ ����
        attackMarkPos = transform.GetChild(2).transform;
        attack = Instantiate(attackMark, attackMarkPos);
        //attackMark.transform.position = new Vector2(0, 0);
        //attackMark.transform.localScale = Vector2.one + Vector2.one;
        attack.SetActive(false);

        // �״� ���� ����
        death = new GameObject[deathGhost.Length];
        for(int i=0; i<deathGhost.Length; i++)
        {
            death[i] = Instantiate(deathGhost[i], transform.GetChild(3 + i).transform);
            death[i].SetActive(false);
        }

        // �ڱⰡ ������ ��ġ ����
        createdPos = transform.position;
        homeDistance = 0.5f;

        destination = new Vector2(createdPos.x + wonderRange, 0);

        isHome = true;
        isAttacking = false;
        isDetect = false;
        isAttackFinish = true;

        attackElapsedTime = attackCoolTime;
        state = AttackState.Default;
    }

    void Update()
    {
        attackPlayer = Physics2D.OverlapCircle(transform.position, attackRange, playerMask);
        if (attackPlayer != null)
        {
            isAttacking = true;
        }
        else
        {
            isAttacking = false;
            if (isAttackFinish == true)
            {
                if (frontAttackNum > 0)
                {
                    frontAttackNum = 0;
                    myAnimator.SetInteger("FrontAttackNum", frontAttackNum);
                    state = AttackState.Default;
                    //attackElapsedTime = attackCoolTime;
                }
                //ResetAttack();
                //attackElapsedTime = attackCoolTime;
            }

            attackElapsedTime = attackCoolTime;
            DetectPlayer();
        }

        if(isAttacking)
        {
            if(state != AttackState.FrontAttack)
            {
                bang.SetActive(true);
            }
            MovePosition(attackPlayer.transform.position.x);
            attackElapsedTime += Time.deltaTime;
            // 0.5�� ������ ����ǥ ����
            if (0.5f <= attackElapsedTime)
            {
                bang.SetActive(false);
            }
        }
        if (isAttacking && attackCoolTime <= attackElapsedTime)
        {
            AttackPlayer();
            //FrontAttack�϶� ����ؼ� ���ͼ� ���ߴ� ��
            if (state != AttackState.FrontAttack)
            {
                attackElapsedTime = 0;
                isAttacking = false;
            }
            //attackElapsedTime = 0;
            //isAttacking = false;
        }

        if(isHit)
        {
            hitElapsedTime += Time.deltaTime;
            if(hitElapsedTime >= 1.5f)
            {
                comboHitNum = 0;
                isComboHit = false;
                hitElapsedTime = 0;
                isHit = false;
            }
            else
            {
                isComboHit = true;
            }
        }
    }

    // �÷��̾� �����ϴ� �Լ� => �����ϴ� �͵� ������ ��
    void DetectPlayer()
    {
        player = Physics2D.OverlapCircle(transform.position, sightRange, playerMask);
        if (player != null)
        {
            isDetect = true;
            targetPos = player.transform;
            attackPos = targetPos;
            if (isHome)
            {
                // ���� Ÿ�� ����
                isFrontAttack = AttackType(targetPos.rotation.eulerAngles.y, transform.rotation.eulerAngles.y);

                myPlayer.Play(surprisedClip, idleClip, false, false);
                surprised.SetActive(true);
                isHome = false;
            }
            if(myPlayer.isEndofFrame)
            {
                surprised.SetActive(false);
                //attackPos = targetPos;
                // �÷��̾� ����ٴϴ� ���� ����
                MovePosition(targetPos.position.x);
            }
        }
        else
        {
            isDetect = false;
            // �� �ڸ��� �ƴϸ� �ٽ� ���ư���
            if(createdPos.x != transform.position.x && isHome == false)
            {
                // ���ƿ����� �ڸ� ���� ���ƴٴϱ�
                if(Mathf.Abs(createdPos.x - transform.position.x) < homeDistance)
                {
                    transform.position = new Vector2(createdPos.x, transform.position.y);
                    // ������ ��ġ�� ���ư��� ���ƴٴϱ� ���� ��ġ ����
                    destination = new Vector2(transform.position.x + wonderRange, 0);
                    isHome = true;
                }
                // ���� �������� �ʾ�����
                //else
                //{
                //    isHome = false;
                //    // ������� ��ҷ� ���ư��� ���� ����
                //    MovePosition(createdPos.x);
                //}
            }

            // �� �ڸ��� ���ƴٴϱ�(WonderSpotRange)
            if(isHome)
            {
                WonderSpotRange();
            }
            surprised.SetActive(false);
        }
    }

    // 0���� ���ư� ������ �����ؼ� �������� �����̴� ���� ����
    public override void MoveLeft()
    {
        if (transform.rotation.eulerAngles.y <= 0)
        {
            MoveStop();
            myPlayer.Play(uTurnClip, idleClip, true);
            transform.rotation = Quaternion.Euler(0, -180, 0);
        }
        if(myPlayer.isEndofFrame)
        {
            base.MoveLeft();
        }
    }

    // 180���� ���ư� ������ �����ؼ� ���������� �����̴� ���� ����
    public override void MoveRight()
    {
        if (transform.rotation.eulerAngles.y >= 180)
        {
            MoveStop();
            myPlayer.Play(uTurnClip, idleClip, true);
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        if (myPlayer.isEndofFrame)
        {
            base.MoveRight();
        }
    }

    // ���� �������� �ϴ� �ڸ����� �̵���Ű�� �Լ�
    void MovePosition(float targetPosX)
    {
        if(transform.position.x <= targetPosX)
        {
            MoveRight();
        }
        else
        {
            MoveLeft();
        }
    }

    // �÷��̾� ������ ���� ���ƴٴϱ�
    void WonderSpotRange()
    {
        // ���� ��ġ�� �������� �Ÿ� ���̰� 0���� ũ��
        //if ((int)Vector2.Distance(transform.position, destination) > 5)
        if (Mathf.Abs(destination.x - transform.position.x) > homeDistance)
        {
            MovePosition(destination.x);
        }
        else
        {
            // ��ȸ�ϴ� ������ �����
            if (wonderRange > 0)
            {
                // ��ȸ ������ ������ ����
                wonderRange = -wonderRange;
            }
            // ��ȸ�ϴ� ������ ������
            else
            {
                // ��ȸ ������ ����� ����
                wonderRange = Mathf.Abs(wonderRange);
            }
            // destination ����
            destination = new Vector2(createdPos.x + wonderRange, 0);
        }
    }

    bool AttackType(float targetRot, float myRot)
    {
        // ���� �÷��̾ ���� ������ �ٶ󺸰� �ִ� ���
        if (targetRot == myRot)
        {
            return false;
        }
        // ���� �÷��̾ ���ֺ��� �ִ� ���
        else
        {
            return true;
        }
    }

    // ����
    void AttackPlayer()
    {
        // ���� ����
        // 1. (����) ��(Attack01) - ������(Attack02) - ����(Attack03) ������ ���濡 �ָ��� �ֵθ� -> ���ط� 12 - 10 - 11
        // 2. (�Ĺ�) ���� ������ ȸ����Ű�� �յ��� ª�� �Ÿ��� ����(Attack04) -> ���ط� 10
        // 3. (����) �������� ������.(Attack02) -> ���ط� 10

        // ���� ���� �ȿ� ������ ���� �÷��̾� ��ġ�� �����ϴ� ��
        // ���� ���� �ȿ� ������ �� �÷��̾ �տ� ������ ���� ������ �ϰ�
        //                          �÷��̾ �ڿ� ������ �Ĺ� ������ �ϰ�
        //                          �÷��̾ ���� ������ ���� ������ ��

        // ���࿡ �������� ���ڿ�������� state == AttackState.BackAttack���� �ٲ�� ��
        //if(isDetect== false && state != AttackState.FrontAttack)
        //{
        //    attackPos = attackPlayer.transform;

        //    isFrontAttack = AttackType(attackPos.rotation.eulerAngles.y, transform.rotation.eulerAngles.y);
        //}

        if (isDetect == false && isAttackFinish == true)
        {
            attackPos = attackPlayer.transform;

            isFrontAttack = AttackType(attackPos.rotation.eulerAngles.y, transform.rotation.eulerAngles.y);
        }

        if (attackPos.position.y > transform.position.y)
        {
            //Debug.Log("���� ����");
            UpAttack();
        }
        else if(isFrontAttack)
        {
            //Debug.Log("���� ����");
            FrontAttack();
        }
        else
        {
            //Debug.Log("�Ĺ� ����");
            BackAttack();
        }

        isDetect = false;
    }


    void FrontAttack()
    {
        if(isAttackFinish)
        {
            if(frontAttackNum >= 3)
            {
                frontAttackNum = 0;
            }
            frontAttackNum++;
            frontAttack[frontAttackNum - 1].Use(transform, null, new string[] { "Player" }, GameManager.ShowEffect, GameManager.Use, GameManager.GetProjectile);
            myAnimator.SetInteger("FrontAttackNum", frontAttackNum);

            attack.SetActive(true);
            state = AttackState.FrontAttack;
            isAttackFinish = false;
        }
    }

    void BackAttack()
    {
        myAnimator.SetBool("isBackAttack", true);

        attack.SetActive(true);

        StartCoroutine(DoPlay());
        IEnumerator DoPlay()
        {
            yield return new WaitForSeconds(0.5f);
            backAttack.Use(transform, null, new string[] {"Player"}, GameManager.ShowEffect, GameManager.Use, GameManager.GetProjectile);
        }
        state = AttackState.BackAttack;

        isAttackFinish = false;
    }

    void UpAttack()
    {
        myAnimator.SetBool("isUpAttack", true);
        // ���ط� �߰� �ʿ�

        attack.SetActive(true);

        StartCoroutine(DoPlay());
        IEnumerator DoPlay()
        {
            yield return new WaitForSeconds(0.5f);
            upAttack.Use(transform, null, new string[] { "Player" }, GameManager.ShowEffect, GameManager.Use, GameManager.GetProjectile);
        }

        state = AttackState.UpAttack;

        isAttackFinish = false;
    }

    public void ResetAttack()
    {
        bang.SetActive(false);
        attack.SetActive(false);

        isAttackFinish = true;

        switch (state)
        {
            // �ؿ� ������ �� �۵��ϱ� ���ؼ� ���� ���� �ڿ� �ٿ��� ��
            case AttackState.FrontAttack:
                if (frontAttackNum >= 3)
                {
                    attackElapsedTime = 0;
                    isAttacking = false;
                    frontAttackNum = 0;
                    myAnimator.SetInteger("FrontAttackNum", 0);
                    state = AttackState.Default;
                }
                else
                {
                    attackElapsedTime = attackCoolTime;
                    isAttacking = true;
                }
                break;
            case AttackState.UpAttack:
                myAnimator.SetBool("isUpAttack", false);
                state = AttackState.Default;
                break;
            case AttackState.BackAttack:
                myAnimator.SetBool("isBackAttack", false);
                state = AttackState.Default;
                break;
        }
    }

    public void Hit(Strike strike)
    {
        surprised.SetActive(false);
        attack.SetActive(false);
        // �´� �ִϸ��̼� ����
        int result = strike.result;
        hp += result;

        isHit = true;

        // ���� HP>0�̿���
        if(isAlive)
        {
            MovePosition(targetPos.position.x);

            if(isComboHit == true)
            {
                comboHitNum -= result;
            }

            Debug.Log(comboHitNum);
            // Ư�� ���� ���� �� ���� �ɸ�
            if (comboHitNum >= 40)
            {
                myPlayer.Play(stunClip, idleClip, false);
                stun.SetActive(true);
                StartCoroutine(DoTurnOff());
                IEnumerator DoTurnOff()
                {
                    yield return new WaitForSeconds(0.8f);
                    stun.SetActive(false);
                }
            }
            else
            {
                myPlayer.Play(hitClip, idleClip, false);
            }
            
        }

        else
        {
            if(damageCollider.isActiveAndEnabled == true)
            {
                hp = 0;
                damageCollider.enabled = false;
                myRigid.gravityScale = 0;
                for (int i = 0; i < death.Length; i++)
                {
                    death[i].SetActive(true);
                }
                // �״� ���
                myPlayer.Play(deathClip);
                if (gameObject.activeInHierarchy)
                {
                    StartCoroutine(DoDead());
                    IEnumerator DoDead()
                    {
                        yield return new WaitForSeconds(0.8f);
                        gameObject.SetActive(false);
                    }
                }
            }
        }
        GameManager.Report(this, result);
    }

    public Collider2D GetCollider2D()
    {
        return damageCollider;
    }

    public MonoBehaviour GetLootObject()
    {
        return dropItem;
    }
}

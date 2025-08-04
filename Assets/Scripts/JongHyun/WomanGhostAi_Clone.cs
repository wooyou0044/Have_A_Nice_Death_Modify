using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class WomanGhostAi_Clone : Walker,IHittable,ILootable
{
    [SerializeField]
    MonoBehaviour anima;
    bool isDeath = false;
    [SerializeField]
    int womanGhostHP = 50;
    [SerializeField]
    Projectile womanFire;

    [SerializeField]
    Transform Launcher;

    //자식오브젝트
    [SerializeField, Header("자식스크립트")]   
    surpriseImote getSurprisedImote;
    [SerializeField, Header("자식애니메이션")]    
    private Animator surprisedAnimator;
    [SerializeField]
    GameObject find;
    [SerializeField]
    GameObject AttackEffect;

    //애니메이션 클립들
    [SerializeField, Header("애니메이션 클립")]
    private AnimatorPlayer animatorPlayer;
    [SerializeField]
    private AnimationClip idleClip;
    [SerializeField]
    private AnimationClip deathClip;
    [SerializeField]
    private AnimationClip spotteddClip;
    [SerializeField]
    private AnimationClip hitClip;
    [SerializeField]
    private AnimationClip attackClip;
    [SerializeField]
    private AnimationClip uturnClip;

    //적 감지
    [SerializeField,Header("적 감지")]
    public LayerMask playerLayermask;

    [SerializeField, Header("공격범위")] float ghostSightRange;
    Transform target;


    [SerializeField, Header("이동거리")] 
    float moveDistance = 1f;

    IHittable attackPlayer;
    bool isHit = false;

    private bool movingRight = true;
    private Vector2 startPos;
    public bool isAlive { get { return womanGhostHP > 0; } }

    private void Start()
    {
        startPos = transform.position;
        animatorPlayer = GetComponent<AnimatorPlayer>();        
        surprisedAnimator = surprisedAnimator.gameObject.GetComponent<Animator>();
        getSurprisedImote.stop = true;
    }

    private void Update()
    {
        Move();        
        DetectPlayer();
        
        
        //만약 내 하위 오브젝트 불값이 false라면?
        if (getSurprisedImote.stop == false)
        {
            //하위 오브젝트 꺼라
            surprisedAnimator.gameObject.SetActive(false);
        }
    }

    void Move()
    {
        if (movingRight == true)
        {
            MoveRight();
            HandleRotation();
            if (transform.position.x > startPos.x + moveDistance)
            {
                animatorPlayer.Play(uturnClip, idleClip, true);
                movingRight = false;
            }
        }
        else
        {
            MoveLeft();
            HandleRotation();
            if (transform.position.x < startPos.x - moveDistance)
            {
                animatorPlayer.Play(uturnClip, idleClip, true);
                movingRight = true;
            }
        }
    }

    private void HandleRotation()
    {        
        if (movingRight)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }              
    }

    IEnumerator DoFire(float delay)
    {
        while (isHit == false)
        {
            yield return new WaitForSeconds(delay);
            AttackEffect.gameObject.SetActive(false);
            find.gameObject.SetActive(false);
            FireShot();
        }        
    }

    void DetectPlayer()
    {
        Collider2D player;
        player = Physics2D.OverlapCircle(transform.position, ghostSightRange, playerLayermask);
        if (player != null)
        {
            if(target == null)
            {
                animatorPlayer.Play(spotteddClip, true);
                animatorPlayer.animator.SetBool("isAttack",true);
                getSurprisedImote.stop = true;

                attackPlayer = player.GetComponent<IHittable>();
                StartCoroutine(DoFire(1.3f));
                isHit = false;
            }
            target = player.transform;

            ShowExclamationMark();
            SpottedPlayer();
        }
        else if(target != null)
        {
            animatorPlayer.Play(idleClip, true);
            animatorPlayer.animator.SetBool("isAttack", false);
            attackPlayer = target.GetComponent<IHittable>();
            target = null;
            StopAllCoroutines();
        }
    }
    void FireShot()
    {
        AttackEffect.gameObject.SetActive(true);
        find.gameObject.SetActive(true);
        Projectile projectile = GameManager.GetProjectile(womanFire);
        projectile.Shot(Launcher, attackPlayer, GameManager.ShowEffect, GameManager.Use);                
    }
    void SpottedPlayer()
    {        
        MoveStop();
        if (target.position.x <= transform.position.x)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);            
        }
    }
    void ShowExclamationMark()
    {
        if (surprisedAnimator != null)
        {
            // 느낌표 활성화
            surprisedAnimator.gameObject.SetActive(true);
        }
    }

    public void Hit(Strike strike)
    {
        int result = strike.result;
        womanGhostHP += result;
        
        if (womanGhostHP>0)
        {
            MoveStop();
            animatorPlayer.Play(hitClip, true);
            isHit = true;
        }
        else
        {
            animatorPlayer.Play(deathClip, true);
            StartCoroutine(DoHide());
            IEnumerator DoHide()
            {
                yield return new WaitForSeconds(0.3f);
                gameObject.SetActive(false);
            }
        }
        GameManager.Report(this, result);
    }

    public Collider2D GetCollider2D()
    {
        return getCollider2D;
    }

    public MonoBehaviour GetLootObject()
    {
        return anima;
    }
}

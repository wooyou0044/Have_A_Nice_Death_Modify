using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Enemy_Book_AI : Walker, IHittable
{
    public int MaxEnemyHealth;
    public int NowEnemyHealth;
    public ParticleSystem StunEffect;
    Animator EnemyBookAnimator;

    public GameObject FindEffect;
    public GameObject AttackEffect;
    public GameObject SurprisedEffect;

    #region 애니메이션 클립
    public AnimationClip idleClip;
    public AnimationClip uturnClip;
    public AnimationClip findClip;
    public AnimationClip hitClip;
    public AnimationClip attackClip;
    public AnimationClip dieClip;
    #endregion

    float detectTime;
    float BookSightRange;
    bool isFacingRight = true;
    bool isUturn = true;
    bool isFind = true;
    bool isDie = false;
    int facingRight;
    float dieCooltime;
    float EffectCooltime;

    float BookFindCooltime;
    float BookFindElapsedtime;
    float moveCooltime;
    float moveElapsedtime;

    Rigidbody2D enemyRigid;
    Collider2D enemyCollider;
    Collider2D detectplayerErea;
    Vector2 leftEnemyLocation;
    Vector2 nowEnemyLocation;
    Vector2 rightEnemyLocation;

    [SerializeField]
    AnimatorPlayer BookAnimatorPlayer;
    [SerializeField]
    float moveDistance;
    [SerializeField]
    LayerMask playerLayerMask;

    public bool isAlive
    {
        get
        {
            return MaxEnemyHealth > 0;
        }
    }

    void Start()
    {
        MaxEnemyHealth = 20;
        NowEnemyHealth = 20;
        BookSightRange = 8;
        moveCooltime = 3.0f;
        moveElapsedtime = 0;
        BookFindCooltime = 4.0f;
        BookFindElapsedtime = 4.0f;
        moveDistance = 10.0f;
        EnemyBookAnimator = GetComponent<Animator>();
        enemyCollider = GetComponent<Collider2D>();       
        leftEnemyLocation = new Vector2(transform.position.x, transform.position.y);
        BookAnimatorPlayer = GetComponent<AnimatorPlayer>();
    }

    void Update()
    {
        if (NowEnemyHealth <= 0)
        {
            MoveStop();
            Die();
        }

        else if (BookAnimatorPlayer.IsPlaying(hitClip)!= true && BookAnimatorPlayer.IsPlaying(findClip) != true 
            && BookAnimatorPlayer.IsPlaying(attackClip) != true)
        {
            DetectPlayer();
        }     
    }

    void LateUpdate()
    {

    }

    void DetectPlayer()
    {
        detectplayerErea = Physics2D.OverlapCircle(transform.position, BookSightRange, playerLayerMask);

        if (detectplayerErea != null)
        {
            FindPlayer();
        }

        else if (detectplayerErea == null)
        {
            BookWander();
            SurprisedEffect.SetActive(false);
            isFind = true;
            BookFindElapsedtime = 4.0f;
        }
    }

    void FindPlayer()
    {
        MoveStop();
        BookFindElapsedtime += Time.deltaTime;
        EffectCooltime += Time.deltaTime;

        if (isFind == true)
        {
            BookAnimatorPlayer.Play(findClip, idleClip);
            SurprisedEffect.SetActive(true);
            isFind = false;
        }

        if (BookAnimatorPlayer.isEndofFrame && BookFindElapsedtime >= BookFindCooltime)
        {
            BookAnimatorPlayer.Play(attackClip, idleClip);
            AttackEffect.SetActive(true);
            FindEffect.SetActive(true);

            EffectCooltime = 0;
            BookFindElapsedtime = 0;           
            
        }

        if (BookFindElapsedtime >= 2 && BookFindElapsedtime < 3.0f)
        {
            FindEffect.SetActive(false);
            AttackEffect.SetActive(false);
        }
        

        else if (BookAnimatorPlayer.isEndofFrame)
        {
            BookAnimatorPlayer.Play(idleClip);           
        }

    }

    void BookWander()
    {
        nowEnemyLocation = new Vector2(transform.position.x, transform.position.y);

        if (isFacingRight == true)
        {
            MoveRight();
        }

        if (isFacingRight == false)
        {
            MoveLeft();
        }
    }


    public override void MoveRight()
    {
        base.MoveRight();

        if (nowEnemyLocation.x - leftEnemyLocation.x >= moveDistance) 
        {
            MoveStop();
            moveElapsedtime += Time.deltaTime;

            if (moveElapsedtime >= moveCooltime)
            {
                rightEnemyLocation = new Vector2(transform.position.x, transform.position.y);

                if (isUturn == true)
                {
                    BookAnimatorPlayer.Play(uturnClip, idleClip);
                    isUturn = false;
                }

                if(isUturn == false && BookAnimatorPlayer.IsPlaying(uturnClip) != true)
                {
                    Invoke("Turn", 0.36f);
                    isUturn = true;
                    moveElapsedtime = 0;
                    isFacingRight = false;
                }
                
            }
        }        
    }

    public override void MoveLeft()
    {      
        base.MoveLeft();

        if (rightEnemyLocation.x - nowEnemyLocation.x >= moveDistance )
        {
            MoveStop();
            moveElapsedtime += Time.deltaTime;

            if (moveElapsedtime >= moveCooltime)
            {              
                if (isUturn == true)
                {
                    BookAnimatorPlayer.Play(uturnClip, idleClip);
                    isUturn = false;
                }

                if (isUturn == false && BookAnimatorPlayer.IsPlaying(uturnClip) != true)
                {
                    Invoke("Turn", 0.36f);
                    isUturn = true;
                    moveElapsedtime = 0;
                    isFacingRight = true;
                }
            }
        }           
    }

    public void Turn()
    {
        if (isFacingRight == false)
        {
            gameObject.transform.eulerAngles = new Vector3(0, 180, 0);
        }

        else if (isFacingRight == true)
        {
            gameObject.transform.eulerAngles = new Vector3(0, 0, 0);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(this.transform.position, BookSightRange);
    }

    public void Hit(Strike strike)
    {
        NowEnemyHealth += strike.result;

        if (NowEnemyHealth < MaxEnemyHealth / 2 && NowEnemyHealth > 0)
        {
            MoveStop();
            BookAnimatorPlayer.Play(hitClip,idleClip);
            StunEffect.Play();
        }
    }

    public Collider2D GetCollider2D()
    {
        return enemyCollider;
    }

    void Die()
    {
        dieCooltime += Time.deltaTime;

        if(isDie == false)
        {
            BookAnimatorPlayer.Play(dieClip);
            isDie = true;
        }

        if(dieCooltime > 1.0f)
        {
            Destroy(gameObject);
        }
    }
}

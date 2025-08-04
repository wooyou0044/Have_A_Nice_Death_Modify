using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class SkeletonHands : MonoBehaviour, IHittable
{
    [SerializeField]bool alive = true;//살아있나 없나
    [SerializeField]bool detectPlayer;//플레이어 찾아써??
    [SerializeField]Animator animator;
    [SerializeField] LayerMask playerMask;//플레이어 감지용 레이어 마스크
    private Collider2D detectRangeCol;//인식 범위
    private Collider2D attackRangeCol;//공격 범위
    [SerializeField]private bool attackAvailale;//공격 가능 여부
    [SerializeField]int damage;//대미지
    [SerializeField]float detectRange;//인식 범위 수치
    [SerializeField]float attackRange;//공격 범위 수치
    [SerializeField]float responTime = 5f;//부활 시간
    [SerializeField] float attackCoolTime;//공격 후 재시전 까지 걸리는 시간
    private Quaternion rotation = new Quaternion(0,0,0,0);//Rotation.y로 방향 조절
    [SerializeField] IEnumerator currentCoroutine;//코루틴 제어를 위한 변수
    [SerializeField]private Transform imageTransform; //이미지 초기화용 변수
    float left = 0;//회전값
    float right =-180f;//회전값
    private Collider2D thisCol;//IHittable 규격을 위한 자신의 콜라이더
    [SerializeField]private Skill skill;//때리기 위한 스킬


    public bool isAlive//소통용 프로퍼티
    {
        get { return alive; }
        private set { alive = value; }
    }

   

    public void Hit(Strike strike)//피격 당하기
    {
        if (isAlive == true)
        {
            if(currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
            }
            currentCoroutine = IamDead();
            StartCoroutine(currentCoroutine);
        }
    }



    public Collider2D GetCollider2D()//소통용
    {
        return thisCol;
    }

    private void Start()//시작할 때 필요한 것들을 가져오고 또 bool값 켜줌
    {
        animator = GetComponentInChildren<Animator>();
        playerMask = LayerMask.GetMask("Player");
        alive = true;
        thisCol = GetComponent<Collider2D>();
        imageTransform = transform.GetChild(0);
    }

    private void Update() //살아있다면 이 행동을 지속하라
    {
        if(alive == true)
        {
            DetectPlayer();
            if (detectPlayer == true)
            {
                attackCoolTime -= Time.deltaTime;
            }
        }
       
    }

    /// <summary>
    /// 플레이어 감지
    /// </summary>
    void DetectPlayer()
    {
        //플레이어 감지 범위
        detectRangeCol = Physics2D.OverlapCircle(transform.position, detectRange, playerMask);
       
        //플레이어를 감지했다!
        if(detectRangeCol != null)
        {
            detectPlayer = true;
            animator.SetBool("DetectPlayer",true);
            //방향 계산
            float calPosition = detectRangeCol.transform.position.x - transform.position.x;
            //플레이어가 바라보는 방향으로 바꾸기. 그런데 이제 역순
            if (calPosition < 0)
            {
                rotation.y = left;
                transform.rotation = rotation;
            }
            else
            {
                rotation.y = right;
                transform.rotation = rotation;
            }

            //공격 범위 설정
            attackRangeCol = Physics2D.OverlapCircle(transform.position, attackRange, playerMask);
            //공격 범위에 플레이어가 들어와따?
            if (attackRangeCol != null)
            {
                //때릴 수 있어!
                attackAvailale = true;
                if (currentCoroutine == null)//코루틴 실행
                {
                    currentCoroutine = Attack();
                    StartCoroutine(currentCoroutine);
                }
                
            }
            else//때릴 수 없어? 혹은 범위를 벗어났어?
            {
                if (currentCoroutine != null)//때리다가 범위를 벗어난 경우
                {
                    StopCoroutine(currentCoroutine);
                    animator.SetBool("Attack", false);
                }
                attackRangeCol = null;
                currentCoroutine = null;
                attackAvailale = false;
            }
        }
        else//인식 범위에서 벗어나면 초기 상태로 초기화
        {
            if(detectPlayer != false) detectPlayer = false;
            animator.SetBool("DetectPlayer", false);
            attackCoolTime = 0f;

        }


        
       


    }
    /// <summary>
    /// 공격 코루틴
    /// </summary>
    /// <returns></returns>
    IEnumerator Attack()
    {
        
        while(attackAvailale == true)//때릴 수 있는 동안
        {
            if(attackCoolTime < 0)//공격 쿨타임이 돌면
            {

                animator.SetBool("Attack", true);//애니메이션 켜고

                yield return new WaitForSeconds(0.72f);//조금 기다렸다가?
                if (detectRangeCol != null)//플레이어가 여전히 있다면
                {
                    //때린다
                    IHittable hittable = detectRangeCol.transform.GetComponent<IHittable>();
                    skill.Use(transform, hittable, new string[] {"Player"}, GameManager.ShowEffect, GameManager.Use, GameManager.GetProjectile);
                }
                yield return new WaitForSeconds(0.28f);//조금 기다렸다가
                animator.SetBool("Attack", false);//공격 Bool값 꺼주기
                attackCoolTime = 2;//쿨타임 돌리기
            }
            yield return null;

        }
    }
    /// <summary>
    /// 죽을 때 부르는 코루틴
    /// </summary>
    /// <returns></returns>
    IEnumerator IamDead()
    {
        alive = false;//살아있다 꺼주고
        animator.SetTrigger("Dead");//트리거 주고
        animator.SetBool("Regen", true);//리젠 상태에 돌입
        imageTransform.position = transform.position;//위치 다시 돌려주기(죽음 애니메이션 이후 위치가 옮겨지는 일 방지)
        yield return new WaitForSecondsRealtime(responTime);//리스폰 타임 기다렸다가?
        animator.SetBool("Regen", false);//리젠 꺼주고
        alive = true;//살아있다 켜주고
        currentCoroutine = null;//코루틴 해제
    }

}

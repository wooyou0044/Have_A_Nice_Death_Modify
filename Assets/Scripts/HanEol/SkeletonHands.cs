using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class SkeletonHands : MonoBehaviour, IHittable
{
    [SerializeField]bool alive = true;//����ֳ� ����
    [SerializeField]bool detectPlayer;//�÷��̾� ã�ƽ�??
    [SerializeField]Animator animator;
    [SerializeField] LayerMask playerMask;//�÷��̾� ������ ���̾� ����ũ
    private Collider2D detectRangeCol;//�ν� ����
    private Collider2D attackRangeCol;//���� ����
    [SerializeField]private bool attackAvailale;//���� ���� ����
    [SerializeField]int damage;//�����
    [SerializeField]float detectRange;//�ν� ���� ��ġ
    [SerializeField]float attackRange;//���� ���� ��ġ
    [SerializeField]float responTime = 5f;//��Ȱ �ð�
    [SerializeField] float attackCoolTime;//���� �� ����� ���� �ɸ��� �ð�
    private Quaternion rotation = new Quaternion(0,0,0,0);//Rotation.y�� ���� ����
    [SerializeField] IEnumerator currentCoroutine;//�ڷ�ƾ ��� ���� ����
    [SerializeField]private Transform imageTransform; //�̹��� �ʱ�ȭ�� ����
    float left = 0;//ȸ����
    float right =-180f;//ȸ����
    private Collider2D thisCol;//IHittable �԰��� ���� �ڽ��� �ݶ��̴�
    [SerializeField]private Skill skill;//������ ���� ��ų


    public bool isAlive//����� ������Ƽ
    {
        get { return alive; }
        private set { alive = value; }
    }

   

    public void Hit(Strike strike)//�ǰ� ���ϱ�
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



    public Collider2D GetCollider2D()//�����
    {
        return thisCol;
    }

    private void Start()//������ �� �ʿ��� �͵��� �������� �� bool�� ����
    {
        animator = GetComponentInChildren<Animator>();
        playerMask = LayerMask.GetMask("Player");
        alive = true;
        thisCol = GetComponent<Collider2D>();
        imageTransform = transform.GetChild(0);
    }

    private void Update() //����ִٸ� �� �ൿ�� �����϶�
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
    /// �÷��̾� ����
    /// </summary>
    void DetectPlayer()
    {
        //�÷��̾� ���� ����
        detectRangeCol = Physics2D.OverlapCircle(transform.position, detectRange, playerMask);
       
        //�÷��̾ �����ߴ�!
        if(detectRangeCol != null)
        {
            detectPlayer = true;
            animator.SetBool("DetectPlayer",true);
            //���� ���
            float calPosition = detectRangeCol.transform.position.x - transform.position.x;
            //�÷��̾ �ٶ󺸴� �������� �ٲٱ�. �׷��� ���� ����
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

            //���� ���� ����
            attackRangeCol = Physics2D.OverlapCircle(transform.position, attackRange, playerMask);
            //���� ������ �÷��̾ ���͵�?
            if (attackRangeCol != null)
            {
                //���� �� �־�!
                attackAvailale = true;
                if (currentCoroutine == null)//�ڷ�ƾ ����
                {
                    currentCoroutine = Attack();
                    StartCoroutine(currentCoroutine);
                }
                
            }
            else//���� �� ����? Ȥ�� ������ �����?
            {
                if (currentCoroutine != null)//�����ٰ� ������ ��� ���
                {
                    StopCoroutine(currentCoroutine);
                    animator.SetBool("Attack", false);
                }
                attackRangeCol = null;
                currentCoroutine = null;
                attackAvailale = false;
            }
        }
        else//�ν� �������� ����� �ʱ� ���·� �ʱ�ȭ
        {
            if(detectPlayer != false) detectPlayer = false;
            animator.SetBool("DetectPlayer", false);
            attackCoolTime = 0f;

        }


        
       


    }
    /// <summary>
    /// ���� �ڷ�ƾ
    /// </summary>
    /// <returns></returns>
    IEnumerator Attack()
    {
        
        while(attackAvailale == true)//���� �� �ִ� ����
        {
            if(attackCoolTime < 0)//���� ��Ÿ���� ����
            {

                animator.SetBool("Attack", true);//�ִϸ��̼� �Ѱ�

                yield return new WaitForSeconds(0.72f);//���� ��ٷȴٰ�?
                if (detectRangeCol != null)//�÷��̾ ������ �ִٸ�
                {
                    //������
                    IHittable hittable = detectRangeCol.transform.GetComponent<IHittable>();
                    skill.Use(transform, hittable, new string[] {"Player"}, GameManager.ShowEffect, GameManager.Use, GameManager.GetProjectile);
                }
                yield return new WaitForSeconds(0.28f);//���� ��ٷȴٰ�
                animator.SetBool("Attack", false);//���� Bool�� ���ֱ�
                attackCoolTime = 2;//��Ÿ�� ������
            }
            yield return null;

        }
    }
    /// <summary>
    /// ���� �� �θ��� �ڷ�ƾ
    /// </summary>
    /// <returns></returns>
    IEnumerator IamDead()
    {
        alive = false;//����ִ� ���ְ�
        animator.SetTrigger("Dead");//Ʈ���� �ְ�
        animator.SetBool("Regen", true);//���� ���¿� ����
        imageTransform.position = transform.position;//��ġ �ٽ� �����ֱ�(���� �ִϸ��̼� ���� ��ġ�� �Ű����� �� ����)
        yield return new WaitForSecondsRealtime(responTime);//������ Ÿ�� ��ٷȴٰ�?
        animator.SetBool("Regen", false);//���� ���ְ�
        alive = true;//����ִ� ���ְ�
        currentCoroutine = null;//�ڷ�ƾ ����
    }

}

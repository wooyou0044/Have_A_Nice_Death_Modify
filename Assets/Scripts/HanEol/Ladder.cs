using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Ladder : MonoBehaviour
{

    [SerializeField]
    private GameObject player; //�÷��̾� ��ġ
    private Rigidbody2D playerRb; //�÷��̾� ������ٵ�
    [SerializeField] private Walker playerWalker; //�÷��̾� ��Ŀ. isGrounded�˻��
    private Vector3 targetPos = Vector3.zero; //��ٸ� �߽� ��ġ, �÷��̾ �Űܾ��ؼ� Vector3
    private IEnumerator currentCoroutine; //�ڷ�ƾ �ߺ� ���� ����
    private Vector2 getLadder = new Vector2(0, 0); //��ٸ� �ö󰡴� ����� �ӵ�
    private Vector2 gettingDownLadder = new Vector2(0, 0);//��ٸ� �������� ����� �ӵ�
    [SerializeField] private float moveUpSpeed = 25f; //�ӵ�
    private bool isLadder = false; //��ٸ� �ȿ� ������ true, �����ų� �ƴϸ� false
    [SerializeField]float originalGravity; //�������� ���� �߷� ��
    private bool movingUp = false;//�ö󰡴��� �ƴ��� �����ϴ� ����
    float time = 0f;//�ߺ� ���� ������ ���� ����
    [SerializeField] float enteryTime = 0.5f;
    //public bool MovingUp//���� �ִϸ��̼� ���� ������ ���ǹ��� �ɾ ���� �����ϸ�
    //    //�̰� �ʿ� ������ 
    //{
    //    get { return movingUp; }
    //}

    private void Update()//�ߺ� ������
    {
       if(time > 0f)
        {
            time -= Time.deltaTime;
            if(time < 0f)
            {
                time = 0;
            }
        }
    }
    /// <summary>
    /// ��ٸ����� �ö� �� ���� �Լ�
    /// </summary>
    public bool MoveUp()
    {
        //�ߺ� ������ �˻�
        if(time > 0f)
        {
            return false;
        }
        if (player == null)
        {
            isLadder = false;
            return isLadder;
        }

        //�ѹ��� �����ϱ� ���� �ڵ�
        if (movingUp == false)
        {
            if(currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
                currentCoroutine = null;
            }

            if(currentCoroutine == null)
            {
                currentCoroutine = GettingUp();
                StartCoroutine(currentCoroutine);
            }
            
            
        }
        return isLadder;


    }
    /// <summary>
    /// ��ٸ����� Ű�� ���� �����ϴ� �޼ҵ�. �÷��̾� ������ bool�� �Ѱ���
    /// </summary>
    public bool MoveStop()
    {
        if (player == null)
        {
            isLadder = false;
            return isLadder;
        }
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
            if(movingUp == true) movingUp = false;
        }
        //�����ϸ� ��ٸ����� �ȳ����� �̷��� ��
        if (playerRb.gravityScale != originalGravity)
        {
            playerRb.gravityScale = originalGravity;
        }
        //��� ���������� �ڷ�ƾ ������ ���´� <-- Ÿ�̸ӷ� �ð��� �缭 
        //�ð��� ������ �� �ڿ� �ڷ�ƾ�� �ް�/ ���� ������
        if(isLadder == true)
        {
            time = enteryTime;
        }
        return isLadder;
    }
   

    public bool MoveDown()
    {
        if(time > 0f)
        {
            return false;
        }
        if (player == null)
        {
            isLadder = false;
            return isLadder;
        }
       
        if (movingUp == true)
        {
            
            if(currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
                currentCoroutine = null;
            }
            
            if(currentCoroutine == null)
            {
                currentCoroutine = GettingDown();
                StartCoroutine(currentCoroutine);
            }
            
            
        }
        return isLadder;
    }
    /// <summary>
    /// ��ٸ� Ÿ�� ���� �������� �ڷ�ƾ
    /// </summary>
    /// <returns></returns>
    IEnumerator GettingDown()
    {
        while (isLadder == true && playerWalker.isGrounded == false)
        {
            if(movingUp != false)
            {
                movingUp = false;
            }
            playerRb.velocity = gettingDownLadder;
            yield return null;
        }
        //���ٴڿ� ������ �ʱ�ȭ
        if(playerWalker.isGrounded == true)
        {
            playerRb.gravityScale = originalGravity; 
        }
        currentCoroutine = null;
    }



    /// ��ٸ��� �ö󰡴� �ڷ�ƾ
    /// </summary>
    /// <returns></returns>
    IEnumerator GettingUp()
    {
        //���� ����� �߷� ����
        if (playerRb.gravityScale != 0.001f)
        {
            
            playerRb.gravityScale = 0.001f;
        }
        //��ġ�� �߰����� �̵�
        if (player.transform.position != targetPos)
        {
            player.transform.position = targetPos;
        }
      


        while (isLadder == true)
        {
            if(movingUp != true)
            {
                movingUp = true;
            }
            playerRb.velocity = getLadder;
            yield return null;//�Ⱦ��� Boom
        }
        currentCoroutine = null;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") == true)
        {
            //�÷��̾ null�̸� �ʱ�ȭ
            if (player == null)
            {
                player = collision.gameObject;
                playerRb = player.GetComponent<Rigidbody2D>();
                isLadder = true;
                playerWalker = player.GetComponent<Player>();
                //�߷°� ����
                originalGravity = playerRb.gravityScale;
            }
            //�ö󰡴� ���� �� ����
            if (getLadder.y != moveUpSpeed)
            {
                getLadder.y = moveUpSpeed;
            }
            //�������� ���� �� ����
            if (gettingDownLadder.y != -moveUpSpeed)
            {
                gettingDownLadder.y -= moveUpSpeed;
            }
            //��� �� ���ϱ� + �÷��̾� ���� ������
            if (targetPos.x != transform.position.x)
            {
                targetPos.x = transform.position.x;
            }
            if (targetPos.y != player.transform.position.y)
            {
                targetPos.y = player.transform.position.y;
            }


        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //�÷��̾ ������ �ʱ�ȭ
        if (collision.CompareTag("Player") && player != null)
        {
            isLadder = false;
            //��ٸ����� ����� �ٽ� ������ �� �ְ� ��
            if (playerRb.gravityScale != originalGravity)
            {

                playerRb.gravityScale = originalGravity;

            }
            if(movingUp == true) movingUp = false;
            playerRb = null;
            playerWalker = null;
            player = null;
            originalGravity = 0f;
        }
    }
}

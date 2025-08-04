using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Ladder : MonoBehaviour
{

    [SerializeField]
    private GameObject player; //플레이어 위치
    private Rigidbody2D playerRb; //플레이어 리지드바디
    [SerializeField] private Walker playerWalker; //플레이어 워커. isGrounded검사용
    private Vector3 targetPos = Vector3.zero; //사다리 중심 위치, 플레이어를 옮겨야해서 Vector3
    private IEnumerator currentCoroutine; //코루틴 중복 실행 방지
    private Vector2 getLadder = new Vector2(0, 0); //사다리 올라가는 방향과 속도
    private Vector2 gettingDownLadder = new Vector2(0, 0);//사다리 내려가는 방향과 속도
    [SerializeField] private float moveUpSpeed = 25f; //속도
    private bool isLadder = false; //사다리 안에 들어오면 true, 나가거나 아니면 false
    [SerializeField]float originalGravity; //복구해줄 원본 중력 값
    private bool movingUp = false;//올라가는지 아닌지 구분하는 변수
    float time = 0f;//중복 적용 방지를 위한 변수
    [SerializeField] float enteryTime = 0.5f;
    //public bool MovingUp//만약 애니메이션 제어 값으로 조건문을 걸어서 실행 가능하면
    //    //이거 필요 없어짐 
    //{
    //    get { return movingUp; }
    //}

    private void Update()//중복 방지용
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
    /// 사다리에서 올라갈 때 쓰는 함수
    /// </summary>
    public bool MoveUp()
    {
        //중복 방지용 검사
        if(time > 0f)
        {
            return false;
        }
        if (player == null)
        {
            isLadder = false;
            return isLadder;
        }

        //한번만 실행하기 위한 코드
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
    /// 사다리에서 키를 눌러 정지하는 메소드. 플레이어 유무를 bool값 넘겨줌
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
        //점프하면 사다리에서 안나가도 이렇게 됨
        if (playerRb.gravityScale != originalGravity)
        {
            playerRb.gravityScale = originalGravity;
        }
        //어느 시점까지는 코루틴 실행을 막는다 <-- 타이머로 시간을 재서 
        //시간이 지나면 그 뒤에 코루틴을 받고/ 멈춘 동안은
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
    /// 사다리 타는 도중 내려가는 코루틴
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
        //땅바닥에 닿으면 초기화
        if(playerWalker.isGrounded == true)
        {
            playerRb.gravityScale = originalGravity; 
        }
        currentCoroutine = null;
    }



    /// 사다리를 올라가는 코루틴
    /// </summary>
    /// <returns></returns>
    IEnumerator GettingUp()
    {
        //최초 실행시 중력 제거
        if (playerRb.gravityScale != 0.001f)
        {
            
            playerRb.gravityScale = 0.001f;
        }
        //위치를 중간으로 이동
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
            yield return null;//안쓰면 Boom
        }
        currentCoroutine = null;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") == true)
        {
            //플레이어가 null이면 초기화
            if (player == null)
            {
                player = collision.gameObject;
                playerRb = player.GetComponent<Rigidbody2D>();
                isLadder = true;
                playerWalker = player.GetComponent<Player>();
                //중력값 저장
                originalGravity = playerRb.gravityScale;
            }
            //올라가는 벡터 값 설정
            if (getLadder.y != moveUpSpeed)
            {
                getLadder.y = moveUpSpeed;
            }
            //내려가는 벡터 값 설정
            if (gettingDownLadder.y != -moveUpSpeed)
            {
                gettingDownLadder.y -= moveUpSpeed;
            }
            //가운데 값 구하기 + 플레이어 높이 가지기
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
        //플레이어가 나가면 초기화
        if (collision.CompareTag("Player") && player != null)
        {
            isLadder = false;
            //사다리에서 벗어나면 다시 움직일 수 있게 함
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

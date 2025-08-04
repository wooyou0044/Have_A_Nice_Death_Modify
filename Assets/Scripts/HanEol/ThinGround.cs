using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThinGround : MonoBehaviour
{
    private BoxCollider2D topCollider;
    private BoxCollider2D[] colls;
    [SerializeField]GameObject player;
    [SerializeField]Rigidbody2D playerRb;
    static Vector2 uping = new Vector2(0, 0);
    [SerializeField]float Speed = 20f;
    [SerializeField]private bool gonnaUp = true; //밑 콜라이더에 닿으면 올릴지 말지를 결정하는 코드



    private void Start()
    {
        colls = GetComponents<BoxCollider2D>();
        foreach(BoxCollider2D b in colls)
        {
            if(b.offset.y > 0)
            {
                topCollider = b;
            }
        }
        //Vector2 값 넣어주시고
        if (uping.y != Speed)
        {
            uping.y = Speed;
        }
    }
    //private void Update()
    //{
    //    if(player != null)
    //    {
    //        if (Input.GetKey(KeyCode.S)) MoveDownThinGround();
    //    }
    //}

    public bool MoveDownThinGround()
    {
        
        if(player == null) return false;
        if (gonnaUp == true)
        {
            gonnaUp = false;
        }
        topCollider.isTrigger = true;
        return true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //플레이어면 가져오고
        if(collision.gameObject.CompareTag("Player") == true) //땅 밟았을 때
        {
            //처음 플레이어 가져오는거면
            if(player == null)
            {
                player = collision.gameObject;
                playerRb = player.GetComponent<Rigidbody2D>();
                gonnaUp = false;
                
            }
            
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player") == true)
        {
            if (player != null)
            {
                playerRb = null;
                player = null;
                gonnaUp = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) //밑에서 올라올 때
    {
        if (collision.gameObject.CompareTag("Player") == true)
        {
            //처음 플레이어 가져오는거면
            if (player == null)
            {
                player = collision.gameObject;
                playerRb = player.GetComponent<Rigidbody2D>();
                
                if(gonnaUp == true)
                {
                    playerRb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
                    playerRb.velocity = uping;
                    topCollider.isTrigger = true;
                    gonnaUp = false;
                }
                

            }

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") == true)
        {
            
            if(gonnaUp == false)//내려가는 상황일 때 실행 됨
            {
                if (player != null)
                {
                    playerRb.constraints = RigidbodyConstraints2D.None | RigidbodyConstraints2D.FreezeRotation;
                    playerRb = null;
                    player = null;
                    gonnaUp = true;
                    topCollider.isTrigger = true;
                    
                }
            }
            //올라가는 상황일 때 실행
            if(topCollider.isTrigger == true)
            {
                topCollider.isTrigger = false;
            }
            
        }




    }
}

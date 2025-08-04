using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ElevatorInPlayer : MonoBehaviour
{
    [SerializeField,Header("화면전환")]
    GameObject FadeIn;
    [SerializeField]Image FadeInImage;

    [SerializeField,Header("엘레베이터")]
    SpriteRenderer ElevatorSR;
    [SerializeField]Animator Elevator;
    [SerializeField]Animator InPlayer;

    [SerializeField,Header("플레이어")]
    GameObject Player;
    [SerializeField,Header("F버튼")]
    GameObject InteractButton;

    private void Start()
    {
        Elevator.speed = 0f;
        InPlayer.speed = 0f;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            InteractButton.SetActive(true);

            if (Input.GetKeyDown(KeyCode.F))
            {
                InPlayer.gameObject.SetActive(true);
                Elevator.speed = 1f;
                InPlayer.speed = 1f;
            }
        }        
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        InteractButton?.SetActive(false);
    }
    private void FixedUpdate()
    {
        if(ElevatorSR.color.r == 0)
        {
            FadeIn.gameObject.SetActive(true);
            if (FadeInImage.color.r == 0)
            {
                SceneManager.LoadScene("BossTestRoom");
            }
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ElevatorInPlayer : MonoBehaviour
{
    [SerializeField,Header("ȭ����ȯ")]
    GameObject FadeIn;
    [SerializeField]Image FadeInImage;

    [SerializeField,Header("����������")]
    SpriteRenderer ElevatorSR;
    [SerializeField]Animator Elevator;
    [SerializeField]Animator InPlayer;

    [SerializeField,Header("�÷��̾�")]
    GameObject Player;
    [SerializeField,Header("F��ư")]
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterSpot : MonoBehaviour
{
    bool isPlayerEnter;
    [SerializeField] GameObject canvas;

    public bool IsEnter
    {
        get
        {
            return isPlayerEnter;
        }
        set
        {
            isPlayerEnter = value;
        }
    }

    void Start()
    {
        isPlayerEnter = false;
    }


    // 플레이어가 들어오면 Canvas가 SetActive됨
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            canvas.SetActive(true);
            isPlayerEnter = true;
        }
    }
}

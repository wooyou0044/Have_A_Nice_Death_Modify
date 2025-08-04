using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paperplane_controller : MonoBehaviour
{
    Rigidbody2D planeRigid;
    Vector2 Destiantion;
    GameObject playerInfo;
    float planeSpeed;

    void Start()
    {
        playerInfo = GameObject.FindWithTag("Player");
        planeSpeed = 5.0f;
    }


    void Update()
    {
        if(playerInfo != null)
        {
            Destiantion = (playerInfo.transform.position - transform.position);

            Destiantion.Normalize();

            planeRigid.AddForce(Destiantion * planeSpeed, ForceMode2D.Force);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
    }
}

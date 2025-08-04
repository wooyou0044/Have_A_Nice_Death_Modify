using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCamera : MonoBehaviour, ITriggerShake
{
    public enum BossCameraMode
    {
        Follow,Shake,BossRoom
    }
    public BossCameraMode currentMode = BossCameraMode.Follow;
    public GameObject Player;
    public GameObject BossRoomWall;
    Vector3 bossRoomPos;
    Vector3 originalPos;
    float shakeElapsed = 0;

    //[SerializeField]
    //GameObject BossHP;
    public bool isArrive = false;

    private void Start()
    {
        bossRoomPos = new Vector3(5.4f, 1.6f, -10);
        originalPos = transform.position;
    }
    private void Update()
    {
        switch (currentMode)
        {
            case BossCameraMode.Follow:
                FollowPlayer();
                break;
            case BossCameraMode.Shake:
                ShakeCamera();
                break;
            case BossCameraMode.BossRoom:
                break;
        }
             
        
    }
    void FollowPlayer()
    {        
        if (transform.position.x >= -8f)
        {
            Camera.main.orthographicSize = 9;
            transform.position = Vector3.Lerp(transform.position, bossRoomPos, Time.deltaTime * 4f);
            if (Vector3.Distance(transform.position, bossRoomPos) <= 1f)
            {
                transform.position = bossRoomPos;                
                BossRoomWall.SetActive(true);
                currentMode = BossCameraMode.BossRoom;
                //BossHP.gameObject.SetActive(true);
                isArrive = true;

            }
        }
        else
        {
            Vector3 targetPos = new Vector3(Player.transform.position.x, 0, -10);
            MaxDistanceCamera();
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * 2f);
        }
    }
    void MaxDistanceCamera()
    {
        if(transform.position.x <= -44f)
        {
            transform.position = new Vector3(-44f, transform.position.y, -10);
        }
        if (transform.position.y <= 0f)
        {
            transform.position = new Vector3(transform.position.x, 0f, -10);
        }
        else if(transform.position.y >= 0.5f)
        {
            transform.position = new Vector3(transform.position.x, 0.5f, -10);
        }        
    }
    void ShakeCamera()
    {
        originalPos = bossRoomPos;
        if (shakeElapsed < 0.3f)
        {
            shakeElapsed += Time.deltaTime;
            Vector3 randomShake = new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), 0);
            transform.position = originalPos + randomShake;
        }
        else
        {
            shakeElapsed = 0f;
            transform.position = originalPos;
            currentMode = BossCameraMode.BossRoom;
        }
    }
    public void TriggerShake()
    {
        shakeElapsed = 0f;
        currentMode = BossCameraMode.Shake;
    }
}

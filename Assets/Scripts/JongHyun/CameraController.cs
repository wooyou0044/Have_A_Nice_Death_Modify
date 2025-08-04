using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CameraController : MonoBehaviour, ITriggerShake
{
    public enum CameraMode
    {
        Follow,Shake
    }
    public CameraMode currentMode = CameraMode.Follow;

    [SerializeField]
    Transform Player;
    Vector3 CameraOffset = new Vector3(0, 2, -10);
    Vector3 originalPos;

    float shakeElapsed = 0f;
    void Start()
    {
        originalPos = transform.position;
    }

    
    void Update()
    {
        switch(currentMode)
        {
            case CameraMode.Follow:
                FollowPlayer();
                break;
            case CameraMode.Shake:
                ShakeCamera();
                break;
        }
    }
    void FollowPlayer()
    {
        Vector3 targetPos = new Vector3(Player.transform.position.x, Player.transform.position.y + 3, -10);
        targetPos.x = Mathf.Clamp(targetPos.x,-6f, 316f);
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * 4f);
        originalPos = targetPos;
    }
    void ShakeCamera()
    {
        if (shakeElapsed < 0.3f)
        {
            shakeElapsed += Time.deltaTime;
            Vector3 randomShake = new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f),0);
            transform.position = originalPos + randomShake;
        }
        else
        {
            shakeElapsed = 0f;
            transform.position = originalPos;
            currentMode = CameraMode.Follow;
        }
    }
    public void TriggerShake()
    {
        originalPos = transform.position;
        currentMode = CameraMode.Shake;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    Transform cameraTransform;
    [SerializeField]
    float backgroundSpeed;
    private Vector3 offset; // 배경과 카메라 사이의 초기 거리

    void Start()
    {
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform; // 메인 카메라를 기본으로 설정
        }

        offset = transform.position;
    }

    void Update()
    {
        transform.position = cameraTransform.position * backgroundSpeed + offset;
    }
}

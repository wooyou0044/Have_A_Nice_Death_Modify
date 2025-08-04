using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeColorA : MonoBehaviour
{
    Color TempColor; //색 자료형
    bool isForward; //정방향 여부

    void Start()
    {
        TempColor = new Color(1, 1, 1, 0); //알파는 0으로 초기화
        isForward = true; //정방향이 맞다고 참으로 초기화
    }

    void Update()
    {
        if (isForward == true) //정방향이면?
        {
            TempColor.a += 0.01f; //0.01을 증가시킨다

            if (TempColor.a >= 1.0f) //1에 도달했거나 넘었다면?
                isForward = false; //정방향 아니라고 알림
        }

        if (isForward == false) //정방향이 아니라면?
        {
            TempColor.a -= 0.01f; //0.01을 깎는다

            if (TempColor.a <= 0.0f) //0이하 도달하면?
                isForward = true; //다시 정방향
        }

        //반영 부분
        GetComponent<Text>().color = TempColor; //설명 참고
    }
}

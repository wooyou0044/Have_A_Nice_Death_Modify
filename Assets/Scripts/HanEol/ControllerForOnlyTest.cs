using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 키보드 입력을 이용하여 플레이어를 조종할 수 있는 클래스
/// </summary>
public class ControllerForOnlyTest : MonoBehaviour
{
    //오른쪽 이동 방향
    private const bool RightDirection = true;
    //왼쪽 이동 방향
    private const bool LeftDirection = false;

    //조종할 대상 플레이어
    public PlayerForOnlyTest player;

    /// <summary>
    /// 특정 키보드 입력이 이루어졌는지 확인하고 그 결과를 반환하는 구조체 
    /// </summary>
    [Serializable]
    private struct Input
    {
        [SerializeField]
        public KeyCode[] keyCodes;

        public bool isPressed
        {
            set;
            get;
        }
    }

    //방향키 ↑
    [SerializeField, Header("방향키 ↑")]
    private Input upInput;
    //방향키 
    [SerializeField, Header("방향키 ↓")]
    private Input downInput;
    //방향키 ←
    [SerializeField, Header("방향키 ←")]
    private Input leftInput;
    //방향키 →
    [SerializeField, Header("방향키 →")]
    private Input rightInput;
    //점프
    [SerializeField, Header("점프")]
    private Input jumpInput;
    //상호 작용
    [SerializeField, Header("상호 작용")]
    private Input interactionInput;
    //체력 회복
    [SerializeField, Header("체력 회복")]
    private Input healInput;
    //궁극기
    [SerializeField, Header("궁극기")]
    private Input lethalInput;
    //공격 1
    [SerializeField, Header("공격 1")]
    private Input attack1Input;
    //공격 2
    [SerializeField, Header("공격 2")]
    private Input attack2Input;
    //공격 3
    [SerializeField, Header("공격 3")]
    private Input attack3Input;

    //캔버스가 활성화 될 때는 캔버스를 선택하는 내용으로 바꿈

    private void Update()
    {
        Set(ref upInput);
        Set(ref downInput);
        Set(ref leftInput);
        Set(ref rightInput);
        Set(ref jumpInput);

        Set(ref healInput);
        Set(ref lethalInput);
        Set(ref attack1Input);
        Set(ref attack2Input);
        Set(ref attack3Input);
        if (player != null)
        {
            if(rightInput.isPressed != leftInput.isPressed)
            {
                switch (rightInput.isPressed)
                {
                    case RightDirection:
                        player.MoveRight();
                        break;
                    case LeftDirection:
                        player.MoveLeft();
                        break;
                }
            }
            else
            {
                player.MoveStop();
            }
            if (upInput.isPressed == true)
            {
                //if()
                player.MoveUp();
            }
            if (jumpInput.isPressed == true)
            {
                if (downInput.isPressed == true)
                {
                    player.MoveDown();
                }
                else
                {
                    player.Jump();
                }
            }
            if(interactionInput.isPressed == true)
            {

            }
            if (healInput.isPressed == true)
            {
                player.Heal();
            }
            if(lethalInput.isPressed == true)
            {
                player.UseLethalMove();
            }
            if (attack1Input.isPressed == true)
            {
                player.Attack1();
            }
            if (attack2Input.isPressed == true)
            {
                player.Attack2();
            }
            if (attack3Input.isPressed == true)
            {
                player.Attack3();
            }
        }
        upInput.isPressed = false;
        downInput.isPressed = false;
        leftInput.isPressed = false;
        rightInput.isPressed = false;
        jumpInput.isPressed = false;
        interactionInput.isPressed = false;
        healInput.isPressed = false;
        lethalInput.isPressed = false;
        attack1Input.isPressed = false;
        attack2Input.isPressed = false;
        attack3Input.isPressed = false;
    }

    /// <summary>
    /// 특정 키보드 입력을 받는 객체에 해당 키가 눌러졌는지 확인하고 결과를 넣어주는 메서드
    /// </summary>
    /// <param name="input"></param>
    /// 

    //임시본
    private void Set(ref Input input)
    {
        int length = input.keyCodes != null ? input.keyCodes.Length : 0;
        for (int i = 0; i < length; i++)
        {
            if (UnityEngine.Input.GetKey(input.keyCodes[i]) == true)
            {
                //원본
                input.isPressed = true;
                break;
            }
        }
    }
}


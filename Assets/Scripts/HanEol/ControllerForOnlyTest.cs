using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Ű���� �Է��� �̿��Ͽ� �÷��̾ ������ �� �ִ� Ŭ����
/// </summary>
public class ControllerForOnlyTest : MonoBehaviour
{
    //������ �̵� ����
    private const bool RightDirection = true;
    //���� �̵� ����
    private const bool LeftDirection = false;

    //������ ��� �÷��̾�
    public PlayerForOnlyTest player;

    /// <summary>
    /// Ư�� Ű���� �Է��� �̷�������� Ȯ���ϰ� �� ����� ��ȯ�ϴ� ����ü 
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

    //����Ű ��
    [SerializeField, Header("����Ű ��")]
    private Input upInput;
    //����Ű 
    [SerializeField, Header("����Ű ��")]
    private Input downInput;
    //����Ű ��
    [SerializeField, Header("����Ű ��")]
    private Input leftInput;
    //����Ű ��
    [SerializeField, Header("����Ű ��")]
    private Input rightInput;
    //����
    [SerializeField, Header("����")]
    private Input jumpInput;
    //��ȣ �ۿ�
    [SerializeField, Header("��ȣ �ۿ�")]
    private Input interactionInput;
    //ü�� ȸ��
    [SerializeField, Header("ü�� ȸ��")]
    private Input healInput;
    //�ñر�
    [SerializeField, Header("�ñر�")]
    private Input lethalInput;
    //���� 1
    [SerializeField, Header("���� 1")]
    private Input attack1Input;
    //���� 2
    [SerializeField, Header("���� 2")]
    private Input attack2Input;
    //���� 3
    [SerializeField, Header("���� 3")]
    private Input attack3Input;

    //ĵ������ Ȱ��ȭ �� ���� ĵ������ �����ϴ� �������� �ٲ�

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
    /// Ư�� Ű���� �Է��� �޴� ��ü�� �ش� Ű�� ���������� Ȯ���ϰ� ����� �־��ִ� �޼���
    /// </summary>
    /// <param name="input"></param>
    /// 

    //�ӽú�
    private void Set(ref Input input)
    {
        int length = input.keyCodes != null ? input.keyCodes.Length : 0;
        for (int i = 0; i < length; i++)
        {
            if (UnityEngine.Input.GetKey(input.keyCodes[i]) == true)
            {
                //����
                input.isPressed = true;
                break;
            }
        }
    }
}


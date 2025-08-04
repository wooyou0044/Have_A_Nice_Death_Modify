using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeColorA : MonoBehaviour
{
    Color TempColor; //�� �ڷ���
    bool isForward; //������ ����

    void Start()
    {
        TempColor = new Color(1, 1, 1, 0); //���Ĵ� 0���� �ʱ�ȭ
        isForward = true; //�������� �´ٰ� ������ �ʱ�ȭ
    }

    void Update()
    {
        if (isForward == true) //�������̸�?
        {
            TempColor.a += 0.01f; //0.01�� ������Ų��

            if (TempColor.a >= 1.0f) //1�� �����߰ų� �Ѿ��ٸ�?
                isForward = false; //������ �ƴ϶�� �˸�
        }

        if (isForward == false) //�������� �ƴ϶��?
        {
            TempColor.a -= 0.01f; //0.01�� ��´�

            if (TempColor.a <= 0.0f) //0���� �����ϸ�?
                isForward = true; //�ٽ� ������
        }

        //�ݿ� �κ�
        GetComponent<Text>().color = TempColor; //���� ����
    }
}

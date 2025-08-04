using System;
using UnityEngine;

/// <summary>
/// �������� ���ϴ� Ȯ�� �޼��带 �߰��� ����� ���� �� �ֱ� ������ partial�� ������ ���Ҵ�.
/// </summary>
public static partial class ExtensionMethod
{
    /// <summary>
    /// ����� �ִϸ����� �ڵ鷯���� Ư�� �Ķ���͸� ���� ��ų �� ���� �Լ�
    /// </summary>
    /// <param name="parameter"></param>
    /// <param name="animator"></param>
    /// <param name="key"></param>
    public static void SetState(this Parameter parameter, Animator animator, string key)
    {
        if (parameter != null)
        {
            parameter.Set(animator, key);
        }
        else
        {
            animator?.SetTrigger(key);
        }
    }

}
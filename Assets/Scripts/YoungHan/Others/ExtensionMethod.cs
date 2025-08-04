using System;
using UnityEngine;

/// <summary>
/// 팀원들이 원하는 확장 메서드를 추가로 만들고 싶을 수 있기 때문에 partial로 설정해 놓았다.
/// </summary>
public static partial class ExtensionMethod
{
    /// <summary>
    /// 컨디션 애니메이터 핸들러에서 특정 파라미터를 동작 시킬 때 쓰는 함수
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
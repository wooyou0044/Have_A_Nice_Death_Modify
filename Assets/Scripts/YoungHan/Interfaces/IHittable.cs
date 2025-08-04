using UnityEngine;

/// <summary>
/// 피격 당할 수 있는 객체들이 상속 받는 인터페이스
/// </summary>
public interface IHittable
{
    /// <summary>
    /// 상속 받은 대상이 살아있는지 유무를 확인하는 프로퍼티
    /// </summary>
    public bool isAlive
    {
        get;
    }

    /// <summary>
    /// 상속 받은 대상이 지니는 특정 태그 이름
    /// </summary>
    public string tag
    {
        set;
        get;
    }

    /// <summary>
    /// 상속 받은 대상의 트랜스폼
    /// </summary>
    public Transform transform
    {
        get;
    }

    /// <summary>
    /// 상속 받은 대상이 피격 당할 때 사용하는 함수
    /// </summary>
    /// <param name="strike"></param>
    public void Hit(Strike strike);

    /// <summary>
    /// 상속 대상의 피격 범위를 반환할 함수
    /// </summary>
    /// <returns>상속 대상의 피격 콜라이더를 반환함</returns>
    public Collider2D GetCollider2D();
}
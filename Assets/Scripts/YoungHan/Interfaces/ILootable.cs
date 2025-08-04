using UnityEngine;

/// <summary>
/// 아이템을 가지고 있는 몬스터들이 전리품을 떨어뜨릴 수 있도록 유도하는 인터페이스
/// </summary>
public interface ILootable
{
    /// <summary>
    /// 떨어뜨릴 아이템 프리팹을 반환한다.
    /// </summary>
    /// <returns></returns>
    MonoBehaviour GetLootObject();
}

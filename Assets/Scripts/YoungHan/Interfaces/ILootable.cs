using UnityEngine;

/// <summary>
/// �������� ������ �ִ� ���͵��� ����ǰ�� ����߸� �� �ֵ��� �����ϴ� �������̽�
/// </summary>
public interface ILootable
{
    /// <summary>
    /// ����߸� ������ �������� ��ȯ�Ѵ�.
    /// </summary>
    /// <returns></returns>
    MonoBehaviour GetLootObject();
}

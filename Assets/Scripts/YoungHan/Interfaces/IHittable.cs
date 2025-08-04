using UnityEngine;

/// <summary>
/// �ǰ� ���� �� �ִ� ��ü���� ��� �޴� �������̽�
/// </summary>
public interface IHittable
{
    /// <summary>
    /// ��� ���� ����� ����ִ��� ������ Ȯ���ϴ� ������Ƽ
    /// </summary>
    public bool isAlive
    {
        get;
    }

    /// <summary>
    /// ��� ���� ����� ���ϴ� Ư�� �±� �̸�
    /// </summary>
    public string tag
    {
        set;
        get;
    }

    /// <summary>
    /// ��� ���� ����� Ʈ������
    /// </summary>
    public Transform transform
    {
        get;
    }

    /// <summary>
    /// ��� ���� ����� �ǰ� ���� �� ����ϴ� �Լ�
    /// </summary>
    /// <param name="strike"></param>
    public void Hit(Strike strike);

    /// <summary>
    /// ��� ����� �ǰ� ������ ��ȯ�� �Լ�
    /// </summary>
    /// <returns>��� ����� �ǰ� �ݶ��̴��� ��ȯ��</returns>
    public Collider2D GetCollider2D();
}
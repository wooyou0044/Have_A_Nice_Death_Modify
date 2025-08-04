using System;
using UnityEngine;

/// <summary>
/// ������ �����ϰ� �Ǵ� �÷��̾� Ŭ����
/// </summary>
public sealed class PlayerForOnlyTest : Runner, IHittable
{
    private static readonly Vector2 LeftRotation = new Vector2(0, 180);
    private static readonly Vector2 RightRotation = new Vector2(0, 0);

    public enum Interaction
    {
        Pick,
        MoveUp,
        MoveDown
    }

    [SerializeField]
    private Animator _animator = null;

    public bool isAlive
    {
        get;
    }

    //�ǰ� �׼� ��������Ʈ
    private Action<IHittable, int> _hitAction = null;
    //��ų ��� �׼� ��������Ʈ
    
    //private Func<Interaction, bool> _interactionFunction = null;

    public void Initialize(Action<IHittable, int> hit)
    {
        _hitAction = hit;
        //_strikeAction = strike;
        //_interactionFunction = interaction;
    }

    public void Heal()
    {

    }

    public void UseLethalMove()
    {

    }

    [SerializeField]
    private Projectile projectile;

    public void Attack1()
    {
        Projectile sample = Instantiate(projectile);
        //sample.Shot(new Strike(), null, position, getTransform.rotation, null);
    }

    public void Attack2()
    {

    }

    public void Attack3()
    {

    }

    public override void MoveLeft()
    {
        base.MoveLeft();
        getTransform.rotation = Quaternion.Euler(LeftRotation);
        //_animator?.SetBool("IsWork", true);
    }

    public override void MoveRight()
    {
        base.MoveRight();
        getTransform.rotation = Quaternion.Euler(RightRotation);
        //_animator?.SetBool("IsWork", true);
    }

    public override void MoveStop()
    {
        base.MoveStop();
        //_animator?.SetBool("IsWork", false);
    }

    public void MoveUp()
    {
        //if(_interactionFunction != null && _interactionFunction.Invoke(Interaction.MoveUp) == true)
        //{
        //    //�����ϸ� �ִϸ��̼� �ٲٱ�
        //}
    }

    public void MoveDown()
    {

    }

    public void Hit(Strike strike)
    {

    }

    public Collider2D GetCollider2D()
    {
        return getCollider2D;
    }

    //���⼭���� ���Ƿ� ����� ���� ��

    
   

   
}
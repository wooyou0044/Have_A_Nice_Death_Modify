using System;
using UnityEngine;

/// <summary>
/// 유저가 조종하게 되는 플레이어 클래스
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

    //피격 액션 델리게이트
    private Action<IHittable, int> _hitAction = null;
    //스킬 사용 액션 델리게이트
    
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
        //    //성공하면 애니메이션 바꾸기
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

    //여기서부턴 임의로 만들어 보는 곳

    
   

   
}
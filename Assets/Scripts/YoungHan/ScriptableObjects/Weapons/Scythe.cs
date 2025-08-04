using System;
using UnityEngine;

[CreateAssetMenu(menuName = nameof(Weapon) + "/Scythe")]
public class Scythe : Weapon
{
    [SerializeField, Header("±âº» ÄÞº¸1")]
    private Skill.Action moveAction1;
    [SerializeField, Header("±âº» ÄÞº¸2")]
    private Skill.Action moveAction2;
    [SerializeField, Header("±âº» ÄÞº¸3")]
    private Skill.Action moveAction3;
    [SerializeField, Header("±âº» ÄÞº¸4")]
    private Skill.Action moveAction4;

    [SerializeField, Header("°øÁß ÄÞº¸1")]
    private Skill.Action standAction1;
    [SerializeField, Header("°øÁß ÄÞº¸2")]
    private Skill.Action standAction2;
    [SerializeField, Header("°øÁß ÄÞº¸3")]
    private Skill.Action standAction3;

    [SerializeField, Header("»ó´Ü °ø°Ý")]
    private Skill.Action upAction;

    public override bool TryUse(Transform transform, Attack attack, Action<GameObject, Vector2, Transform> action1, Action<Strike, Strike.Area, GameObject> action2, Func<Projectile, Projectile> func, Animator animator)
    {
        switch (attack)
        {
            case Attack.Move:
                if (moveAction1.TryUse(transform, null, action1, action2, func, animator) == true)
                {
                    return true;
                }
                if (moveAction2.TryUse(transform, null, action1, action2, func, animator) == true)
                {
                    return true;
                }
                if (moveAction3.TryUse(transform, null, action1, action2, func, animator) == true)
                {
                    return true;
                }
                if (moveAction4.TryUse(transform, null, action1, action2, func, animator) == true)
                {
                    return true;
                }
                break;
            case Attack.Stand:
                if (standAction1.TryUse(transform, null, action1, action2, func, animator) == true)
                {
                    return true;
                }
                if (standAction2.TryUse(transform, null, action1, action2, func, animator) == true)
                {
                    return true;
                }
                if (standAction3.TryUse(transform, null, action1, action2, func, animator) == true)
                {
                    return true;
                }
                break;
            case Attack.Move_Up:
                if(upAction.TryUse(transform, null, action1, action2, func, animator) == true)
                {
                    return true;
                }
                break;
        }
        return false;
    }
}
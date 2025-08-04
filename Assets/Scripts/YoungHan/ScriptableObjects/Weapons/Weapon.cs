using System;
using UnityEngine;

public abstract class Weapon : ScriptableObject
{
    public enum Attack: byte
    {
        Move,           //�⺻ ����
        Move_Up,        //�� ����
        Move_Down,      //�Ʒ� ����
        Stand,          //���� ����
        Stand_Up,       //���� �� ����
        Stand_Down,     //���� �Ʒ� ����
        Charge,         //���� ����
        Charge_Rush,    //���� ���� ����
        Fury            //�г� ����
    }

    public abstract bool TryUse(Transform transform, Attack attack, Action<GameObject, Vector2, Transform> action1, Action<Strike, Strike.Area, GameObject> action2, Func<Projectile, Projectile> func, Animator animator);
}
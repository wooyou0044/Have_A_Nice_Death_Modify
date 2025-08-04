using System;
using UnityEngine;

public abstract class Weapon : ScriptableObject
{
    public enum Attack: byte
    {
        Move,           //기본 공격
        Move_Up,        //위 공격
        Move_Down,      //아래 공격
        Stand,          //공중 공격
        Stand_Up,       //공중 위 공격
        Stand_Down,     //공중 아래 공격
        Charge,         //충전 공격
        Charge_Rush,    //충전 러쉬 공격
        Fury            //분노 공격
    }

    public abstract bool TryUse(Transform transform, Attack attack, Action<GameObject, Vector2, Transform> action1, Action<Strike, Strike.Area, GameObject> action2, Func<Projectile, Projectile> func, Animator animator);
}
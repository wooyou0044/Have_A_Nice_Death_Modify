using UnityEngine;

public abstract class Parameter : ScriptableObject
{
    public abstract void Set(Animator animator, string key);
}
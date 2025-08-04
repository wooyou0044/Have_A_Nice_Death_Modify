using UnityEngine;

[CreateAssetMenu(menuName = nameof(Parameter) + "/Int")]
public class IntParameter : Parameter
{
    [SerializeField]
    private int value;

    public override void Set(Animator animator, string key)
    {
        animator?.SetInteger(key, value);
    }
}
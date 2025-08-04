using UnityEngine;

[CreateAssetMenu(menuName = nameof(Parameter) + "/Bool")]
public class BoolParameter : Parameter
{
    [SerializeField]
    private bool value;

    public override void Set(Animator animator, string key)
    {
        animator?.SetBool(key, value);
    }
}
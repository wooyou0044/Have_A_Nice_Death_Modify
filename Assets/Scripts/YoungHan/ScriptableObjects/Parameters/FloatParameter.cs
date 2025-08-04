using UnityEngine;

[CreateAssetMenu(menuName = nameof(Parameter) + "/Float")]
public class FloatParameter : Parameter
{
    [SerializeField]
    private float value;

    public override void Set(Animator animator, string key)
    {
        animator?.SetFloat(key, value);
    }
}
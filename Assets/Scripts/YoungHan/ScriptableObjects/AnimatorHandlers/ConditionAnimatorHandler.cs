using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = nameof(AnimatorHandler) + "/" + nameof(ConditionAnimatorHandler))]
public sealed class ConditionAnimatorHandler : AnimatorHandler
{
    [SerializeField]
    private SerializableDictionary<Parameter> parameters = new SerializableDictionary<Parameter>();

    public override void Play(Animator animator)
    {
        if (parameters != null)
        {
            IEnumerable<SerializableDictionary<Parameter>.Data<Parameter>> datas = parameters.GetDatas();
            foreach (SerializableDictionary<Parameter>.Data<Parameter> data in datas)
            {
                data.value.SetState(animator, data.key);
            }
        }
    }
}
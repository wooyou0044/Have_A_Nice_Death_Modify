using UnityEngine;

[CreateAssetMenu(menuName = nameof(AnimatorHandler) + "/" + nameof(ClipAnimatorHandler))]
public sealed class ClipAnimatorHandler : AnimatorHandler
{
    [SerializeField]
    private AnimationClip animationClip;

    public override void Play(Animator animator)
    {
        animator?.Play(animationClip != null ? animationClip.name : null);
    }
}
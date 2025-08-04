using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = nameof(Skill), order = 0)]
public class Skill : ScriptableObject
{
    [SerializeField, Header("타격 값")]
    private Strike strike;
    [SerializeField, Header("범위 모양")]
    private Shape shape;

    [Space(10)]
    [SerializeField, Header("사용자가 휘두르는 효과")]
    private GameObject shotObject;
    [SerializeField, Header("기술 사용으로 인한 주변 효과")]
    private GameObject splashObject;
    [SerializeField, Header("각각의 대상을 타격하는 오브젝트")]
    private GameObject hitObject;

    [Space(10), SerializeField, Header("발사체")]
    private Projectile projectile;

    /// <summary>
    /// 스킬을 사용할 때 쓰는 함수
    /// </summary>
    /// <param name="user"></param>
    /// <param name="target"></param>
    /// <param name="tags"></param>
    /// <param name="action2"></param>
    /// <param name="action1"></param>
    /// <param name="func"></param>
    public void Use(Transform user, IHittable target, string[] tags, Action<GameObject, Vector2, Transform> action1, Action<Strike, Strike.Area, GameObject> action2, Func<Projectile, Projectile> func)
    {
        if (action1 != null)
        {
            //사용자의 Transform 값을 주면 근거리에서 휘두르는 공격
            if (user != null)
            {
                Vector2 position = user.position;
                action1.Invoke(shotObject, position, user);
                action1.Invoke(splashObject, position, null);
            }
            //그렇지 않다면 원거리 대상을 향한 갑작스런 광역 공격(혹은 타점에 대한 경고 이펙트를 줄 수 있다)
            else if(target != null)
            {
                action1.Invoke(splashObject, target.GetCollider2D().bounds.center, null);
            }
        }
        //공격 모양이 있으면
        if (shape != null)
        {
            if(user != null)
            {
                action2?.Invoke(strike, shape.GetPolygonArea(user, tags), hitObject);
            }
            else if(target != null)
            {
                action2?.Invoke(strike, shape.GetPolygonArea(target.transform, tags), hitObject);
            }
            else
            {
                //action2?.Invoke(strike, new Strike.TagArea(tags), hitObject);
            }
        }
        //공격 모양이 없으면
        else
        {
            //int length = tags != null ? tags.Length : 0;
            //if(length > 0)
            //{
            //    action2?.Invoke(strike, new Strike.TagArea(tags), hitObject);
            //}
            /*else */if(target != null)
            {
                action2?.Invoke(strike, new Strike.TargetArea(new IHittable[] { target }), hitObject);
            }
            else
            {
                //action2?.Invoke(strike, null, hitObject);
            }
        }
        //발사체를 반환하는 함수가 있다면
        if (func != null)
        {
            Projectile projectile = func.Invoke(this.projectile);
            projectile?.Shot(user, target, action1, action2);
        }
    }


    [Serializable]
    public struct Action : ISerializationCallbackReceiver
    {
        [SerializeField, Header("사용할 스킬")]
        private Skill skill;
        [SerializeField, Header("스킬 사용의 쿨타임"), Range(0, byte.MaxValue)]
        private float coolTime;
        [SerializeField, Header("대상의 태그값")]
        private string[] tags;
        [SerializeField, Header("스킬 사용과 동시에 동작하는 애니메이터")]
        private AnimatorHandler animatorHandler;
        [SerializeField, Header("발동 필수 애니메이션 클립")]
        private List<AnimationClip> essentialClips;

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            int length = tags != null ? tags.Length : 0;
            if(length > 0)
            {
                List<string> list = new List<string>();
                for (int i = 0; i < length; i++)
                {
                    if (string.IsNullOrEmpty(tags[i]) == false && list.Contains(tags[i]) == false)
                    {
                        list.Add(tags[i]);
                    }
                    else if (i == length - 1)
                    {
                        if (list.Contains(tags[i]) == true)
                        {
                            list.Add(null);
                        }
                        else
                        {
                            list.Add(tags[i]);
                        }
                    }
                }
                tags = list.ToArray();
            }
            int count = essentialClips.Count;
            if (count > 0)
            {
                List<AnimationClip> list = new List<AnimationClip>();
                for (int i = 0; i < count; i++)
                {
                    if (essentialClips[i] != null && list.Contains(essentialClips[i]) == false)
                    {
                        list.Add(essentialClips[i]);
                    }
                    else if (i == count - 1)
                    {
                        if (list.Contains(essentialClips[i]) == true)
                        {
                            list.Add(null);
                        }
                        else
                        {
                            list.Add(essentialClips[i]);
                        }
                    }
                }
                essentialClips = list;
            }
        }

        public bool TryUse(Transform user, IHittable target, Action<GameObject, Vector2, Transform> action1, Action<Strike, Strike.Area, GameObject> action2, Func<Projectile, Projectile> func, Animator animator)
        {
            int count = essentialClips != null ? essentialClips.Count : 0;
            if (animator != null)
            {
                if (count > 0)
                {
                    AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                    float playTime = stateInfo.length * stateInfo.normalizedTime;
                    for (int i = 0; i < count; i++)
                    {
                        if (essentialClips[i] != null && stateInfo.IsName(essentialClips[i].name) == true && coolTime <= playTime)
                        {
                            animatorHandler?.Play(animator);
                            skill?.Use(user, target, tags, action1, action2, func);
                            return true;
                        }
                    }
                }
                else
                {
                    animatorHandler?.Play(animator);
                    skill?.Use(user, target, tags, action1, action2, func);
                    return true;
                }
            }
            else if (count == 0 && coolTime == 0)
            {
                skill?.Use(user, target, tags, action1, action2, func);
                return true;
            }
            return false;
        }
    }
}
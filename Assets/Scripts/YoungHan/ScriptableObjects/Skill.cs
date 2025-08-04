using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = nameof(Skill), order = 0)]
public class Skill : ScriptableObject
{
    [SerializeField, Header("Ÿ�� ��")]
    private Strike strike;
    [SerializeField, Header("���� ���")]
    private Shape shape;

    [Space(10)]
    [SerializeField, Header("����ڰ� �ֵθ��� ȿ��")]
    private GameObject shotObject;
    [SerializeField, Header("��� ������� ���� �ֺ� ȿ��")]
    private GameObject splashObject;
    [SerializeField, Header("������ ����� Ÿ���ϴ� ������Ʈ")]
    private GameObject hitObject;

    [Space(10), SerializeField, Header("�߻�ü")]
    private Projectile projectile;

    /// <summary>
    /// ��ų�� ����� �� ���� �Լ�
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
            //������� Transform ���� �ָ� �ٰŸ����� �ֵθ��� ����
            if (user != null)
            {
                Vector2 position = user.position;
                action1.Invoke(shotObject, position, user);
                action1.Invoke(splashObject, position, null);
            }
            //�׷��� �ʴٸ� ���Ÿ� ����� ���� ���۽��� ���� ����(Ȥ�� Ÿ���� ���� ��� ����Ʈ�� �� �� �ִ�)
            else if(target != null)
            {
                action1.Invoke(splashObject, target.GetCollider2D().bounds.center, null);
            }
        }
        //���� ����� ������
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
        //���� ����� ������
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
        //�߻�ü�� ��ȯ�ϴ� �Լ��� �ִٸ�
        if (func != null)
        {
            Projectile projectile = func.Invoke(this.projectile);
            projectile?.Shot(user, target, action1, action2);
        }
    }


    [Serializable]
    public struct Action : ISerializationCallbackReceiver
    {
        [SerializeField, Header("����� ��ų")]
        private Skill skill;
        [SerializeField, Header("��ų ����� ��Ÿ��"), Range(0, byte.MaxValue)]
        private float coolTime;
        [SerializeField, Header("����� �±װ�")]
        private string[] tags;
        [SerializeField, Header("��ų ���� ���ÿ� �����ϴ� �ִϸ�����")]
        private AnimatorHandler animatorHandler;
        [SerializeField, Header("�ߵ� �ʼ� �ִϸ��̼� Ŭ��")]
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
using System;
using UnityEngine;

/// <summary>
/// ����� Ÿ���ϴ� ����ü
/// </summary>
[Serializable]
public struct Strike
{
    //Ÿ���ϴ� ����
    [SerializeField, Header("����� ȸ��, ������ ����")]
    public int power;

    //���� Ÿ�� �뵵�� �����̶�� ���ݷ��� Ȯ���ϴ� �뵵
    [SerializeField, Header("�뵵�� ���� �� ���ط��� ")]
    public byte extension;

    //���������� ��󿡰� ������ ������� ��ȯ�Ѵ�.
    public int result
    {
        get
        {
            //power�� �����̸� ������ ���� �뵵
            if (power < 0)
            {
                return power - UnityEngine.Random.Range(0, extension);
            }
            //power�� ����̸� ȸ���� ��Ű�� ���� �뵵
            else
            {
                return power;
            }
        }
    }

    public Strike(int power, byte extension)
    {
        this.power = power;
        this.extension = extension;
    }

    /// <summary>
    /// �ش� ����� Ÿ�� ������� �����ϴ� Ŭ����(�� �� ���� null�̶�� ����� �������� �ʰ� ���� Ÿ��)
    /// </summary>
    public abstract class Area
    {
#if UNITY_EDITOR
        private static readonly float DotSize = 0.01f;
        protected static readonly float DrawDuration = 3;

        protected static void DrawDot(Vector2 point, Color color, float duration = 0)
        {
            float half = DotSize * 0.5f;
            Vector2 dot1 = new Vector2(point.x + half, point.y + half);
            Vector2 dot2 = new Vector2(point.x - half, point.y + half);
            Vector2 dot3 = new Vector2(point.x - half, point.y - half);
            Vector2 dot4 = new Vector2(point.x + half, point.y - half);
            Debug.DrawLine(dot1, dot2, color, duration);
            Debug.DrawLine(dot2, dot3, color, duration);
            Debug.DrawLine(dot3, dot4, color, duration);
            Debug.DrawLine(dot4, dot1, color, duration);
        }
#endif
        public abstract void Show(Color color);

        public abstract bool CanStrike(IHittable hittable);
    }

    /// <summary>
    /// Ư�� �±׸� Ÿ�� ������� �����ϴ� Ŭ����
    /// </summary>
    public class TagArea : Area
    {
        private string[] tags;

        public TagArea(string[] tags)
        {
            this.tags = tags;
        }

        public override void Show(Color color)
        {
#if UNITY_EDITOR
            string hexColor = ColorUtility.ToHtmlStringRGB(color);
            int length = tags != null ? tags.Length : 0;
            if (length > 0)
            {
                for (int i = 0; i < length; i++)
                {
                    Debug.Log($"<b><color=#{hexColor}>{"�±� �̸�:" + tags[i]}</color></b>");
                }
            }
            else
            {
                Debug.Log($"<b><color=#{hexColor}>{"Ÿ�� ��� ����"}</color></b>");
            }
#endif
        }

        public override bool CanStrike(IHittable hittable)
        {
            if (hittable != null)
            {
                int length = tags != null ? tags.Length : 0;
                for (int i = 0; i < length; i++)
                {
                    if(hittable.tag == tags[i])
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }

    /// <summary>
    /// ������ ���鸸 Ÿ�� ������� �����ϴ� Ŭ����
    /// </summary>
    public class TargetArea : Area
    {
        private IHittable[] hittables;

        public TargetArea(IHittable[] hittables)
        {
            this.hittables = hittables;
        }

        public override void Show(Color color)
        {
#if UNITY_EDITOR
            string hexColor = ColorUtility.ToHtmlStringRGB(color);
            int length = hittables != null? hittables.Length : 0;
            if (length > 0)
            {
                for (int i = 0; i < length; i++)
                {
                    Debug.Log($"<b><color=#{hexColor}>{"��� �̸�:" + hittables[i]}</color></b>");
                }
            }
            else
            {
                Debug.Log($"<b><color=#{hexColor}>{"Ÿ�� ��� ����"}</color></b>");
            }
#endif
        }

        public override bool CanStrike(IHittable hittable)
        {
            if (hittable != null)
            {
                int length = hittables.Length;
                for (int i = 0; i < length; i++)
                {
                    if (hittable == hittables[i])
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }

    /// <summary>
    /// Ư�� ��ġ�� Ư�� �±� ���� ������ Ÿ�� ������� �����ϴ� Ŭ����
    /// </summary>
    public class PolygonArea : TagArea
    {
        private Vector2[] vertices;

        public PolygonArea(Vector2[] vertices, string[] tags) : base(tags)
        {
            this.vertices = vertices;
        }

        public void Show(Color color, float duration)
        {
#if UNITY_EDITOR
            int length = vertices != null ? vertices.Length : 0;
            if (length > 1)
            {
                for (int i = 0; i < length - 1; i++)
                {
                    Debug.DrawLine(vertices[i], vertices[i + 1], color, duration);
                }
            }
            else if (length > 0)
            {
                DrawDot(vertices[0], color, duration);
            }
#endif
        }

        public override void Show(Color color)
        {
#if UNITY_EDITOR
            int length = vertices != null ? vertices.Length : 0;
            if(length > 1)
            {
                for (int i = 0; i < length - 1; i++)
                {
                    Debug.DrawLine(vertices[i], vertices[i+ 1], color);
                }
            }
            else if(length > 0)
            {
                DrawDot(vertices[0], color);
            }
#endif
        }

        public override bool CanStrike(IHittable hittable)
        {
            return base.CanStrike(hittable) == true && Shape.IsCollision(vertices, hittable.GetCollider2D()) == true;
        }
    }
}
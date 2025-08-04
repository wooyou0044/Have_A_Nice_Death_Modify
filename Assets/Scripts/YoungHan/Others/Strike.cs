using System;
using UnityEngine;

/// <summary>
/// 대상을 타격하는 구조체
/// </summary>
[Serializable]
public struct Strike
{
    //타격하는 강도
    [SerializeField, Header("양수면 회복, 음수면 공격")]
    public int power;

    //만약 타격 용도가 공격이라면 공격량을 확장하는 용도
    [SerializeField, Header("용도가 공격 시 피해량을 ")]
    public byte extension;

    //최종적으로 대상에게 가해질 결과값을 반환한다.
    public int result
    {
        get
        {
            //power가 음수이면 공격을 위한 용도
            if (power < 0)
            {
                return power - UnityEngine.Random.Range(0, extension);
            }
            //power가 양수이면 회복을 시키기 위한 용도
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
    /// 해당 대상이 타격 대상인지 구별하는 클래스(단 이 값이 null이라면 대상을 구별하지 않고 광역 타격)
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
    /// 특정 태그만 타격 대상으로 간주하는 클래스
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
                    Debug.Log($"<b><color=#{hexColor}>{"태그 이름:" + tags[i]}</color></b>");
                }
            }
            else
            {
                Debug.Log($"<b><color=#{hexColor}>{"타격 대상 없음"}</color></b>");
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
    /// 지정한 대상들만 타격 대상으로 간주하는 클래스
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
                    Debug.Log($"<b><color=#{hexColor}>{"대상 이름:" + hittables[i]}</color></b>");
                }
            }
            else
            {
                Debug.Log($"<b><color=#{hexColor}>{"타격 대상 없음"}</color></b>");
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
    /// 특정 위치의 특정 태그 값을 가지면 타격 대상으로 간주하는 클래스
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
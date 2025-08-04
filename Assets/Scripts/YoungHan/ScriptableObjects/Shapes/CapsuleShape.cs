using UnityEngine;

[CreateAssetMenu(menuName = nameof(Shape) + "/Capsule")]
public sealed class CapsuleShape : Shape
{
    [SerializeField, Header("수직 여부")]
    private bool vertical;

    [SerializeField, Header("크기")]
    private Vector2 size;

    [SerializeField, Header("오프셋")]
    private Vector2 offset;

    /// <summary>
    /// 캡슐의 영역을 반환하는 함수
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="tags"></param>
    /// <returns></returns>
    public override Strike.PolygonArea GetPolygonArea(Transform transform, string[] tags)
    {
        if (transform != null)
        {
            int SegmentCount = 32;
            Vector2[] vertices = new Vector2[((SegmentCount + 1) * 2) + 1];
            if (vertical == true)
            {
                float height = Mathf.Clamp(size.y - size.x, 0, size.y);
                vertices[0] = transform.TransformPoint(new Vector2(size.x * 0.5f * Mathf.Cos(Mathf.PI), height * 0.5f + (size.x * 0.5f) * Mathf.Sin(Mathf.PI)) + offset);
                // 아래쪽 반원
                for (int i = 0; i <= SegmentCount; i++)
                {
                    float angle = Mathf.PI + Mathf.PI * i / SegmentCount;
                    vertices[i + 1] = transform.TransformPoint(new Vector2(size.x * 0.5f * Mathf.Cos(angle), -height * 0.5f + (size.x * 0.5f) * Mathf.Sin(angle)) + offset);
                }
                // 위쪽 반원
                for (int i = 0; i <= SegmentCount; i++)
                {
                    float angle = Mathf.PI * i / SegmentCount;
                    vertices[i + SegmentCount + 2] = transform.TransformPoint(new Vector2(size.x * 0.5f * Mathf.Cos(angle), height * 0.5f + (size.x * 0.5f) * Mathf.Sin(angle)) + offset);
                }
            }
            else
            {
                float width = Mathf.Clamp(size.x - size.y, 0, size.x);
                vertices[0] = transform.TransformPoint(new Vector2(width * 0.5f + (size.y * 0.5f) * Mathf.Cos(-Mathf.PI * 0.5f + Mathf.PI), size.y * 0.5f * Mathf.Sin(-Mathf.PI * 0.5f + Mathf.PI)) + offset);
                // 왼쪽 반원
                for (int i = 0; i <= SegmentCount; i++)
                {
                    float angle = Mathf.PI * 0.5f + Mathf.PI * i / SegmentCount;
                    vertices[i + 1] = transform.TransformPoint(new Vector2(-width * 0.5f + (size.y * 0.5f) * Mathf.Cos(angle), size.y * 0.5f * Mathf.Sin(angle)) + offset);
                }
                // 오른쪽 반원
                for (int i = 0; i <= SegmentCount; i++)
                {
                    float angle = -Mathf.PI * 0.5f + Mathf.PI * i / SegmentCount;
                    vertices[i + SegmentCount + 2] = transform.TransformPoint(new Vector2(width * 0.5f + (size.y * 0.5f) * Mathf.Cos(angle), size.y * 0.5f * Mathf.Sin(angle)) + offset);
                }
            }
            return new Strike.PolygonArea(vertices, tags);
        }
        return null;
    }

    /// <summary>
    /// 캡슐의 정점들을 반환하는 함수
    /// </summary>
    /// <param name="capsuleCollider2D"></param>
    /// <returns></returns>
    public static Vector2[] GetVertices(CapsuleCollider2D capsuleCollider2D)
    {
        Vector2[] vertices = null;
        if (capsuleCollider2D != null)
        {
            CapsuleDirection2D capsuleDirection2D = capsuleCollider2D.direction;
            Vector2 size = capsuleCollider2D.size;
            Vector2 offset = capsuleCollider2D.offset;
            int SegmentCount = 32;
            switch(capsuleDirection2D)
            {
                case CapsuleDirection2D.Vertical:
                    vertices = new Vector2[((SegmentCount + 1) * 2) + 1];
                    float height = Mathf.Clamp(size.y - size.x, 0, size.y);
                    vertices[0] = capsuleCollider2D.transform.TransformPoint(new Vector2(size.x * 0.5f * Mathf.Cos(Mathf.PI), height * 0.5f + (size.x * 0.5f) * Mathf.Sin(Mathf.PI)) + offset);
                    // 아래쪽 반원
                    for (int i = 0; i <= SegmentCount; i++)
                    {
                        float angle = Mathf.PI + Mathf.PI * i / SegmentCount;
                        vertices[i + 1] = capsuleCollider2D.transform.TransformPoint(new Vector2(size.x * 0.5f * Mathf.Cos(angle), -height * 0.5f + (size.x * 0.5f) * Mathf.Sin(angle)) + offset);
                    }
                    // 위쪽 반원
                    for (int i = 0; i <= SegmentCount; i++)
                    {
                        float angle = Mathf.PI * i / SegmentCount;
                        vertices[i + SegmentCount + 2] = capsuleCollider2D.transform.TransformPoint(new Vector2(size.x * 0.5f * Mathf.Cos(angle), height * 0.5f + (size.x * 0.5f) * Mathf.Sin(angle)) + offset);
                    }
                    break;
                case CapsuleDirection2D.Horizontal:
                    vertices = new Vector2[((SegmentCount + 1) * 2) + 1];
                    float width = Mathf.Clamp(size.x - size.y, 0, size.x);
                    vertices[0] = capsuleCollider2D.transform.TransformPoint(new Vector2(width * 0.5f + (size.y * 0.5f) * Mathf.Cos(-Mathf.PI * 0.5f + Mathf.PI), size.y * 0.5f * Mathf.Sin(-Mathf.PI * 0.5f + Mathf.PI)) + offset);
                    // 왼쪽 반원
                    for (int i = 0; i <= SegmentCount; i++)
                    {
                        float angle = Mathf.PI * 0.5f + Mathf.PI * i / SegmentCount;
                        vertices[i + 1] = capsuleCollider2D.transform.TransformPoint(new Vector2(-width * 0.5f + (size.y * 0.5f) * Mathf.Cos(angle), size.y * 0.5f * Mathf.Sin(angle)) + offset);
                    }
                    // 오른쪽 반원
                    for (int i = 0; i <= SegmentCount; i++)
                    {
                        float angle = -Mathf.PI * 0.5f + Mathf.PI * i / SegmentCount;
                        vertices[i + SegmentCount + 2] = capsuleCollider2D.transform.TransformPoint(new Vector2(width * 0.5f + (size.y * 0.5f) * Mathf.Cos(angle), size.y * 0.5f * Mathf.Sin(angle)) + offset);
                    }
                    break;
            }
        }
        return vertices;
    }

    /// <summary>
    /// 캡슐의 정점들을 반환하는 함수
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="capsuleDirection2D"></param>
    /// <param name="size"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    public static Vector2[] GetVertices(Transform transform, CapsuleDirection2D capsuleDirection2D, Vector2 size, Vector2 offset)
    {
        Vector2[] vertices = null;
        if (transform != null)
        {
            int SegmentCount = 32;
            switch (capsuleDirection2D)
            {
                case CapsuleDirection2D.Vertical:
                    vertices = new Vector2[((SegmentCount + 1) * 2) + 1];
                    float height = Mathf.Clamp(size.y - size.x, 0, size.y);
                    vertices[0] = transform.TransformPoint(new Vector2(size.x * 0.5f * Mathf.Cos(Mathf.PI), height * 0.5f + (size.x * 0.5f) * Mathf.Sin(Mathf.PI)) + offset);
                    // 아래쪽 반원
                    for (int i = 0; i <= SegmentCount; i++)
                    {
                        float angle = Mathf.PI + Mathf.PI * i / SegmentCount;
                        vertices[i + 1] = transform.TransformPoint(new Vector2(size.x * 0.5f * Mathf.Cos(angle), -height * 0.5f + (size.x * 0.5f) * Mathf.Sin(angle)) + offset);
                    }
                    // 위쪽 반원
                    for (int i = 0; i <= SegmentCount; i++)
                    {
                        float angle = Mathf.PI * i / SegmentCount;
                        vertices[i + SegmentCount + 2] = transform.TransformPoint(new Vector2(size.x * 0.5f * Mathf.Cos(angle), height * 0.5f + (size.x * 0.5f) * Mathf.Sin(angle)) + offset);
                    }
                    break;
                case CapsuleDirection2D.Horizontal:
                    vertices = new Vector2[((SegmentCount + 1) * 2) + 1];
                    float width = Mathf.Clamp(size.x - size.y, 0, size.x);
                    vertices[0] = transform.TransformPoint(new Vector2(width * 0.5f + (size.y * 0.5f) * Mathf.Cos(-Mathf.PI * 0.5f + Mathf.PI), size.y * 0.5f * Mathf.Sin(-Mathf.PI * 0.5f + Mathf.PI)) + offset);
                    // 왼쪽 반원
                    for (int i = 0; i <= SegmentCount; i++)
                    {
                        float angle = Mathf.PI * 0.5f + Mathf.PI * i / SegmentCount;
                        vertices[i + 1] = transform.TransformPoint(new Vector2(-width * 0.5f + (size.y * 0.5f) * Mathf.Cos(angle), size.y * 0.5f * Mathf.Sin(angle)) + offset);
                    }
                    // 오른쪽 반원
                    for (int i = 0; i <= SegmentCount; i++)
                    {
                        float angle = -Mathf.PI * 0.5f + Mathf.PI * i / SegmentCount;
                        vertices[i + SegmentCount + 2] = transform.TransformPoint(new Vector2(width * 0.5f + (size.y * 0.5f) * Mathf.Cos(angle), size.y * 0.5f * Mathf.Sin(angle)) + offset);
                    }
                    break;
            }
        }
        return vertices;
    }
}
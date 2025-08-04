using UnityEngine;

[CreateAssetMenu(menuName = nameof(Shape) + "/Circle")]
public sealed  class CircleShape : Shape
{
    [SerializeField, Header("반지름")]
    private float radius;

    [SerializeField, Header("오프셋")]
    private Vector2 offset;

    /// <summary>
    /// 원의 영역을 반환하는 함수
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="tags"></param>
    /// <returns></returns>
    public override Strike.PolygonArea GetPolygonArea(Transform transform, string[] tags)
    {
        if (transform != null)
        {
            int SegmentCount = 32;
            Vector2[] vertices = new Vector2[SegmentCount * 2];
            for (int i = 0; i < SegmentCount; i++)
            {
                float angle1 = i * Mathf.PI * 2 / SegmentCount;
                float angle2 = (i + 1) * Mathf.PI * 2 / SegmentCount;
                Vector2 point1 = new Vector2(Mathf.Cos(angle1) * radius, Mathf.Sin(angle1) * radius) + offset;
                Vector2 point2 = new Vector2(Mathf.Cos(angle2) * radius, Mathf.Sin(angle2) * radius) + offset;
                vertices[i * 2] = transform.TransformPoint(point1);
                vertices[i * 2 + 1] = transform.TransformPoint(point2);
            }
            return new Strike.PolygonArea(vertices, tags);
        }
        return null;
    }

    /// <summary>
    /// 원의 정점들을 반환하는 함수
    /// </summary>
    /// <param name="circleCollider2D"></param>
    /// <returns></returns>
    public static Vector2[] GetVertices(CircleCollider2D circleCollider2D)
    {
        Vector2[] vertices = null;
        if(circleCollider2D != null)
        {
            float radius = circleCollider2D.radius;
            Vector2 offset = circleCollider2D.offset;
            int SegmentCount = 32;
            vertices = new Vector2[SegmentCount * 2];
            for (int i = 0; i < SegmentCount; i++)
            {
                float angle1 = i * Mathf.PI * 2 / SegmentCount;
                float angle2 = (i + 1) * Mathf.PI * 2 / SegmentCount;
                Vector2 point1 = new Vector2(Mathf.Cos(angle1) * radius, Mathf.Sin(angle1) * radius) + offset;
                Vector2 point2 = new Vector2(Mathf.Cos(angle2) * radius, Mathf.Sin(angle2) * radius) + offset;
                vertices[i * 2] = circleCollider2D.transform.TransformPoint(point1);
                vertices[i * 2 + 1] = circleCollider2D.transform.TransformPoint(point2);
            }
        }
        return vertices;
    }

    /// <summary>
    /// 원의 정점들을 반환하는 함수
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="radius"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    public static Vector2[] GetVertices(Transform transform, float radius, Vector2 offset)
    {
        Vector2[] vertices = null;
        if (transform != null)
        {
            int SegmentCount = 32;
            vertices = new Vector2[SegmentCount * 2];
            for (int i = 0; i < SegmentCount; i++)
            {
                float angle1 = i * Mathf.PI * 2 / SegmentCount;
                float angle2 = (i + 1) * Mathf.PI * 2 / SegmentCount;
                Vector2 point1 = new Vector2(Mathf.Cos(angle1) * radius, Mathf.Sin(angle1) * radius) + offset;
                Vector2 point2 = new Vector2(Mathf.Cos(angle2) * radius, Mathf.Sin(angle2) * radius) + offset;
                vertices[i * 2] = transform.TransformPoint(point1);
                vertices[i * 2 + 1] = transform.TransformPoint(point2);
            }
        }
        return vertices;
    }
}
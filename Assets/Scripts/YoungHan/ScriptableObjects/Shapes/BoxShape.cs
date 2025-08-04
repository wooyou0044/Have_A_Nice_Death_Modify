using UnityEngine;

[CreateAssetMenu(menuName = nameof(Shape) + "/Box")]
public sealed class BoxShape : Shape
{
    [SerializeField, Header("크기")]
    private Vector2 size;

    [SerializeField, Header("오프셋")]
    private Vector2 offset;

    /// <summary>
    /// 사각형의 영역을 반환하는 함수
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="tags"></param>
    /// <returns></returns>
    public override Strike.PolygonArea GetPolygonArea(Transform transform, string[] tags)
    {
        if (transform != null)
        {
            int dotCount = 4;
            float half = 0.5f;
            Vector2[] localEdges = new Vector2[]
            {
                offset + new Vector2(-size.x * half, -size.y * half),
                offset + new Vector2(size.x * half, -size.y * half),
                offset + new Vector2(size.x * half, size.y * half),
                offset + new Vector2(-size.x * half, size.y * half),
            };
            Vector2[] vertices = new Vector2[dotCount * 2];
            for (int i = 0; i < dotCount; i++)
            {
                vertices[i * 2] = transform.TransformPoint(localEdges[i]);
                vertices[i * 2 + 1] = transform.TransformPoint(localEdges[(i + 1) % dotCount]);
            }
            return new Strike.PolygonArea(vertices, tags);
        }
        return null;
    }

    /// <summary>
    /// 사각형의 정점들을 반환하는 함수
    /// </summary>
    /// <param name="boxCollider2D"></param>
    /// <returns></returns>
    public static Vector2[] GetVertices(BoxCollider2D boxCollider2D)
    {
        Vector2[] vertices = null;
        if (boxCollider2D != null)
        {
            Vector2 size = boxCollider2D.size;
            Vector2 offset = boxCollider2D.offset;
            int dotCount = 4;
            float half = 0.5f;
            Vector2[] localEdges = new Vector2[]
            {
                offset + new Vector2(-size.x * half, -size.y * half),
                offset + new Vector2(size.x * half, -size.y * half),
                offset + new Vector2(size.x * half, size.y * half),
                offset + new Vector2(-size.x * half, size.y * half),
            };
            vertices = new Vector2[dotCount * 2];
            for (int i = 0; i < dotCount; i++)
            {
                vertices[i * 2] = boxCollider2D.transform.TransformPoint(localEdges[i]);
                vertices[i * 2 + 1] = boxCollider2D.transform.TransformPoint(localEdges[(i + 1) % dotCount]);
            }
        }
        return vertices;
    }

    /// <summary>
    /// 사각형의 정점들을 반환하는 함수
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="size"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    public static Vector2[] GetVertices(Transform transform, Vector2 size, Vector2 offset)
    {
        Vector2[] vertices = null;
        if (transform != null)
        {
            int dotCount = 4;
            float half = 0.5f;
            Vector2[] localEdges = new Vector2[]
            {
                offset + new Vector2(-size.x * half, -size.y * half),
                offset + new Vector2(size.x * half, -size.y * half),
                offset + new Vector2(size.x * half, size.y * half),
                offset + new Vector2(-size.x * half, size.y * half),
            };
            vertices = new Vector2[dotCount * 2];
            for (int i = 0; i < dotCount; i++)
            {
                vertices[i * 2] = transform.TransformPoint(localEdges[i]);
                vertices[i * 2 + 1] = transform.TransformPoint(localEdges[(i + 1) % dotCount]);
            }
        }
        return vertices;
    }
}
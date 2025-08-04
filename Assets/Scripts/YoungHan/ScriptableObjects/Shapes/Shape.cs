using UnityEngine;

public abstract class Shape : ScriptableObject
{
    public abstract Strike.PolygonArea GetPolygonArea(Transform transform, string[] tags);

    /// <summary>
    /// 두 선분이 교차하는지 확인하는 함수
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <param name="q1"></param>
    /// <param name="q2"></param>
    /// <returns></returns>
    private static bool IsIntersection(Vector2 p1, Vector2 p2, Vector2 q1, Vector2 q2)
    {
        // 벡터 계산
        Vector2 d1 = p2 - p1; // 선분 1의 방향 벡터
        Vector2 d2 = q2 - q1; // 선분 2의 방향 벡터

        // 교차 여부 계산
        float denominator = d1.x * d2.y - d1.y * d2.x; // 두 벡터의 외적 (z 성분)
        if (Mathf.Approximately(denominator, 0))
        {
            return false; // 평행 또는 공선 상태에서는 교차하지 않음
        }

        // 교차 지점의 파라미터 계산
        float t = ((q1.x - p1.x) * d2.y - (q1.y - p1.y) * d2.x) / denominator;
        float u = ((q1.x - p1.x) * d1.y - (q1.y - p1.y) * d1.x) / denominator;

        // t와 u가 [0, 1] 범위에 있을 경우 교차
        return t >= 0 && t <= 1 && u >= 0 && u <= 1;
    }

    /// <summary>
    /// 두 정점을 이은 선분들이 서로 교차하는지 확인하는 함수
    /// </summary>
    /// <param name="area"></param>
    /// <param name="collider"></param>
    /// <returns></returns>
    private static bool IsIntersection(Vector2[] area, Vector2[] collider)
    {
        if (collider != null)
        {
            // 도형 A와 B의 모든 선분을 비교
            for (int i = 0; i < area.Length; i++)
            {
                Vector2 aStart = area[i];
                Vector2 aEnd = area[(i + 1) % area.Length]; // 다음 정점 (루프를 위해 % 사용)
                for (int j = 0; j < collider.Length; j++)
                {
                    Vector2 bStart = collider[j];
                    Vector2 bEnd = collider[(j + 1) % collider.Length]; // 다음 정점 (루프를 위해 % 사용)
                    if (IsIntersection(aStart, aEnd, bStart, bEnd))
                    {
                        return true; // 교차점 발견
                    }
                }
            }
        }
        return false; // 교차점 없음
    }

    /// <summary>
    /// 정점들과 콜라이더가 충돌했는지 확인하는 함수
    /// </summary>
    /// <param name="vertices"></param>
    /// <param name="collider2D"></param>
    /// <returns></returns>
    public static bool IsCollision(Vector2[] vertices, Collider2D collider2D)
    {
        int length = vertices != null ? vertices.Length : 0;
        if (length > 0)
        {
            if (collider2D != null)
            {
                for (int i = 0; i < length; i++)
                {
                    if (collider2D.OverlapPoint(vertices[i]) == true)
                    {
                        return true;
                    }
                }
                if (IsIntersection(vertices, BoxShape.GetVertices(collider2D as BoxCollider2D)) == true)
                {
                    return true;
                }
                if (IsIntersection(vertices, CircleShape.GetVertices(collider2D as CircleCollider2D)) == true)
                {
                    return true;
                }
                if (IsIntersection(vertices, CapsuleShape.GetVertices(collider2D as CapsuleCollider2D)) == true)
                {
                    return true;
                }
            }
        }
        else
        {
            return true;
        }
        return false;
    }

    public static Strike.PolygonArea GetPolygonArea(Collider2D collider2D, string[] tags)
    {
        if(collider2D != null)
        {
            Vector2[] vertices = BoxShape.GetVertices(collider2D as BoxCollider2D);
            if (vertices != null)
            {
                return new Strike.PolygonArea(vertices, tags);
            }
            vertices = CircleShape.GetVertices(collider2D as CircleCollider2D);
            if (vertices != null)
            {
                return new Strike.PolygonArea(vertices, tags);
            }
            vertices = CapsuleShape.GetVertices(collider2D as CapsuleCollider2D);
            if (vertices != null)
            {
                return new Strike.PolygonArea(vertices, tags);
            }
        }
        return null;
    }

    public static void Draw(Vector2[] vertices, Color color, float duration)
    {
#if UNITY_EDITOR
        int length = vertices != null ? vertices.Length : 0;
        for (int i = 0; i < length - 1; i++)
        {
            Debug.DrawLine(vertices[i], vertices[i + 1], color, duration);
        }
#endif
    }
}
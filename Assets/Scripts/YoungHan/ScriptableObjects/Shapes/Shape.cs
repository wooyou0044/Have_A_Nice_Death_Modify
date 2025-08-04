using UnityEngine;

public abstract class Shape : ScriptableObject
{
    public abstract Strike.PolygonArea GetPolygonArea(Transform transform, string[] tags);

    /// <summary>
    /// �� ������ �����ϴ��� Ȯ���ϴ� �Լ�
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <param name="q1"></param>
    /// <param name="q2"></param>
    /// <returns></returns>
    private static bool IsIntersection(Vector2 p1, Vector2 p2, Vector2 q1, Vector2 q2)
    {
        // ���� ���
        Vector2 d1 = p2 - p1; // ���� 1�� ���� ����
        Vector2 d2 = q2 - q1; // ���� 2�� ���� ����

        // ���� ���� ���
        float denominator = d1.x * d2.y - d1.y * d2.x; // �� ������ ���� (z ����)
        if (Mathf.Approximately(denominator, 0))
        {
            return false; // ���� �Ǵ� ���� ���¿����� �������� ����
        }

        // ���� ������ �Ķ���� ���
        float t = ((q1.x - p1.x) * d2.y - (q1.y - p1.y) * d2.x) / denominator;
        float u = ((q1.x - p1.x) * d1.y - (q1.y - p1.y) * d1.x) / denominator;

        // t�� u�� [0, 1] ������ ���� ��� ����
        return t >= 0 && t <= 1 && u >= 0 && u <= 1;
    }

    /// <summary>
    /// �� ������ ���� ���е��� ���� �����ϴ��� Ȯ���ϴ� �Լ�
    /// </summary>
    /// <param name="area"></param>
    /// <param name="collider"></param>
    /// <returns></returns>
    private static bool IsIntersection(Vector2[] area, Vector2[] collider)
    {
        if (collider != null)
        {
            // ���� A�� B�� ��� ������ ��
            for (int i = 0; i < area.Length; i++)
            {
                Vector2 aStart = area[i];
                Vector2 aEnd = area[(i + 1) % area.Length]; // ���� ���� (������ ���� % ���)
                for (int j = 0; j < collider.Length; j++)
                {
                    Vector2 bStart = collider[j];
                    Vector2 bEnd = collider[(j + 1) % collider.Length]; // ���� ���� (������ ���� % ���)
                    if (IsIntersection(aStart, aEnd, bStart, bEnd))
                    {
                        return true; // ������ �߰�
                    }
                }
            }
        }
        return false; // ������ ����
    }

    /// <summary>
    /// ������� �ݶ��̴��� �浹�ߴ��� Ȯ���ϴ� �Լ�
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
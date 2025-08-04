using UnityEngine;

[DisallowMultipleComponent]
public class YoungHan : MonoBehaviour
{
    [SerializeField]
    private int SegmentCount = 32;

    [SerializeField]
    private Vector2 size;

    [SerializeField]
    private Vector2 offset;

    [SerializeField]
    private CapsuleDirection2D capsuleDirection2D;

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        Vector2[] vertices = GetVertices();
        int length = vertices != null ? vertices.Length : 0;
        for (int i = 0; i < length - 1; i++)
        {
            Debug.DrawLine(vertices[i], vertices[i + 1], Color.red);
        }
#endif
    }

    public Vector2[] GetVertices()
    {
        Vector2[] vertices = null;
        if (transform != null)
        {
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

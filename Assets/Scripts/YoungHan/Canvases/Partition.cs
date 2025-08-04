using System;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[DisallowMultipleComponent]
[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(CanvasScaler))]
[RequireComponent(typeof(GraphicRaycaster))]
/// <summary>
/// ĵ���� ������ �ִ� UI �г� ������Ʈ ��ü���� ���ϴ� �������� ���� ���ִ� ����� �ִ�.
/// </summary>
public class Partition : MonoBehaviour
{
    private bool _hasRectTransform = false;

    private RectTransform _rectTransform = null;

    protected RectTransform getRectTransform {
        get
        {
            if (_hasRectTransform == false)
            {
                _rectTransform = GetComponent<RectTransform>();
                _hasRectTransform = true;
            }
            return _rectTransform;
        }
    }

    [Serializable]
    private struct Frame
    {
        [SerializeField]
        private RectTransform rectTransform;
        [SerializeField]
        private Vector2 division;
        [SerializeField]
        private Vector2 pivot;

        public void Resize(Vector2 anchorMin, Vector2 anchorMax, Vector2 sizeDelta)
        {
            if (division.x < 1)
            {
                division.x = 1;
            }
            if (division.y < 1)
            {
                division.y = 1;
            }
            if (rectTransform != null)
            {
                rectTransform.pivot = pivot;
                rectTransform.anchorMin = anchorMin;
                rectTransform.anchorMax = anchorMax;
                rectTransform.sizeDelta = new Vector2((sizeDelta.x / division.x) - sizeDelta.x, (sizeDelta.y / division.y) - sizeDelta.y);
            }
        }
    }

    [SerializeField]
    private float ratio = 0;

    [SerializeField]
    private Frame[] frames = null;

    private Vector2 sizeDelta = new Vector2();

    private static Vector2 screenSize = new Vector2(Screen.width, Screen.height);

#if UNITY_EDITOR
    private void OnValidate()
    {
        UnityEditor.EditorApplication.delayCall += () => Resize();
    }
#endif

    private void LateUpdate()
    {
        int width = Screen.width;
        int height = Screen.height;
        if (screenSize.x != width || screenSize.y != height)
        {
            screenSize.x = width;
            screenSize.y = height;
        }
        Vector2 sizeDelta = getRectTransform.sizeDelta;
        if (this.sizeDelta != sizeDelta)
        {
            this.sizeDelta = sizeDelta;
        }
        Resize();
    }

    private void Resize()
    {
        if (screenSize.x != 0 && screenSize.y != 0)
        {
            Rect safeArea = Screen.safeArea;
            Vector2 ratio = safeArea.size / screenSize;
            Vector2 sizeDelta = new Vector2(this.sizeDelta.x * ratio.x, this.sizeDelta.y * ratio.y);
            Vector2 anchorMin = new Vector2(safeArea.position.x / screenSize.x, safeArea.position.y / screenSize.y);
            Vector2 anchorMax = new Vector2((safeArea.position.x + safeArea.size.x) / screenSize.x, (safeArea.position.y + safeArea.size.y) / screenSize.y);
            //���� ����̸� �������� ������
            if (this.ratio > 0)
            {
                float value = (safeArea.size.y / safeArea.size.x) * 0.5f * (1 / (this.ratio + 1));
                anchorMin.x = Mathf.Clamp(0.5f - value, anchorMin.x, 0.5f);
                anchorMax.x = Mathf.Clamp(0.5f + value, 0.5f, anchorMax.x);
            }
            //���� �����̸� �������� ������
            else if (this.ratio < 0)
            {
                float value = (safeArea.size.x / safeArea.size.y) * 0.5f * (1 / (-this.ratio + 1));
                anchorMin.y = Mathf.Clamp(0.5f - value, anchorMin.y, 0.5f);
                anchorMax.y = Mathf.Clamp(0.5f + value, 0.5f, anchorMax.y);
            }
            int length = frames != null ? frames.Length : 0;
            for (int i = 0; i < length; i++)
            {
                frames[i].Resize(anchorMin, anchorMax, sizeDelta);
            }
        }
    }

#if UNITY_EDITOR
    [ContextMenu("ȭ�� ���� Ȯ���ϱ�")]
    private void Log()
    {
        if (ratio > 0)
        {
            Debug.Log("ȭ�� ���� 1:" + (ratio + 1));
        }
        else if (ratio < 0)
        {
            Debug.Log("ȭ�� ���� " + (-ratio + 1) + ":1");
        }
        else
        {
            Debug.Log("ȭ�� ���� ���� ����");
        }
    }
#endif
}
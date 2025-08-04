using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public class StatusPanel : MonoBehaviour
{
    [SerializeField, Header("������ �پ��� �ӵ�"), Range(0.1f, 10f)]
    private float _wainingValue = 1f;
    [SerializeField, Header("ü�¹� �̹���")]
    private Image _lifeImage;
    [SerializeField, Header("�ҿ�� �̹���")]
    private Text _soularyText;
    [SerializeField, Header("������̿� �̹���")]
    private Text _prismiumText;
    [SerializeField, Header("�ƴϸ� ������Ʈ")]
    private GameObject[] animaObjects;

    public byte soulary
    {
        set
        {
            if(_soularyText != null)
            {
                _soularyText.text = value.ToString();
            }
        }
    }

    public byte prismium
    {
        set
        {
            if (_prismiumText != null)
            {
                _prismiumText.text = value.ToString();
            }
        }
    }

    public byte anima
    {
        set
        {
            byte count = value;
            foreach (GameObject animaObject in animaObjects)
            {
                if (count > 0)
                {
                    animaObject.SetActive(true);
                    count--;
                }
                else
                {
                    animaObject?.SetActive(false);
                }
            }
        }
    }

    public void Set(Player player)
    {
        if (player != null && _lifeImage != null)
        {
            float life = player.remainLife > 0 ? player.remainLife : 0;
            float value = player.maxLife > 0 ? life / player.maxLife : 0;
            _lifeImage.DOKill();
            _lifeImage.DOFillAmount(value, _wainingValue);
        }
    }
}
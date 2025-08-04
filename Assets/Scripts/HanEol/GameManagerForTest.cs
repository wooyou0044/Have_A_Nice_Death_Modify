using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������ ������ �Ѱ��ϴ� �Ŵ���, �̱����� ��� ���� Ŭ����
/// </summary>
[RequireComponent(typeof(Controller))]
public sealed class GameManagerForTest : Manager<GameManagerForTest>
{
    private bool _hasController = false;

    private ControllerForOnlyTest _controller;

    //�÷��̾ ������ �� �ִ� ��Ʈ�ѷ�
    private ControllerForOnlyTest getController {
        get
        {
            if (_hasController == false)
            {
                _hasController = true;
                _controller = GetComponent<ControllerForOnlyTest>();
            }
            return _controller;
        }
    }

    private List<IHittable> _hittableList = new List<IHittable>();

    private List<GameObject> _effectObjectList = new List<GameObject>();

    protected override void Initialize()
    {
        _destroyOnLoad = true;
        getController.player?.Initialize(Report);
    }

    private void Hit(Strike strike, IHittable hittable, GameObject effectObject)
    {
        ShowEffect(effectObject, hittable.GetCollider2D().bounds.center);
        hittable.Hit(strike);
    }

    /// <summary>
    /// Ư�� ��ġ�� �Ͼ�� ȿ�� ������Ʈ�� �����ִ� �Լ�
    /// </summary>
    /// <param name="effectObject"></param>
    /// <param name="position"></param>
    public void ShowEffect(GameObject effectObject, Vector2 position)
    {
        if(effectObject != null)
        {

        }
    }

    /// <summary>
    /// ��󿡰� ���� Ȥ�� ȸ���� ���� �󸶳� ���Դ��� �˷��ִ� �Լ�
    /// </summary>
    /// <param name="hittable"></param>
    /// <param name="result"></param>
    public static void Report(IHittable hittable, int result)
    {
        //�������� �� ��󿡰� �󸶳� ���Դ��� �����ϰ� ui�� ���� ����
        if (hittable.isAlive == false)
        {
            //����ϸ� �߰� ����
            if (instance._controller.player == (Object)hittable)
            {

            }
        }
    }

    /// <summary>
    /// Ÿ���� ������ �� ȣ���ϴ� �Լ�
    /// </summary>
    /// <param name="strike"></param>
    /// <param name="area"></param>
    /// <param name="effectObject"></param>
    public static void Report(Strike strike, Strike.Area area, GameObject effectObject)
    {
        if(area == null)
        {
            instance.Hit(strike, instance._controller.player, effectObject);
            int count = instance._hittableList.Count;
            for (int i = 0; i < count; i++)
            {
                instance.Hit(strike, instance._hittableList[i], effectObject);
            }
        }
        else
        {
            if (area.CanStrike(instance._controller.player) == true)
            {
                instance.Hit(strike, instance._controller.player, effectObject);
            }
            int count = instance._hittableList.Count;
            for (int i = 0; i < count; i++)
            {
                if (area.CanStrike(instance._hittableList[i]) == true)
                {
                    instance.Hit(strike, instance._hittableList[i], effectObject);
                }
            }
        }
    }

    //��ų�� ����� �� ȣ���ϴ� �Լ�
    public static void Report(Skill skill)
    {

    }

    //���Ƿ� ����� ��

}
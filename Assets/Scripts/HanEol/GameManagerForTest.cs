using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게임의 진행을 총괄하는 매니저, 싱글턴을 상속 받은 클래스
/// </summary>
[RequireComponent(typeof(Controller))]
public sealed class GameManagerForTest : Manager<GameManagerForTest>
{
    private bool _hasController = false;

    private ControllerForOnlyTest _controller;

    //플레이어를 조종할 수 있는 컨트롤러
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
    /// 특정 위치에 일어나는 효과 오브젝트를 보여주는 함수
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
    /// 대상에게 피해 혹은 회복의 값이 얼마나 들어왔는지 알려주는 함수
    /// </summary>
    /// <param name="hittable"></param>
    /// <param name="result"></param>
    public static void Report(IHittable hittable, int result)
    {
        //데미지가 이 대상에게 얼마나 들어왔는지 보고하고 ui로 값을 전송
        if (hittable.isAlive == false)
        {
            //사망하면 추가 보고
            if (instance._controller.player == (Object)hittable)
            {

            }
        }
    }

    /// <summary>
    /// 타격이 들어왔을 때 호출하는 함수
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

    //스킬을 사용할 때 호출하는 함수
    public static void Report(Skill skill)
    {

    }

    //임의로 만드는 곳

}
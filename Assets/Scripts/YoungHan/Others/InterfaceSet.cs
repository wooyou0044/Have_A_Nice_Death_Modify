using UnityEngine;

/// <summary>
/// 트랜스폼을 반환하는 용도를 가진 인터페이스
/// </summary>
public interface ITransformable
{
    //왼쪽 방향 오일러 벡터
    protected static readonly Vector2 LeftRotation = new Vector2(0, 180);
    //오른쪽 방향 오일러 벡터
    protected static readonly Vector2 RightRotation = new Vector2(0, 0);

    public Vector2 position {
        get;
        set;
    }

    public Quaternion rotation {
        get;
        set;
    }
}

/// <summary>
/// 오른쪽 또는 왼쪽으로 이동하거나 바라볼 수 있는 객체들이 상속 받는 인터페이스
/// </summary>
public interface IMovable
{
    //다른 물체와 충돌 시 유효 거리
    protected static readonly float OverlappingDistance = 0.02f;
    //땅에 착지 중 땅을 이탈했다고 간주하는 최소 속도
    protected static readonly float MinimumDropVelocity = -0.49f;
    //오른쪽으로 이동
    public void MoveRight();
    //왼쪽으로 이동
    public void MoveLeft();
    //이동 멈춤
    public void MoveStop();
}

/// <summary>
/// 상호 작용 할 수 있는 객체들이 상속 받는 인터페이스
/// </summary>
public interface IPickable
{

}


public interface ISkillable
{

}

public interface IAnimationable
{
    public Animator animator {
        get;
    }
}
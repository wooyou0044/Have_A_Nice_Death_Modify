using System.Collections;
using UnityEngine;

public class BossMovement : Runner, IHittable
{
    [Header("애니메이션 클립")]
    [SerializeField] AnimationClip surprisedClip;
    [SerializeField] AnimationClip fightStartClip;
    [SerializeField] AnimationClip idleClip;
    [SerializeField] AnimationClip uTurnClip;
    [SerializeField] AnimationClip hitClip;
    [SerializeField] AnimationClip stunStartClip;
    [SerializeField] AnimationClip stunIdleClip;
    [SerializeField] AnimationClip stunEndClip;
    [SerializeField] AnimationClip dashStartClip;
    [SerializeField] AnimationClip dashLoopClip;
    [SerializeField] AnimationClip dashEndClip;
    [SerializeField] AnimationClip comboAttack1Clip;
    [SerializeField] AnimationClip comboAttack2Clip;
    [SerializeField] AnimationClip death1Clip;
    [SerializeField] AnimationClip death2Clip;
    [SerializeField] AnimationClip death3Clip;
    [SerializeField] AnimationClip death4Clip;

    [Header("정보")]
    [SerializeField] float HP = 1300;
    [SerializeField] float dropSpeed;
    [SerializeField] float flySpeed;
    [SerializeField] float maxHeight;
    [SerializeField] GameObject stunPrefab;
    [SerializeField] Transform stunPos;
    [SerializeField] GameObject darkTornado;
    [SerializeField] Transform tornadoPos;

    Collider2D bossCollider;
    Rigidbody2D myRigid;
    AnimatorPlayer myPlayer;
    GargoyleBrain bossAI;

    GameObject stun;
    GameObject tornado1;
    GameObject tornado2;
    GameObject tornado3;

    // 맵의 중간 지점을 박아놓고 오른쪽 왼쪽 판단
    Vector2 startPoint;
    Vector2 endPoint;
    Vector2 midPoint;
    Vector2 targetPos;
    Vector2 pointPos;
    Vector2 destination;

    float attackElapsedTime;
    float fullHp;
    float oppositePointX;
    float maxHp;

    bool isStun;
    bool isAttacking;
    bool isMeetPlayer;
    bool isDead;
    bool isDisappear;

    // 임시
    GameObject player;

    public enum DeathType
    {
        Default,
        Be_Rock,
        Awake,
        Be_AnotherBoss,
        GoOutside,
        Death
    }

    public DeathType deathState;

    //체력바
    [SerializeField]Boss_Hp_Bar hpBar;
    // 최대 체력
    public float currentHP
    {
        get
        {
            return HP;
        }
    }

    public float StartPointX
    {
        get { return startPoint.x; }
    }

    public float EndPointX
    {
        get { return endPoint.x; }
    }

    public bool isAlive
    {
        get
        {
            return (HP > 0) ? true : false;
        }
    }

    public bool IsMeetPlayer
    {
        get
        {
            return isMeetPlayer;
        }
        set
        {
            isMeetPlayer = value;
        }
    }

    public bool IsDead
    {
        get
        {
            return isDead;
        }
        set
        {
            isDead = value;
        }
    }

    public bool IsDisappear
    {
        get
        {
            return isDisappear;
        }
    }

    void Awake()
    {
        bossCollider = GetComponent<Collider2D>();
        myPlayer = GetComponent<AnimatorPlayer>();
        myRigid = GetComponent<Rigidbody2D>();
        startPoint = GameObject.Find("StartPoint").transform.position;
        endPoint = GameObject.Find("EndPoint").transform.position;
        bossAI = GetComponent<GargoyleBrain>();

        // 임시
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Start()
    {
        stun = Instantiate(stunPrefab, stunPos);
        stun.transform.localScale = new Vector2(0.7f, 0.7f);
        stun.SetActive(false);
        tornado1 = Instantiate(darkTornado, tornadoPos);
        tornado1.transform.localScale = new Vector2(1, 1);
        tornado2 = Instantiate(darkTornado, tornadoPos);
        tornado3 = Instantiate(darkTornado, tornadoPos);
        tornado3.transform.localScale = new Vector2(5, 5);
        SetActiveTornado(false);
        midPoint = (startPoint + endPoint) / 2;
        fullHp = maxHp = HP;
        isStun = false;
        isMeetPlayer = false;
        isDisappear = false;
        deathState = DeathType.Default;
    }

    public IHittable target;

    void Update()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        // 싸움 중인지 확인
        if (isAttacking)
        {
            attackElapsedTime += Time.deltaTime;
            if (bossAI._rechargeTime <= attackElapsedTime)
            {
                attackElapsedTime = 0;
                // 스킬 사용중이라는 값 반환
                isAttacking = false;
            }
        }

        if(isAlive == false && bossCollider.isActiveAndEnabled == true)
        {
            HP = 0;
            MoveStop();
            bossAI.enabled = false;
            deathState = DeathType.Be_Rock;
            DeathAnimation(deathState);
            bossCollider.enabled = false;
        }
    }

    public Collider2D GetCollider2D()
    {
        return bossCollider;
    }

    public void Hit(Strike strike)
    {
        hpBar.UpdateBossHPBar();
        if (HP > 0)
        {
            if (transform.rotation.eulerAngles.y == player.transform.rotation.eulerAngles.y)
            {
                transform.rotation = Quaternion.Euler(0, -(180 - transform.rotation.eulerAngles.y), 0);
            }

            if (bossAI._skill != GargoyleBrain.Skill.Dash)
            {
            }

            // 스턴
            if (HP <= fullHp / 2)
            {
                isStun = true;
                myPlayer.Play(stunStartClip, stunIdleClip, false);
            }

            if (isStun == true)
            {
                // 스턴을 일정 시간동안 계속 재생
                myPlayer.Play(stunIdleClip);
                stun.SetActive(true);
                fullHp = -1;
                StopAllCoroutines();
                StartCoroutine(DoStun());
                IEnumerator DoStun()
                {
                    yield return new WaitForSeconds(1.2f);
                    myPlayer.Play(stunEndClip, idleClip, false);
                    stun.SetActive(false);
                    isStun = false;
                }
            }

            if (isStun == false)
            {
                myPlayer.Play(hitClip, idleClip, false);
            }

            // HP 감소
            HP += strike.result;
        }
    }

    // 스킬을 사용중인가에 대한 함수
    public bool IsAttacking()
    {
        //attackElapsedTime += Time.deltaTime;

        //// attackCoolTime이 차서 공격 실행하면
        //if (bossAI._rechargeTime <= attackElapsedTime)
        //{
        //    attackElapsedTime = 0;
        //    // 스킬 사용중이라는 값 반환
        //    return true;
        //}
        //else
        //{
        //    // 스킬 사용중 아니라는 값 반환
        //    return false;
        //}
        return isAttacking;
    }

    public void MovePosition(float targetPosX)
    {
        if (bossAI._skill == GargoyleBrain.Skill.Dash)
        {
            myPlayer.Play(dashLoopClip);
        }

        if (transform.position.x < targetPosX)
        {
            MoveRight();
        }
        else if (transform.position.x > targetPosX)
        {
            MoveLeft();
        }
    }

    // 도착하면 안 움직이게 하는 함수 
    public override void MoveStop()
    {
        if(deathState == DeathType.Default)
        {
            myPlayer.Play(idleClip);
        }
        if (myRigid.velocity.y != 0)
        {
            myRigid.velocity = new Vector2(0, 0);
        }
        else
        {
            base.MoveStop();
        }
    }

    public void UTurn()
    {
        if (transform.position.x > midPoint.x)
        {
            if (transform.rotation.eulerAngles.y <= 0)
            {
                MoveStop();
                if (bossAI._skill == GargoyleBrain.Skill.Dash)
                {
                    myPlayer.Play(dashStartClip);
                }
                else
                {
                    myPlayer.Play(uTurnClip, idleClip, true);
                }
                transform.rotation = Quaternion.Euler(0, -180, 0);
            }
        }
        else
        {
            if (transform.rotation.eulerAngles.y >= 180)
            {
                MoveStop();
                if (bossAI._skill == GargoyleBrain.Skill.Dash)
                {
                    myPlayer.Play(dashStartClip);
                }
                else
                {
                    myPlayer.Play(uTurnClip, idleClip, true);
                }
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }
    }

    // 대각선으로 떨어지는 함수
    public void DropDiagonalMove()
    {
        myPlayer.Play(dashLoopClip);
        // 내 위치가 중간 지점보다 크면
        if (transform.position.x >= midPoint.x)
        {
            // 왼쪽 끝에 있는 곳이 목표
            pointPos = startPoint;
            oppositePointX = endPoint.x;
            OnDrawGizmos();

            destination = (Vector2)transform.position - pointPos;
            myRigid.velocity = -destination * (dropSpeed * 0.3f);
        }
        // 내 위치가 중간 지점보다 작으면
        else
        {
            // 오른쪽 끝에 있는 곳이 목표
            pointPos = endPoint;
            oppositePointX = startPoint.x;
            OnDrawGizmos();

            destination = (Vector2)transform.position - pointPos;
            myRigid.velocity = -destination * (dropSpeed * 0.3f);
        }
    }

    // 공중으로 날라가는 함수
    public void FlyMove()
    {
        if (bossAI._skill == GargoyleBrain.Skill.Dash)
        {
            myPlayer.Play(dashLoopClip);
        }

        // 내 위치가 중간 지점보다 오른쪽에 있으면
        if (transform.position.x >= midPoint.x)
        {
            pointPos = new Vector2(startPoint.x, maxHeight);
            OnDrawGizmos();
            destination = pointPos - (Vector2)transform.position;
            myRigid.velocity = destination * (flySpeed * 0.3f);
        }

        else if (transform.position.x < midPoint.x)
        {
            pointPos = new Vector2(endPoint.x, maxHeight);
            OnDrawGizmos();
            destination = pointPos - (Vector2)transform.position;
            myRigid.velocity = destination * (flySpeed * 0.3f);
        }
    }

    // 내려 날아와서 반대쪽으로 이동하는 함수
    public void MoveOppositeEndPoint()
    {
        MovePosition(oppositePointX);
    }
    /// <summary>
    /// 내가 플레이어 위치를 넣어야 하는 플레이어 따라가는 함수
    /// </summary>
    /// <param name="playerPosX"> 플레이어 위치</param>
    // 플레이어 따라다니면서 이동하는 함수
    public void FollowPlayer(float playerPosX)
    {
        MovePosition(playerPosX);
    }

   /// <summary>
   /// 플레이어 위치가 이미 들어있는 플레이어 따라 가는 함수
   /// </summary>
    public void FollowPlayer()
    {
        MovePosition(player.transform.position.x);
    }

    // 임시
    new void OnDrawGizmos()
    {
        Debug.DrawRay(pointPos, (Vector2)transform.position - pointPos);
    }

    [SerializeField]
    private Strike _comboAttack1Strike1;
    [SerializeField]
    private Vector4 _comboAttack1Square;
    [SerializeField]
    private Strike _comboAttack1Strike2;
    [SerializeField]
    private Vector4 _comboAttack1Capsule;
    [SerializeField]
    private GameObject _hitObject;
    [SerializeField]
    private float _comboAttack1Delay1 = 0.6f;
    [SerializeField]
    private float _comboAttack1Delay2 = 0.2f;

    [SerializeField]
    private Strike _comboAttack2Strike;
    [SerializeField]
    private float _comboAttack2Radius;
    [SerializeField]
    private Vector2 _comboAttack2Offset;

    private string playerTag = "Player";

    public void ComboAttack1()
    {
        isAttacking = true;
        myPlayer.Play(comboAttack1Clip, idleClip, false);
        StartCoroutine(DoPlay());
        IEnumerator DoPlay()
        {
            target = null;
            yield return new WaitForSeconds(_comboAttack1Delay1);
            Vector2 size = new Vector2(_comboAttack1Square.x, _comboAttack1Square.y);
            Vector2 offset = new Vector2(_comboAttack1Square.z, _comboAttack1Square.w);
#if UNITY_EDITOR
            Shape.Draw(BoxShape.GetVertices(getTransform, size, offset), Color.red, 2);
#endif
            offset = new Vector2(offset.x * getTransform.forward.z, offset.y);
            Collider2D[] collider2Ds = Physics2D.OverlapBoxAll((Vector2)getTransform.position + offset, size, getTransform.eulerAngles.z);
            for(int i = 0; i < collider2Ds.Length; i++)
            {
                IHittable hittable = collider2Ds[i].GetComponent<IHittable>();
                if(hittable != null && hittable.tag == playerTag)
                {
                    target = hittable;
                    GameManager.Use(_comboAttack1Strike1, new Strike.TargetArea(new IHittable[] { hittable }), _hitObject);
                }
            }
            yield return new WaitForSeconds(_comboAttack1Delay2);
            size = new Vector2(_comboAttack1Capsule.x, _comboAttack1Capsule.y);
            offset = new Vector2(_comboAttack1Capsule.z, _comboAttack1Capsule.w);
#if UNITY_EDITOR
            Shape.Draw(CapsuleShape.GetVertices(getTransform, CapsuleDirection2D.Horizontal, size, offset), Color.red, 2);
#endif
            offset = new Vector2(offset.x * getTransform.forward.z, offset.y);
            collider2Ds = Physics2D.OverlapCapsuleAll((Vector2)getTransform.position + offset, size, CapsuleDirection2D.Horizontal, getTransform.eulerAngles.z);
            for (int i = 0; i < collider2Ds.Length; i++)
            {
                IHittable hittable = collider2Ds[i].GetComponent<IHittable>();
                if (hittable != null && hittable.tag == playerTag)
                {
                    target = hittable;
                    GameManager.Use(_comboAttack2Strike, new Strike.TargetArea(new IHittable[] { hittable }), _hitObject);
                }
            }
        }
    }

    public void ComboAttack2()
    {
        isAttacking = true;
        // 위로 잠깐 떠오름 추가 필요(maxHeight / 2)
        //pointPos = new Vector2(transform.position.x, maxHeight / 2);
        //OnDrawGizmos();
        //destination = pointPos - (Vector2)transform.position;
        //myRigid.velocity = destination * (flySpeed * 0.2f);
        myPlayer.Play(comboAttack2Clip, idleClip, false);
        StartCoroutine(DoPlay());
        IEnumerator DoPlay()
        {
            while (isGrounded == true)
            {
                yield return null;
            }
            while (isGrounded == false)
            {
                yield return null;
            }
#if UNITY_EDITOR
            Shape.Draw(CircleShape.GetVertices(getTransform, _comboAttack2Radius, _comboAttack2Offset), Color.red, 2);
#endif
            Vector2 offset = new Vector2(_comboAttack2Offset.x * getTransform.forward.z, _comboAttack2Offset.y);
            Collider2D[] collider2Ds = Physics2D.OverlapCircleAll((Vector2)getTransform.position + offset, _comboAttack2Radius);
            for (int i = 0; i < collider2Ds.Length; i++)
            {
                IHittable hittable = collider2Ds[i].GetComponent<IHittable>();
                if (hittable != null && hittable.tag == playerTag)
                {
                    GameManager.Use(_comboAttack1Strike2, new Strike.TargetArea(new IHittable[] { hittable }), _hitObject);
                }
            }
        }
    }

    // 중간으로 올라가는 함수 필요
    public void MoveMiddleSpot()
    {
        pointPos = new Vector2(midPoint.x, maxHeight);
        OnDrawGizmos();

        destination = pointPos - (Vector2)transform.position;
        myRigid.velocity = destination * (flySpeed * 0.3f);
    }

    [SerializeField]
    private Projectile _dashProjectile;

    public void AdjustRotation()
    {
        Vector2 velocity = getRigidbody2D.velocity;
        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        if (velocity.x >= 0)
        {
            getTransform.rotation = Quaternion.Euler(0, 0, angle);
        }
        else
        {
            getTransform.rotation = Quaternion.Euler(180, 0, -angle);
        }
    }

    public void UseDashSkill(float duration)
    {
        Projectile projectile = GameManager.GetProjectile(_dashProjectile);
        projectile.Shot(getTransform, null, GameManager.ShowEffect, GameManager.Use, null, duration);
    }


    public void PlayerEnterBossStage()
    {
        myPlayer.Play(surprisedClip);
        isMeetPlayer = true;
    }

    public bool IsEndAnimation()
    {
        if(myPlayer.isEndofFrame)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void FightParticipation()
    {
        myPlayer.Play(fightStartClip, idleClip, false);

        pointPos = new Vector2(endPoint.x, transform.position.y);
        OnDrawGizmos();

        destination = pointPos - (Vector2)transform.position;
        myRigid.velocity = destination * 0.8f;
    }

    void MoveToPointPosition(Vector2 pointSpotPos, float speed)
    {
        destination = pointSpotPos - (Vector2)transform.position;
        myRigid.velocity = destination * speed;
    }

    bool IsArrivePointSpot(Vector2 pointSpotPos)
    {
        if(Vector2.Distance(transform.position, pointSpotPos) <= 1f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public void SetActiveTornado(bool isOn)
    {
        tornado1.SetActive(isOn);
        tornado2.SetActive(isOn);
        tornado3.SetActive(isOn);
    }

    public void DeathAnimation(DeathType state)
    {
        switch(state)
        {
            case DeathType.Be_Rock:
                transform.position = new Vector2(transform.position.x, startPoint.y + 2.0f);
                myPlayer.Play(death1Clip);
                isDead = true;
                break;
            case DeathType.Awake:
                myPlayer.Play(death2Clip);
                break;
            case DeathType.Be_AnotherBoss:
                pointPos = new Vector2(transform.position.x, maxHeight / 2);
                MoveToPointPosition(pointPos, 0.8f);
                myPlayer.Play(death3Clip);
                SetActiveTornado(true);
                break;
            case DeathType.GoOutside:
                pointPos = new Vector2(transform.position.x, startPoint.y);
                MoveToPointPosition(pointPos, 0.8f);
                myPlayer.Play(death4Clip);
                break;
            case DeathType.Death:
                if(transform.rotation.eulerAngles.y == 180)
                {
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                }
                pointPos = new Vector2(endPoint.x + 15.0f, transform.position.y);
                MoveToPointPosition(pointPos, 1f);
                if(IsArrivePointSpot(pointPos) == true)
                {
                    gameObject.SetActive(false);
                    isDisappear = true;
                }
                break;
        }
    }
}

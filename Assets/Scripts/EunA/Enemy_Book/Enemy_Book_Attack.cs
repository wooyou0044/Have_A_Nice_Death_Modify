using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Enemy_Book_Attack : MonoBehaviour
{
    public GameObject PaperPlane;
    public GameObject PaperPlaneMuzzle;
    public bool isAttack = false;
    public bool isEnabled;
    float PlaneCoolTime;
    float PlaneElapsedTime;

    [SerializeField]
    private Projectile Projectile;

    void Start()
    {
        PlaneCoolTime = 1.0f;
        PlaneElapsedTime = 1.0f;
        isEnabled = false;
        isAttack = true;
    }

    void Update()
    {
        PlaneElapsedTime += Time.deltaTime;
        if (isEnabled == true && isAttack == true && PlaneElapsedTime >= PlaneCoolTime) 
        {
            Projectile projectile = GameManager.GetProjectile(Projectile);
            IHittable hittable = GameObject.FindWithTag("Player").GetComponent<IHittable>();
            projectile.Shot(PaperPlaneMuzzle.transform, hittable, GameManager.ShowEffect, GameManager.Use);
            //Instantiate(PaperPlane, PaperPlaneMuzzle.transform.position, PaperPlaneMuzzle.transform.rotation);
            PlaneElapsedTime = 0;

        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Wall")
        {
            Destroy(gameObject);
        }
    }
}

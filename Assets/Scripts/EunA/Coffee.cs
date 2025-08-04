using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Aseprite;
using UnityEngine;

public class Coffee : MonoBehaviour
{
    public GameObject Coffee_Image;
    public GameObject Coffee_Info;
    public GameObject GetItemEffect;

    [SerializeField]
    LayerMask playerLayerMask;

    Collider2D CanGetDistance;
    float ItemGetRange = 3.0f;
    float Destroytime;

    bool isGetCoffee = false;

    void Start()
    {
        
    }

    void Update()
    {
        CanGetDistance = Physics2D.OverlapCircle(transform.position, ItemGetRange, playerLayerMask);

        if(CanGetDistance != null && isGetCoffee == false)
        {
            Coffee_Info.SetActive(true);
        }

        else
        {
            Coffee_Info.SetActive(false);
        }

        //if (isGetCoffee == true)
        //{
        //    Destroytime += Time.deltaTime;

        //    if (Destroytime >= 1.0f)
        //    {
        //        Destroy(gameObject);
        //    }
        //}
    }    

    //private void OnTriggerStay2D(Collider2D collision)
    //{
    //    if (Input.GetKey(KeyCode.F))
    //    {
    //        GetCoffee();
    //        isGetCoffee = true;
    //    }
    //}

    public bool GetCoffee()
    {
        if (CanGetDistance != null && Coffee_Image.activeInHierarchy == true)
        {
            Coffee_Image.SetActive(false);
            Coffee_Info.SetActive(false);
            GetItemEffect.SetActive(true);
            isGetCoffee = true;
            return true;
        }
        return false;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(this.transform.position, ItemGetRange);
    }
#endif
}

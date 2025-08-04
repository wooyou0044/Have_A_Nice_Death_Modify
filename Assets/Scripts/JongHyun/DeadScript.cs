using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DeadScript : MonoBehaviour
{
    [SerializeField]
    Image deadImage;
    [SerializeField]
    Text deadText;

    private void Start()
    {
        deadText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (deadImage.color.a == 0)
        {
            deadText.gameObject.SetActive(true);            
        }
    }
}

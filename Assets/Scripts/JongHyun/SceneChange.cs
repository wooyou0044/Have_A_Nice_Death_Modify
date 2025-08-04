using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChange : MonoBehaviour
{
    [SerializeField]
    GameObject CanvasPanel;
    [SerializeField]
    Image Image;
    [SerializeField]
    Animator ani;
    public void MainSceneChanger()
    {
        CanvasPanel.SetActive(true);        
    }
    private void FixedUpdate()
    {
        if (Image.color.g == 0)
        {
            SceneManager.LoadScene("JonghyunTest");
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss_Hp_Bar : MonoBehaviour
{
    [SerializeField] private BossMovement boss;
    [SerializeField] Slider slider;

    private void OnEnable()
    {
        slider.maxValue = boss.currentHP;
        slider.value = boss.currentHP;
    }

    public void UpdateBossHPBar()
    {
        //할당이 되었다면?
        if (boss != null && slider != null)
        {
            slider.value = boss.currentHP;
        }
        if(boss.currentHP <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}

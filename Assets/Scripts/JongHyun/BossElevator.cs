using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossElevator : MonoBehaviour
{
    [SerializeField]
    Player player;
    public bool _playerOut;
    private void Start()
    {
        if (player == null)
        {
            player = GetComponent<Player>();
        }
    }
    void Update()
    {
        if (_playerOut == false)
        {
            player.gameObject.SetActive(false);
        }        
        else
        {
            player.gameObject.SetActive(true);
        }
    }
}

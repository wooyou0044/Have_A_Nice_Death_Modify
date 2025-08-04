using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeskMovement : MonoBehaviour
{
    Animator myAnimator;

    void Awake()
    {
        myAnimator = GetComponent<Animator>();
    }

    void Start()
    {
        myAnimator.enabled = false;
    }

    public void TurnOffAnimator()
    {
        myAnimator.enabled = false;
    }

    public void TurnOnAnimator()
    {
        myAnimator.enabled = true;
    }
}

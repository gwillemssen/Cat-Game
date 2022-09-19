using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cat : Interactable
{
    private Animator anim;

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        anim.SetBool("Excited", LookingAt);
    }
}

using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cat : Interactable
{
    //inspector
    public enum CatStartState { Pettable, Unpettable } 
    public CatStartState StartState;

    //general
    private enum CatState { Pettable, Unpettable, PettingMinigame, DonePetting };
    private CatState state;
    private Animator anim;
    
    //Minigame
    private Vector3 catOriginalPos;
    private Quaternion catOriginalRot;
    private Transform catHoldingPosition;
    private FirstPersonController playerController;
    private float timeStartedMovingCat = -420f;

    private void Start()
    {
        state = (CatState)StartState;
        anim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        anim.SetBool("Excited", LookingAt && state == CatState.Pettable);

        if (state == CatState.PettingMinigame || state == CatState.DonePetting)
        {
            float t = Time.time - timeStartedMovingCat;
            t = Mathf.Clamp01(t);
            t = t * t * t * (t * (6f * t - 15f) + 10f); //smootherstep

            transform.position = Vector3.Lerp(catOriginalPos, catHoldingPosition.position, t);
            transform.rotation = Quaternion.Lerp(catOriginalRot, catHoldingPosition.rotation, t);

            if (state == CatState.DonePetting && t == 1f)
            {
                EndMinigame();
            }
        }
    }

    public override void Interact(FirstPersonController controller)
    {
        if(state == CatState.Pettable)
        {
            catHoldingPosition = controller.CatHoldingPosition;
            playerController = controller;
            StartMinigame();
        }
    }

    private void StartMinigame()
    {
        catOriginalPos = transform.position;
        catOriginalRot = transform.rotation;
        timeStartedMovingCat = Time.time;
        state = CatState.PettingMinigame;
        playerController.enabled = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void EndMinigame()
    {
        state = CatState.Pettable;
        playerController.enabled = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        //maybe this should this change back to the startState?
    }
}

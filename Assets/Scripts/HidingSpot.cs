using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioPlayer))]
public class HidingSpot : Interactable, IGrannyInteractable
{
    public Transform HidingPosition;
    public Transform GrannyOpenPosition;
    public float EnterSpeed = 2f;
    public float ExitSpeed = 0.75f;


    private FirstPersonController FPController;
    private Vector3 startPos;
    private Quaternion startRot;

    private float lerp;
    private bool entering;
    private bool justEntered;

    public virtual void OnEnterHidingSpot() { }

    public override void Interact(FirstPersonController controller)
    {
        if (FPController != null)
            return;

        FPController = controller;
        startPos = FPController.transform.position;
        startRot = FPController.transform.rotation;
        FPController.DisableMovement = true;
        //FPController.UI.SetInfoText("Hiding...");
        FPController.Hiding = true;
        entering = true;
        lerp = 0f;
        justEntered = true;

        Enemy.instance.OnEnteredHidingSpotCallback(this);
        OnEnterHidingSpot();
    }

    private void Update()
    {
        if(FPController != null)
        {
            if (entering && lerp != 1f)
            {
                FPController.transform.position = Vector3.Lerp(startPos, HidingPosition.position, lerp);
                FPController.transform.rotation = Quaternion.Lerp(startRot, HidingPosition.rotation, lerp);
            }
            else if(lerp != 1f)
            {
                FPController.transform.position = Vector3.Lerp(HidingPosition.position, startPos, lerp);
                //FPController.transform.rotation = Quaternion.Lerp(HidingPosition.rotation, startRot, lerp);
            }

            if (entering)
            { lerp += Time.deltaTime * EnterSpeed; }
            else
            { lerp += Time.deltaTime * ExitSpeed; }

            lerp = Mathf.Clamp01(lerp);
            if(lerp >= 1f)
            {
                if(entering)
                {
                    //Debug.Log(FPController.Hiding);
                }
                else
                {
                    
                    FPController.DisableMovement = false;
                    FPController.Hiding = false;
                    FPController = null;
                    
                }
            }

            if(FPController != null && FPController.Input.interactedOnce && entering && !justEntered)
            {
                Exit();
            }
        }

        justEntered = false;
    }

    public virtual void OnExitHidingSpot() {}

    public void GrannyInteract()
    {
        Exit();
    }

    private void Exit()
    {
        OnExitHidingSpot();
        entering = false;
        lerp = 0f;
    }
}

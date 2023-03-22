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


    private FirstPersonController player;
    private Vector3 startPos;
    private Quaternion startRot;

    private float lerp;
    private bool entering;

    public static event Action<HidingSpot> OnEnteredHidingSpot;

    public virtual void OnEnterHidingSpot() { }

    public override void Interact(FirstPersonController controller)
    {
        if (player != null)
            return;

        player = controller;
        startPos = player.transform.position;
        startRot = player.transform.rotation;
        player.DisableMovement = true;
        player.UI.SetInfoText("Hiding...\nRight click to exit");
        player.Hiding = true;
        entering = true;
        lerp = 0f;

        OnEnteredHidingSpot?.Invoke(this);
        OnEnterHidingSpot();
    }

    private void Update()
    {
        if(player != null)
        {
            if (entering)
            {
                player.transform.position = Vector3.Lerp(startPos, HidingPosition.position, lerp);
                player.transform.rotation = Quaternion.Lerp(startRot, HidingPosition.rotation, lerp);
            }
            else
            {
                player.transform.position = Vector3.Lerp(HidingPosition.position, startPos, lerp);
                //player.transform.rotation = Quaternion.Lerp(HidingPosition.rotation, startRot, lerp);
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
                    //Debug.Log(player.Hiding);
                }
                else
                {
                    
                    player.DisableMovement = false;
                    player.Hiding = false;
                    player = null;
                    
                }
            }

            if(player != null && player.Input.throwing && entering)
            {
                Exit();
            }
        }
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

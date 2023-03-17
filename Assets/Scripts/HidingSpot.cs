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
    public Animation anim;
    public Sound DoorOpenSound;
    public Sound DoorCloseSound;

    private FirstPersonController player;
    private Vector3 startPos;
    private Quaternion startRot;
    private AudioPlayer audioPlayer;

    private float lerp;
    private bool entering;

    public static event Action<HidingSpot> OnEnteredHidingSpot;

    private void Start()
    {
        anim = GetComponentInChildren<Animation>();
        audioPlayer = GetComponent<AudioPlayer>();
    }

    public override void Interact(FirstPersonController controller)
    {
        audioPlayer.Play(DoorOpenSound);
        if (player != null)
            return;
        player = controller;
        startPos = player.transform.position;
        startRot = player.transform.rotation;
        player.DisableMovement = true;
        player.UI.SetInfoText("Hiding...\nRight click to exit");
        player.Hiding = true;
        anim.Stop();
        anim.Play();
        entering = true;
        lerp = 0f;

        OnEnteredHidingSpot?.Invoke(this);
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

    private void Exit()
    {
        audioPlayer.Play(DoorCloseSound);
        entering = false;
        lerp = 0f;
        anim.Stop();
        anim.Play();
    }

    public void GrannyInteract()
    {
        Exit();
    }
}

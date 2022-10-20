using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidingSpot : Interactable
{
    public Transform HidingPosition;
    public Animation anim;
    private FirstPersonController player;
    private Vector3 startPos;

    private void Start()
    {
        anim = GetComponentInChildren<Animation>();
    }

    public override void InteractClick(FirstPersonController controller)
    {
        AudioManager.instance.Play("Door");
        if (player != null)
            return;
        player = controller;
        startPos = player.transform.position;
        player.DisableMovement = true;
        player.UI.SetInfoText("Hiding...\nRight click to exit");
        player.transform.position = HidingPosition.position;
        player.transform.rotation = HidingPosition.rotation;
        player.Hiding = true;
        anim.Stop();
        anim.Play();
    }

    private void Update()
    {
        if(player != null)
        {
            if(player.Input.throwing)
            {
                player.transform.position = startPos;
                player.DisableMovement = false;
                player.Hiding = false;
                player = null;
                anim.Stop();
                anim.Play();
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidingSpot : Interactable
{
    public Transform HidingPosition;
    public float EnterSpeed = 2f;
    public float ExitSpeed = 0.75f;
    public Animation anim;
    private FirstPersonController player;
    private Vector3 startPos;
    private Quaternion startRot;

    private float lerp;
    private bool entering;

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
        startRot = player.transform.rotation;
        player.DisableMovement = true;
        player.UI.SetInfoText("Hiding...\nRight click to exit");
        player.Hiding = true;
        anim.Stop();
        anim.Play();
        entering = true;
        lerp = 0f;
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
                    Debug.Log(player.Hiding);
                }
                else
                {
                    player.DisableMovement = false;
                    player.Hiding = false;
                    player = null;
                }
            }

            if(player.Input.throwing)
            {
                entering = false;
                lerp = 0f;
                anim.Stop();
                anim.Play();
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidingCloset : HidingSpot
{
    public Animation anim;
    public AudioClip DoorOpenSound, DoorCloseSound;

    private void Start()
    {
        anim = GetComponentInChildren<Animation>();

    }

    public override void OnEnterHidingSpot()
    {
        PlaySound(DoorOpenSound);
        anim.Stop();
        anim.Play();
    }

    public override void OnExitHidingSpot()
    {
        PlaySound(DoorCloseSound);
        anim.Stop();
        anim.Play();
    }
}

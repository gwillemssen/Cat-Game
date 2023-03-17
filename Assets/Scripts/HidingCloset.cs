using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidingCloset : HidingSpot
{
    public Animation anim;
    public Sound DoorOpenSound;
    public Sound DoorCloseSound;
    private AudioPlayer audioPlayer;

    private void Start()
    {
        anim = GetComponentInChildren<Animation>();
        audioPlayer = GetComponent<AudioPlayer>();
    }

    public override void OnEnterHidingSpot()
    {
        audioPlayer.Play(DoorOpenSound);
        anim.Stop();
        anim.Play();
    }

    public override void OnExitHidingSpot()
    {
        audioPlayer.Play(DoorCloseSound);
        anim.Stop();
        anim.Play();
    }
}

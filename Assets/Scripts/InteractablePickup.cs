using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioPlayer))]
public class InteractablePickup : Interactable
{
    public Rigidbody Rigidbody { get; private set; }
    public Transform OriginalParent { get; private set; }

    public Sound ImpactSound;

    private AudioPlayer audioPlayer;

    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
        audioPlayer = GetComponent<AudioPlayer>();
        OriginalParent = transform.parent;
    }

    public override void InteractClick(FirstPersonController controller)
    {
        controller.UI.SetInfoText("Left click to use\nRight click to drop");
    }


}

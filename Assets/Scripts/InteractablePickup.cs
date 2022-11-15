using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioPlayer))]
public class InteractablePickup : Interactable
{
    public Rigidbody Rigidbody { get; private set; }
    public float ImpactVelocity = 1f;

    public Sound ImpactSound;

    private AudioPlayer audioPlayer;
    private float sqrImpactSoundVelocity;

    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
        audioPlayer = GetComponent<AudioPlayer>();
        sqrImpactSoundVelocity = ImpactVelocity * ImpactVelocity;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if(ImpactSound == null)
        { return; }

        if(Rigidbody.velocity.sqrMagnitude > sqrImpactSoundVelocity)
        {
            audioPlayer.Play(ImpactSound);
            Impacted();
        }
    }

    public virtual void Impacted()
    {

    }

    public override void Interact(FirstPersonController controller)
    {
        PlayerUI.instance.SetInfoText("Left click to use\nRight click to drop");
    }


}

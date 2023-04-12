using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioPlayer))]
//a better name for this would be 'Throwable' but I dont want to change it and risk losing references on a bunch of prefabs
public class InteractablePickup : Interactable
{
    //this throwable will impact at any velocity.
    //This is set to true in FirstPersonInteraction to make it impact when the throw is charged to more than 50%
    public bool AlwaysMakeImpact = false;
    public Rigidbody Rigidbody { get; private set; }
    public float ImpactVelocity = 1f;

    public Sound ImpactSound;

    private AudioPlayer audioPlayer;
    private float sqrImpactVelocity;
    private bool canImpact = false;

    public bool FrozenOnAwake;
    RigidbodyConstraints originalConstraints;
    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
        audioPlayer = GetComponent<AudioPlayer>();
        sqrImpactVelocity = ImpactVelocity * ImpactVelocity;
        originalConstraints = Rigidbody.constraints;
        if (FrozenOnAwake) Freeze();
    }

    public void OnCollisionEnter(Collision collision)
    {
        if(ImpactSound == null || !canImpact)
        { return; }

        if(Rigidbody.velocity.sqrMagnitude > sqrImpactVelocity)
        {
            audioPlayer.Play(ImpactSound);
            Enemy.instance.Distract(transform.position);
            Impacted();
        }
    }

    public virtual void Impacted()
    {

    }

    public override void Interact(FirstPersonController controller)
    {
        PlayerUI.instance.SetInfoText("Press Right Click to Drop\nHold Right Click to Throw");
        Rigidbody.constraints = originalConstraints;
        canImpact = true;
    }

    private void Freeze()
    {
        Rigidbody.constraints = RigidbodyConstraints.FreezeAll;
    }


}

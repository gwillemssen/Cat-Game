using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]

//a better name for this would be 'Throwable' but I dont want to change it and risk losing references on a bunch of prefabs
public class InteractablePickup : Interactable
{
    //this throwable will impact at any velocity.
    //This is set to true in FirstPersonInteraction to make it impact when the throw is charged to more than 50%
    public bool AlwaysMakeImpact = false;
    public Rigidbody Rigidbody { get; private set; }
    public float ImpactVelocity = 1f;

    public List<AudioClip> PickupSounds;
    public List<AudioClip> ImpactSounds;

    [SerializeField] private UnityEvent onImpact;
    private float sqrImpactVelocity;
    private bool canImpact = false;

    public bool FrozenOnAwake;
    RigidbodyConstraints originalConstraints;
    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
        sqrImpactVelocity = ImpactVelocity * ImpactVelocity;
        originalConstraints = Rigidbody.constraints;
        if (FrozenOnAwake) Freeze();
    }

    public void OnCollisionEnter(Collision collision)
    {
        if(!canImpact)
        { return; }

        //Debug.Log("impact: " + collision.gameObject.name);

        Enemy enemy = collision.gameObject.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.Stun();
        }

        if(Rigidbody.velocity.sqrMagnitude > sqrImpactVelocity)
        {
            if(ImpactSounds.Count > 0)
            { base.PlayRandomSound(ImpactSounds); }
            Enemy.instance.Distract(transform.position);
            onImpact?.Invoke();
            Impacted();
        }
    }

    public virtual void Impacted()
    {

    }
    //called when we left click while holding it.
    public virtual void Click()
    {
        //print("CLICKY");
    }

    public override void Interact(FirstPersonController controller)
    {
        PlayerUI.instance.SetInfoText("Press Right Click to Throw");
        Rigidbody.constraints = originalConstraints;
        if (PickupSounds.Count > 0)
        { base.PlayRandomSound(PickupSounds); }
        canImpact = true;
    }

    private void Freeze()
    {
        Rigidbody.constraints = RigidbodyConstraints.FreezeAll;
    }


}

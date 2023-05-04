using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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

    public List<Sound> PickupSounds;
    public List<Sound> ImpactSounds;

    [SerializeField] private UnityEvent onImpact;
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
            { audioPlayer.Play(ImpactSounds[Random.Range(0, ImpactSounds.Count)]); }
            Enemy.instance.Distract(transform.position);
            onImpact?.Invoke();
            Debug.Log("ASCAGSCAYDYIASYTDC");
            Impacted();
        }
    }

    public virtual void Impacted()
    {

    }
    //called when we left click while holding it.
    public virtual void Click()
    {

    }

    public override void Interact(FirstPersonController controller)
    {
        PlayerUI.instance.SetInfoText("Press Right Click to Throw");
        Rigidbody.constraints = originalConstraints;
        if (PickupSounds.Count > 0)
        { audioPlayer.Play(PickupSounds[Random.Range(0, PickupSounds.Count)]); }
        canImpact = true;
    }

    private void Freeze()
    {
        Rigidbody.constraints = RigidbodyConstraints.FreezeAll;
    }


}

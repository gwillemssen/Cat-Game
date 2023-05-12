using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    [HideInInspector] public bool LookingAt = false; //is the player looking at the object?
    [HideInInspector] public bool Disabled = false;
    public bool CanInteract = true;
    public float Cooldown;
    public bool VisiblyLockedOnView = false;
    public bool Open;
    public string RequiredItemToViewInteraction = "";

    public AudioSource player;

    [Tooltip("Called when we click on the object")]
    public UnityEvent OnInteract;

    private float lastTimeInteracted = -420f;

    /// <summary>
    /// Called every frame as you hold left mouse on the object
    /// </summary>
    /// <param name="controller">Reference to the player controller</param>
    public virtual void InteractHold(FirstPersonController controller)
    {

    }
    /// <summary>
    /// Called when the player clicks on the object
    /// </summary>
    /// <param name="controller">Reference to the player controller</param>
    public virtual void Interact(FirstPersonController controller)
    {

    }

    public void InteractBase()
    {
        if(Cooldown <= 0f)
        { return; }

        lastTimeInteracted = Time.time;
        Disabled = true;
    }

    private void FixedUpdate()
    {
        if(Time.time - lastTimeInteracted > Cooldown)
        {
            Disabled = false;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="withInteractable">Interactable that the player is holding when they clicked on this one.  Useful for keys / tools</param>
    public virtual void InteractWith(FirstPersonController controller, Interactable withInteractable)
    {
    }

    public virtual void OpenHinge(GameObject Hinge, Vector3 startingLocation, Vector3 endingLocation, float openTime, float closeTime)
    {
        Open = !Open;
        ToggleChildColliders(Hinge, false);
        LeanTween.rotateLocal(Hinge,Open?startingLocation:endingLocation,Open?openTime:closeTime).setEaseInOutExpo().setOnComplete(()=>ToggleChildColliders(Hinge,true));
    }

    public virtual void PlayInteractionSound(AudioClip sound)
    {
        player.clip = sound;
        player.Play();
    }

    public virtual void PlayRandomInteractionSound(List<AudioClip> sounds)
    {
        player.clip = sounds[Random.Range(0, sounds.Count)];
        player.Play();
    }

    public void PlayAnimation()
    {   
        GetComponent<Animation>().Play();
    }

    void ToggleChildColliders(GameObject parent)
    {
        // Get all child game objects of the parent
        foreach (Transform child in parent.transform)
        {
            // If the child has a collider, turn it off
            Collider collider = child.GetComponent<Collider>();
            if (collider != null)
            {
                if (collider.enabled) { collider.enabled = false; }
                else { collider.enabled = true; }
                
            }

            // Recursively call this function on the child's children
            ToggleChildColliders(child.gameObject);
        }
    }

    void ToggleChildColliders(GameObject parent, bool on)
    {
        // Get all child game objects of the parent
        foreach (Transform child in parent.transform)
        {
            // If the child has a collider, turn it off
            Collider collider = child.GetComponent<Collider>();
            if (collider != null) collider.enabled = on;

            // Recursively call this function on the child's children
            ToggleChildColliders(child.gameObject);
        }
    }
}
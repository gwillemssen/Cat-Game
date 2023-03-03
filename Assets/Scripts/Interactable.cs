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
        OnInteract?.Invoke();
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
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    [HideInInspector] 
    public bool LookingAt = false; //is the player looking at the object?
    public bool CanInteract = true;
    public bool NoiseCrosshair = false;

    [Tooltip("Called when we click on the object")]
    public UnityEvent OnInteract;

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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="withInteractable">Interactable that the player is holding when they clicked on this one.  Useful for keys / tools</param>
    public virtual void InteractWith(FirstPersonController controller, Interactable withInteractable)
    {

    }
}
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
    public virtual void InteractHold(FirstPersonController controller)
    {

    }
    /// <summary>
    /// Called when the player clicks on the object
    /// </summary>
    /// <param name="controller"></param>
    public virtual void InteractDown(FirstPersonController controller)
    {
        OnInteract?.Invoke();
    }
}
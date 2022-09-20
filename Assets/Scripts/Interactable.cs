using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class Interactable : MonoBehaviour
{
    
    [HideInInspector] 
    public bool LookingAt = false; //is the player looking at the object?

    //we are pressing e on the object
    public virtual void Interact(FirstPersonController controller)
    {

    }
}

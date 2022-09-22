using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [HideInInspector] 
    public bool LookingAt = false; //is the player looking at the object?

    //we are clicking the object
    public virtual void Interact(FirstPersonController controller)
    {

    }
    //we are clicking the object (ONLY CALLED THE FIRST FRAME)
    public virtual void InteractClick(FirstPersonController controller)
    {

    }
}

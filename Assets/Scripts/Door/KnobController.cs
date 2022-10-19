using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnobController : Interactable
{
    public bool Locked = false;
    public string KeyName = "TestKey";

    private DoorAnimController parent;
   
    private void Start()
    {
        parent = transform.parent.parent.GetComponent<DoorAnimController>();
       
    }

    public override void InteractClick(FirstPersonController controller)
    {
        if (Locked && !parent.open)
        {
            if (controller.Interaction.Pickup != null && //there is a pickup
                controller.Interaction.Pickup as KeyPickup != null && //it is a key
                ((KeyPickup)controller.Interaction.Pickup).KeyName == KeyName) //keys match
            {
                Debug.Log($"Opening {gameObject.name} with {KeyName}");
            }
            else
            { return; }
        }
        parent.open = !parent.open;
    }
}

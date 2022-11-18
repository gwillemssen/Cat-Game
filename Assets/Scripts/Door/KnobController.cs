using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnobController : Interactable
{
    public bool Locked = false;
    public string KeyName = "TestKey";
    public int NoiseAmt = 20;

    private float lastTime;

    public float Cooldown;

    private DoorAnimController parent;
   
    private void Start()
    {
        parent = transform.parent.parent.GetComponent<DoorAnimController>();
       
    }

    public override void InteractWith(FirstPersonController controller, Interactable withInteractable)
    {
        if(withInteractable as KeyPickup != null && ((KeyPickup)controller.Interaction.Pickup).KeyName == KeyName)
        {
            Debug.Log($"Opening {gameObject.name} with {KeyName}");
            Locked = false;
        }
    }

    public override void Interact(FirstPersonController controller)
    {
        if(Locked)
        {
            FirstPersonController.instance.UI.SetInfoText("It's Locked! There might be a key somewhere...");
            return;
        }
        
        parent.open = !parent.open;
        parent.Open(parent.open);
        LevelManager.instance.MakeNoise(transform.position, NoiseAmt);
        
    }
    public void Open()
    {
        parent.open = true;
        parent.Open(parent.open);
        //for enemies to open it
    }

    public void Close()
    {
        parent.open = false;
        parent.Open(parent.open);
        //for enemies to close it
    }

    public bool IsOpen()
    { return parent.open; }
}

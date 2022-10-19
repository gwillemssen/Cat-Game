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
        { return; }
        parent.open = !parent.open;
    }
}

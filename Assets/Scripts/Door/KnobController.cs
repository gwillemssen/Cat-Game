using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnobController : Interactable
{
    private DoorAnimController parent;
   
    private void Start()
    {
        parent = transform.parent.parent.GetComponent<DoorAnimController>();
       
    }

    public override void InteractClick(FirstPersonController controller)
    {
        parent.open = !parent.open;
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class FuseBox : GenericHinge
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Interact(FirstPersonController controller)
    {
        OpenHinge(Hinge, DefaultPos, TransformPos, OpeningDuration, ClosingDuration, OpenSounds, ClosedSounds);
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class FuseBox : Interactable
{
    public GameObject Hinge;
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

        base.OpenHinge(Hinge, new Vector3(0, 60, 0), new Vector3(0, -90, 0), 1f, 1f); 
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorAnimController : Interactable
{
    public bool _open;

    private Collider doorCollider;

    private float openRot = -90;
    private float closedRot = 0;

    private GameObject doorHinge;
    
    // Start is called before the first frame update
    void Start()
    {
        doorHinge = transform.parent.gameObject;
        doorCollider = GetComponent<Collider>();
    }

    private void FixedUpdate()
    {
        if (doorHinge.transform.localRotation.y == openRot || doorHinge.transform.localRotation.y == closedRot)
        {
            if (!doorCollider.enabled)
            {
                doorCollider.enabled = true;
            }
        }
        else
        {
            if (doorCollider.enabled)
            {
                doorCollider.enabled = false;
            }
        }
    }

    public void OpenDoor(bool open)
    {
        if (open)
        {
            doorHinge.LeanRotateY(openRot,0.2f);
        }
        else
        {
            doorHinge.LeanRotateY(closedRot,0.2f);
        }

        _open = open;
    }

    private void Update()
    {
        
    }

    public override void Interact(FirstPersonController controller)
    {
        _open = !_open;
        OpenDoor(_open);
    }

    public bool IsOpen()
    {
        return _open;
    }
}

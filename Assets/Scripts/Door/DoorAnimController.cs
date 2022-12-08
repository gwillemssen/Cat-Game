using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DoorAnimController : Interactable
{
    public bool _open;
    private bool storedOpen;

    private Collider doorCollider;

    private float openRot = -90;
    private float closedRot = 0;

    public float openTime = 1.5f;
    public float closeTime = 0.5f;

    private GameObject doorHinge;
    
    // Start is called before the first frame update
    void Start()
    {
        doorHinge = transform.parent.gameObject;
        doorCollider = GetComponent<Collider>();
    }

    private void FixedUpdate()
    {
        if (_open != storedOpen)
        {
            OpenDoor(_open);
        }

        doorCollider.enabled = !LeanTween.isTweening(doorHinge);
    }
    
    

    public void OpenDoor(bool open)
    {
        LeanTween.cancel(doorHinge);
        int dude = Random.Range(0, 1001);
        //LeanTween.cancel(doorHinge);
        if (open)
        {
            if (dude < 1000)
            {
                LeanTween.rotateLocal(doorHinge.gameObject,new Vector3(0,openRot,0),openTime).setRotateLocal().setEaseOutElastic();
            }
            else
            {
                LeanTween.rotateLocal(doorHinge.gameObject,new Vector3(openRot,0,0),openTime).setRotateLocal().setEaseOutElastic();
            }
        }
        else
        {
            if (dude < 1000)
            {
                LeanTween.rotateLocal(doorHinge.gameObject,new Vector3(0,closedRot,0),closeTime).setRotateLocal().setEaseOutBounce();
            }
            else
            {
                LeanTween.rotateLocal(doorHinge.gameObject,new Vector3(closedRot,0,0),closeTime).setRotateLocal().setEaseOutBounce();
            }
        }

        _open = open;
        storedOpen = _open;
    }

    public override void Interact(FirstPersonController controller)
    {
        _open = !_open;
    }

    public bool IsOpen()
    {
        return _open;
    }
}

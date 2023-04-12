using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : Interactable
{
    [SerializeField] private string keyName = "Car Keys";
    bool Open = false;
    bool locked = true;

    public GameObject Hinge;

    // Start is called before the first frame update
    void Start()
    {
        base.VisiblyLocked = locked;
    }

    public override void InteractWith(FirstPersonController controller, Interactable withInteractable)
    {
        if (locked)
        {
            if (withInteractable.name == keyName)
            {
                locked = false;
                base.VisiblyLocked = false;
            }
            else
            {
                base.VisiblyLocked = true;
            }
        }
    }

    public override void Interact(FirstPersonController controller)
    {
        if (!locked)
        { MoveDoor(); }
    }

    private void MoveDoor()
    {
        if (!Open) //OPEN
        {
            Open = true;
            LeanTween.rotate(Hinge, new Vector3(0,110,0), 1.5f).setEaseInOutExpo();
        }
        else if (Open) //CLOSED
        {
            LeanTween.rotate(Hinge, new Vector3(0, 169.12f, 0), .8f).setEaseInOutExpo();
            Open = false;
        }
    }
   
}

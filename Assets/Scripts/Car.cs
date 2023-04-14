using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : Interactable
{
    [SerializeField] private string keyName = "Car Keys";
    bool locked = true;

    

    public List<Sound> UnlockSounds;

    public GameObject Hinge;

    // Start is called before the first frame update
    void Start()
    {
        //I dont want this bc i want the play to try and open the door first
        //base.VisiblyLocked = locked;
    }

    public override void InteractWith(FirstPersonController controller, Interactable withInteractable)
    {
        if (locked)
        {
            if (withInteractable.name == keyName)
            {
                locked = false;
                base.PlayRandomInteractionSound(UnlockSounds);
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
        { base.OpenHinge(Hinge, new Vector3(0, 110, 0), new Vector3(0, 169.12f, 0), 1.5f, .8f); }
    }


   
}

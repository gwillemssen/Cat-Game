using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : Interactable
{

    bool Open = false;
    bool locked = true;

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
        if (locked)
        {
            if (controller.Interaction.Pickup != null && controller.Interaction.Pickup.name == "Car Keys")
            {
                locked = false;
                controller.Interaction.VisiblyLocked = false;
            }
            else
            {
                controller.Interaction.VisiblyLocked = true;
                
            }
        }
        if (!locked) MoveDoor();
        
        
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

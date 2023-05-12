using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Lighter : InteractablePickup
{

    private bool Lit;
    // Start is called before the first frame update
    void Start()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }


    public override void Interact(FirstPersonController controller)
    {
        transform.GetChild(0).gameObject.SetActive(true);
    }

    //public override void Click()
    //{
    //    print("Click!");
    //    if (Lit == false)
    //    {
    //        Lit = true;
    //        transform.GetChild(0).gameObject.SetActive(true);
    //        //PlayInteractionSound(Put a lighter lighting sound here);
    //        Debug.Log("lit");
    //    }
    //    else 
    //    {
    //        Lit = false;
    //        transform.GetChild(0).gameObject.SetActive(false);
    //        //PlayInteractionSound(Lighter De-Lighting Sound);
    //        Debug.Log("Unlit");
    //    }
    //}
}


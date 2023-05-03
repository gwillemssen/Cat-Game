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

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {

        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Click Registered!");
            if (Lit == false)
            {
                Lit = true;
                transform.GetChild(0).gameObject.SetActive(true);
                //PlayInteractionSound(Put a lighter lighting sound here);
                Debug.Log("lit");
            }

        }
        //if (Lit == true)
        //{
        //    Lit = false;
        //    transform.GetChild(0).gameObject.SetActive(false);
        //    //PlayInteractionSound(Lighter De-Lighting Sound);
        //    Debug.Log("Unlit");
        //}
    }

}


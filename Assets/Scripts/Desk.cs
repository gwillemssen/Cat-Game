using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Desk : Interactable
{
    public GameObject Hinge;
    public GameObject objectInsideDrawer;
    private Vector3 inDrawerPosition;
    private bool Touched = false;
    // Start is called before the first frame update
    void Start()
    {
        inDrawerPosition= transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        
    }

    public override void Interact(FirstPersonController controller)
    {
       
        if(!base.Open)
        { LeanTween.moveLocalZ(Hinge, 3, 1f).setEaseInOutExpo(); base.Open = true; }
        else
        { LeanTween.moveLocalZ(Hinge, .3f, 1f).setEaseInOutExpo(); base.Open = false; }


        if (objectInsideDrawer.GetComponent<Rigidbody>().constraints != RigidbodyConstraints.FreezeAll)
        {
           DisconnectChild();
        }

    }

    private void DisconnectChild()
    {
        Debug.Log("EADFAFASF");
        objectInsideDrawer.transform.parent = null; 
    }


}

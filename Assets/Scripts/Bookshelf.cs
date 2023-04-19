using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bookshelf : Interactable
{
    public GameObject BookshelfL;
    public GameObject BookshelfR;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public override void InteractWith(FirstPersonController controller, Interactable withInteractable)
    {


        if (withInteractable.name == RequiredItem) 
        {
            LeanTween.moveLocalZ(BookshelfL, 2.6f, 2.5f).setEaseLinear();
            LeanTween.moveLocalZ(BookshelfR, -7f, 2.5f).setEaseLinear();

        }


    }

}

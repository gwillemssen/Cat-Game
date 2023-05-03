using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bookshelf : Interactable
{
    public GameObject BookshelfL, BookshelfR, CandlesParent;

    void Start()
    {
        CandlesParent.SetActive(false);
    }

    public override void InteractWith(FirstPersonController controller, Interactable withInteractable)
    {
        if (withInteractable.name == RequiredItemToViewInteraction) 
        {
            LeanTween.moveLocalZ(BookshelfL, 2.5f, 2.5f).setEaseLinear();
            LeanTween.moveLocalZ(BookshelfR, -7f, 2.5f).setEaseLinear();
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(true);
            CandlesParent.SetActive(true);
            controller.Interaction.DestroyPickup();
            GameManager.instance.FoundSecretRoom = true;
        }
    }

}

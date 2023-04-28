using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitDoor : Interactable
{
    

    //events
    //public static event Action ExitedDoor;

    private Animation anim;
    private bool locked = true;

    private void Start()
    {
        GameManager.instance.OnCatPetted += Unlock;
        anim = GetComponent<Animation>();
    }
    private void OnDestroy()
    {
        GameManager.instance.OnCatPetted -= Unlock;
    }
    public override void Interact(FirstPersonController controller)
    {
        if(!locked)
        {
            GameManager.instance.WinGame();
        }
        else if(!flashingText)
        {
            //Possibly add an audio clip to let the player know they can't leave yet
            //cant leave yet
            flashingText = true;
            StartCoroutine(FlashText());
        }
    }
    private bool flashingText;
    IEnumerator FlashText()
    {
        FirstPersonController.instance.UI.SetInfoText("MUST", 300, false);
        yield return new WaitForSeconds(0.5f);
        FirstPersonController.instance.UI.SetInfoText("PET", 300, false);
        yield return new WaitForSeconds(0.5f);
        FirstPersonController.instance.UI.SetInfoText("CATS", 300, false);
        yield return new WaitForSeconds(0.5f);
        FirstPersonController.instance.UI.SetInfoText("FIRST", 300, false);
        yield return new WaitForSeconds(0.5f);
        FirstPersonController.instance.UI.SetInfoText("", 300, false);
        flashingText = false;
    }

    private void Unlock()
    {
        locked = false;
        anim.Play();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitDoor : Interactable
{
    //events
    public static event Action ExitedDoor;

    private bool locked = true;

    private void Start()
    {
        LevelManager.AllCatsPetted += Unlock;
    }
    private void OnDestroy()
    {
        LevelManager.AllCatsPetted -= Unlock;
    }
    public override void Interact(FirstPersonController controller)
    {
        if(!locked)
        {
            GameManager.instance.WinGame();
        }
        else
        {
            //Possibly add an audio clip to let the player know they can't leave yet
            //cant leave yet
            StartCoroutine(FlashText());
        }
    }

    IEnumerator FlashText()
    {
        FirstPersonController.instance.UI.SetInfoText("MUST", 300, false);
        yield return new WaitForSeconds(0.5f);
        FirstPersonController.instance.UI.SetInfoText("PET", 300, false);
        yield return new WaitForSeconds(0.5f);
        FirstPersonController.instance.UI.SetInfoText("CATS", 300, false);
        yield return new WaitForSeconds(0.5f);
        FirstPersonController.instance.UI.SetInfoText("FIRST", 300, false);

    }

    private void Unlock()
    {
        locked = false;
    }
}

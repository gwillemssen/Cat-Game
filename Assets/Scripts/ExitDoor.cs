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
            FirstPersonController.instance.UI.SetInfoText("MUST. PET. CATS. FIRST");
        }
    }

    private void Unlock()
    {
        locked = false;
    }
}

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
    public override void InteractClick(FirstPersonController controller)
    {
        if(!locked)
        {
            ExitedDoor?.Invoke();
        }
        else
        {
            //cant leave yet
        }
    }

    private void Unlock()
    {
        locked = false;
    }
}

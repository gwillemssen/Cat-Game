using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitDoor : Interactable
{
    //events
    public static event Action OnExitedDoor;

    private bool locked = true;

    private void Start()
    {
        LevelManager.OnAllCatsPetted += Unlock;
    }
    public override void InteractClick(FirstPersonController controller)
    {
        if(!locked)
        {
            OnExitedDoor?.Invoke();
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

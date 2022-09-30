using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitDoor : Interactable
{
    public override void InteractClick(FirstPersonController controller)
    {
        if(LevelManager.instance.AllCatsPetted())
        {
            //open door
            gameObject.SetActive(false);
        }
        else
        {
            //cant leave yet
        }
    }
}

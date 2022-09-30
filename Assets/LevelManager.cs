using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    public int CatsToPet = 1;

    private int catsPetted;


    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        else
        {
            instance = this;
        }
    }

    public void StartGame()
    {
        
    }

    public bool AllCatsPetted()
    {
        return (catsPetted == CatsToPet);
    }

    public void CatPetted()
    {
        catsPetted++;
    }
}

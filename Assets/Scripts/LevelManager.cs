using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public enum LevelState { Normal, CopsCalled }


    public int CatsToPet = 1;
    public float CalledCopsTime = 30f;
    public int CatsPetted { get; private set; }
    public LevelState State { get; private set; } = LevelState.Normal;

    //events
    public static event Action AllCatsPetted;

    private float lastTimeCalledCops = -420f;
    private float lastTimeNoise = -420f;
    private float timeRemaining;


    private void Awake()
    {
        if(instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        Cat.CompletedPetting += OnCompletedPettingCat;
        State = LevelState.Normal;
        StartGame();
    }

    private void OnDestroy()
    {
        Cat.CompletedPetting -= OnCompletedPettingCat;
    }

    private void Update()
    {
        if(State == LevelState.CopsCalled)
        {
            timeRemaining = CalledCopsTime - (Time.time - lastTimeCalledCops);
            PlayerUI.instance.SetTimeUI(timeRemaining / CalledCopsTime);
            if(timeRemaining <= 0)
            { GameManager.instance.GameOver(GameManager.LoseState.Arrested); }
        }       
    }

    public void StartGame()
    {
        CatsPetted = 0;
    }

    private void OnCompletedPettingCat()
    {
        CatsPetted++;
        if(CatsPetted == CatsToPet)
        {
            PlayerUI.instance.SetInfoText("All cats petted!\nTime to escape!");
            AllCatsPetted?.Invoke();
        }
    }
}

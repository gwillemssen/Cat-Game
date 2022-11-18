using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public enum LevelState { Normal, CopsCalled }

    public float MaxNoise = 100f;
    public float NoiseDecayRate = 1f;
    public float NoiseDecayDelay = 3f;
    public int CatsToPet = 1;
    public float CalledCopsTime = 30f;
    public float Noise { get; private set; }
    public int CatsPetted { get; private set; }
    public LevelState State { get; private set; } = LevelState.Normal;

    //events
    public static event Action AllCatsPetted;

    private List<Enemy> enemies;
    [HideInInspector]
    public Enemy.EnemyState MostAlertEnemyState = Enemy.EnemyState.Patrolling; //for the eyeball ui

    private float lastTimeCalledCops = -420f;
    private float lastTimeNoise = -420f;
    private float timeRemaining;


    private void Awake()
    {
        instance = this;

        enemies = new List<Enemy>();
        
        Cat.CompletedPetting += OnCompletedPettingCat;
        Enemy.EnemyChangedState += OnEnemyChangedState;
        Enemy.EnemySpawned += OnEnemySpawned;

        State = LevelState.Normal;

        StartGame();
    }
    private void OnDestroy()
    {
        Cat.CompletedPetting -= OnCompletedPettingCat;
        Enemy.EnemyChangedState -= OnEnemyChangedState;
        Enemy.EnemySpawned -= OnEnemySpawned;
    }
    private void OnEnemySpawned(Enemy e)
    {
        enemies.Add(e);
    }

    private void Update()
    {
        if(Time.time - lastTimeNoise > NoiseDecayDelay)
        {
            Noise -= Time.deltaTime * NoiseDecayRate;
        }
        Noise = Mathf.Clamp(Noise, 0f, MaxNoise);

        if(State == LevelState.CopsCalled)
        {
            timeRemaining = CalledCopsTime - (Time.time - lastTimeCalledCops);
            PlayerUI.instance.SetTimeUI(timeRemaining / CalledCopsTime);
            if(timeRemaining <= 0)
            { GameManager.instance.GameOver(GameManager.LoseState.Arrested); }
        }
    }

    public void MakeNoise(Vector3 pos, float noiseAmt) //add Vector3 LastNoise so the enemy AI investigates it
    {
        lastTimeNoise = Time.time;
        PlayerUI.instance.ShowSpeaker();

        foreach (Enemy e in enemies)
        {
            Noise += e.OnMadeNoise(pos, noiseAmt);

            if (Noise >= MaxNoise)
            { e.OnMaxNoise(); }
        }
    }

    public void MaxOutNoise(Vector3 pos)
    {
        MakeNoise(pos, float.MaxValue);
    }

    private void OnEnemyChangedState(Enemy.EnemyState state)
    {
        int highest = (int)state;

        foreach(Enemy e in enemies)
        {
            if((int)e.State > highest)
            {
                highest = (int)e.State;
            }
        }

        MostAlertEnemyState = (Enemy.EnemyState)highest;
    }
    public void CallCops()
    {
        if(State == LevelState.CopsCalled)
        { return; }
        lastTimeCalledCops = Time.time;
        State = LevelState.CopsCalled;
    }

    public void StartGame()
    {
        CatsPetted = 0;
        Noise = 0;
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

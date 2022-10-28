using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public float MaxNoise = 100f;
    public float NoiseDecayRate = 1f;
    public float NoiseDecayDelay = 3f;
    public int CatsToPet = 1;
    public float Noise { get; private set; }
    public int CatsPetted { get; private set; }

    //events
    public static event Action AllCatsPetted;

    private List<Enemy> enemies;
    [HideInInspector]
    public Enemy.EnemyState MostAlertEnemyState = Enemy.EnemyState.Patrolling; //for the eyeball ui

    
    private float lastTimeNoise = -420f;


    private void Awake()
    {
        instance = this;

        enemies = new List<Enemy>();
        
        Cat.CompletedPetting += OnCompletedPettingCat;
        Enemy.EnemyChangedState += OnEnemyChangedState;
        Enemy.EnemySpawned += OnEnemySpawned;

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
    }

    public void MakeNoise(Vector3 pos, float noiseAmt) //add Vector3 LastNoise so the enemy AI investigates it
    {
        lastTimeNoise = Time.time;

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
            FirstPersonController.instance.UI.SetInfoText("All cats petted!\nTime to escape!");
            AllCatsPetted?.Invoke();
        }
    }
}

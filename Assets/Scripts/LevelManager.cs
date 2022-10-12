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
    public static event Action OnAllCatsPetted;

    private List<Enemy> enemies;
    [HideInInspector]
    public Enemy.EnemyState MostAlertEnemyState = Enemy.EnemyState.Idle; //for the eyeball ui

    
    private float lastTimeNoise = -420f;


    private void Awake()
    {
        if(instance != null)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }

        Cat.OnCompletedPetting += CatPetted;
        Enemy.OnEnemyChangedState += EnemyChangedState;
        Enemy.OnEnemySpawn += RegisterEnemy;

        enemies = new List<Enemy>();

        StartGame();
    }
    private void RegisterEnemy(Enemy e)
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

    public void MakeNoise(float noiseAmt)
    {
        Noise += noiseAmt;
        lastTimeNoise = Time.time;

        if(Noise >= MaxNoise)
        {
            foreach(Enemy e in enemies)
            {
                e.GoAggro();
            }
        }
    }

    public void EnemyChangedState(Enemy.EnemyState state)
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

    public void CatPetted()
    {
        CatsPetted++;
        if(CatsPetted == CatsToPet)
        {
            OnAllCatsPetted?.Invoke();
        }
    }
}

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

    [HideInInspector]
    public List<Enemy> Enemies;

    //IAlertable
    //subscribes to distraction events and also when alertness is maxed out

    private int catsPetted;
    private float lastTimeNoise = -420f;


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

        StartGame();
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
            foreach(Enemy e in Enemies)
            {
                e.GoAggro();
            }
        }
    }

    public void StartGame()
    {
        catsPetted = 0;
        Noise = 0;
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

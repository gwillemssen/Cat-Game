using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
using UnityEditor;
using System;

public class EnemyState
{
    public static EnemyState SittingState = new SittingState();
    public static EnemyState PatrollingState = new PatrollingState();
    public static EnemyState SearchingForNoiseState = new SearchingForNoiseState();
    public static EnemyState CallingCopsGrabbingGunState = new CallingCopsGrabbingGunState();
    public static EnemyState PatrollingWithGunState = new PatrollingWithGunState();

    public virtual void Update(Enemy enemy, IAstarAI ai) { }
    public virtual void OnNoise(Enemy enemy, Vector3 noisePos) { }
}

public class SittingState : EnemyState
{
    public override void Update(Enemy enemy, IAstarAI ai)
    {
        //Movement
        ai.isStopped = true;
        ai.destination = enemy.transform.position;

        //Spotting
        if(enemy.SpottedPlayer)
        { enemy.SetState(PatrollingState); }
    }

    public override void OnNoise(Enemy enemy, Vector3 noisePos)
    { enemy.SetState(SearchingForNoiseState); }
}

public class PatrollingState : EnemyState
{
    private int currentWaypointIndex;

    public override void Update(Enemy enemy, IAstarAI ai)
    {
        //Movement
        ai.isStopped = false;
        ai.destination = enemy.PatrollingRoute[currentWaypointIndex].transform.position;

        //Spotting
        if (enemy.SpottedPlayer)
        { enemy.SetState(PatrollingState); }
    }

    public override void OnNoise(Enemy enemy, Vector3 noisePos)
    { enemy.SetState(SearchingForNoiseState); }
}

public class PatrollingWithGunState : PatrollingState
{
    public override void Update(Enemy enemy, IAstarAI ai)
    {
        base.Update(enemy, ai);
        //gun shooty stuff
    }

    public override void OnNoise(Enemy enemy, Vector3 noisePos)
    { /*do nothing*/ }
}

public class SearchingForNoiseState : EnemyState
{
    public Vector3 NoisePosition;

    public override void Update(Enemy enemy, IAstarAI ai)
    {
        //Movement
        ai.isStopped = false;
        ai.destination = NoisePosition;

        //Spotting
        if(enemy.SpottedPlayer)
        { enemy.SetState(CallingCopsGrabbingGunState); }
    }

    public override void OnNoise(Enemy enemy, Vector3 noisePos)
    { NoisePosition = noisePos; }
}

public class CallingCopsGrabbingGunState : EnemyState
{
    enum State { GoingToPhone, GrabbingGun }
    State state = State.GoingToPhone;

    public override void Update(Enemy enemy, IAstarAI ai)
    {
        //Movement
        ai.isStopped = false;
        switch(state)
        {
            case State.GoingToPhone:
                ai.destination = enemy.PhoneWaypoint.transform.position;
                break;
            case State.GrabbingGun:
                ai.destination = enemy.GunWaypoint.transform.position;

                if(enemy.ArrivedAtDestination)
                { enemy.SetState(PatrollingWithGunState); }

                break;
        }
    }
}

[RequireComponent(typeof(IAstarAI))]
[RequireComponent(typeof(AudioPlayer))]
public class Enemy : MonoBehaviour
{
    public bool DebugMode;
    public List<Waypoint> PatrollingRoute { get; private set; }
    public Waypoint PhoneWaypoint;
    public Waypoint GunWaypoint;
    [HideInInspector] public EnemyState State { get; private set; }
    [HideInInspector] public EnemyState LastState { get; private set; }
    public bool SeesPlayer { get; private set; }
    public bool SpottedPlayer { get; private set; }
    public bool ArrivedAtDestination { get; private set; }

    private IAstarAI ai;

    private void Awake()
    {
        ai = GetComponent<IAstarAI>();
        LevelManager.OnNoise += OnNoiseCallback;
        Cat.CompletedPetting += CompletedPettingCallback;
    }

    private void OnDestroy()
    {
        LevelManager.OnNoise -= OnNoiseCallback;
        Cat.CompletedPetting -= CompletedPettingCallback;
    }

    private void Update()
    {
        return;
        //TODO: calculate stuff before we update the state
        //stuff
        //SeesPlayer
        //SpottedPlayer
        //ArrivedAtDestination

        State.Update(this, ai);
    }

    private void OnNoiseCallback(Vector3 pos)
    { return; State.OnNoise(this, pos);  }

    private void CompletedPettingCallback()
    {
        if (State == EnemyState.SittingState)
        { SetState(EnemyState.PatrollingState); }
    }

    public void SetState(EnemyState newState)
    {
        if(State == newState)
        { Debug.LogWarning("Setting the same state twice"); return; }
        LastState = State;
        State = newState;
    }

    //Editor Gizmo stuff
    private void OnDrawGizmosSelected()
    {
        if (PatrollingRoute == null || PatrollingRoute.Count == 0)
        { return; }
        for (int i = 0; i < PatrollingRoute.Count; i++)
        {
            Waypoint s = PatrollingRoute[i];
            Waypoint e = PatrollingRoute[Mathf.Clamp(i + 1, 0, PatrollingRoute.Count - 1)];
            Gizmos.color = Color.Lerp(Color.green, Color.red, ((float)i / PatrollingRoute.Count));
            Gizmos.DrawLine(s.transform.position, e.transform.position);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(s.transform.position, 0.5f);
        }
    }
}
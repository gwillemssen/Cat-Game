using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
using UnityEditor;
using System;

[RequireComponent(typeof(IAstarAI))]
[RequireComponent(typeof(AudioPlayer))]
public class Enemy : MonoBehaviour
{
    public enum EnemyState { Patrolling, SearchingForNoise, Chasing, CallingCops, GrabbingGun, PatrollingWithGun }

    [Header("References")]
    public List<Waypoint> PatrollingRoute;
    public Waypoint PhoneWaypoint;
    public Waypoint GunWaypoint;
    public Transform eyes;

    [Header("Alertness")]
    [Range(45f, 180f)]
    public float FieldOfView = 90f;
    public float SightDistance = 12f;
    public float HearingRadius = 20f;
    [Tooltip("Higher value = more time / noise is neeeded to alert the enemy")]
    [Range(0f, 8f)]
    public float AlertnessRequired = 2f;

    [Header("Chasing")]
    [Range(1f, 10f)]
    public float ChaseTime = 6f;
    [Tooltip("How long after losing the target does it take until the enemy goes back into idle")]
    [Range(5f, 25f)]
    public float LostTargetTime = 10f;
    public float ShootDistance = 5f;

    [Header("Misc")]
    public LayerMask EverythingExceptEnemy;
    public LayerMask InteractableLayerMask;
    public bool DebugMode = false;
    public TextMesh EnemyDebugObject;

    [Header("Sound")]
    AudioPlayer audioPlayer;
    public float StepCooldownWalk = .5f;
    public float StepCooldownChase = .25f;

    //events
    public static event Action<Enemy.EnemyState> EnemyChangedState;
    public static event Action<Enemy> EnemySpawned;

    public EnemyState State
    {
        get
        {
            return state;
        }
        private set
        {
            if (state != value)
            {
                state = value;
                EnemyChangedState?.Invoke(state);
            }
        }
    }
    public bool InFOV { get; private set; }
    public RaycastHit Hit { get; private set; }
    public float Alertness { get; private set; }

    private IAstarAI ai;
    private Transform target;
    private EnemyState state;
    private float lastTimeSighted = -420f;
    private float lastTimeLostTarget = -420f;
    private float fov;
    private RaycastHit hit;
    private float sqrHearingRadius;
    private float localNoise;
    private Vector3 lastNoisePosition;
    private float stepCooldown;
    private float lastTimeStep;
    private bool isPlaying;
    private int waypointIndex;
    private float lastTimeAtWaypoint = -420f;
    private bool atWaypoint;
    private float timeCalledCops = -420f;
    private float sqrShootDistance;

    void Start()
    {
        ai = GetComponent<IAstarAI>();
        audioPlayer = GetComponent<AudioPlayer>();
        target = FirstPersonController.instance.MainCamera.transform;
        State = EnemyState.Patrolling;

        EnemyDebugObject.gameObject.SetActive(DebugMode);
        fov = Mathf.InverseLerp(180, 0, FieldOfView);
        sqrHearingRadius = HearingRadius * HearingRadius;
        sqrShootDistance = ShootDistance * ShootDistance;

        if (PatrollingRoute == null || PatrollingRoute.Count == 0)
        {
            Waypoint g = new GameObject().AddComponent<Waypoint>();
            g.transform.position = transform.position;
            g.transform.rotation = transform.rotation;
            PatrollingRoute.Add(g);
        }

        EnemySpawned?.Invoke(this);

    }

    void Update()
    {
        if (FirstPersonController.instance.Hiding && IsPlayerWithinFieldOfView() && RaycastToPlayer()) 
        { FirstPersonController.instance.Hiding = false; } //CAUGHT while going into a hiding spot

        if (IsPlayerWithinFieldOfView() && RaycastToPlayer() && !FirstPersonController.instance.Hiding)
        { IncreaseAlertness(); }
        else
        { DecreaseAlertness(); }

        if(state == EnemyState.Chasing)
        {
            if(Vector3.SqrMagnitude(transform.position - target.position) <= sqrShootDistance)
            {
                GameManager.instance.GameOver();
            }
        }

        Move();

        FootstepAudio();
    }

    public void OnMaxNoise()
    {
        State = EnemyState.SearchingForNoise;
        lastTimeLostTarget = Time.time;
    }

    public float OnMadeNoise(Vector3 pos, float amt)
    {
        //returns how much noise it made to the enemy
        localNoise = Vector3.Distance(pos, transform.position) / HearingRadius;
        localNoise = Mathf.Lerp(1, 0, localNoise);
        localNoise = 1f - Mathf.Pow(1f - localNoise, 5f); //easeOutQuint
        localNoise *= amt;
        lastNoisePosition = pos;

        return localNoise;
    }

    void SpotPlayer()
    {
        lastTimeSighted = Time.time;
        if (State == EnemyState.PatrollingWithGun)
        { State = EnemyState.Chasing; }
        else
        { State = EnemyState.CallingCops; }
    }

    void DecreaseAlertness()
    {
        Alertness = Mathf.Clamp(Alertness - Time.deltaTime, 0f, AlertnessRequired);
    }

    void IncreaseAlertness()
    {
        Alertness = Mathf.Clamp(Alertness + Time.deltaTime, 0f, AlertnessRequired);

        if (Alertness >= AlertnessRequired)
        {
            SpotPlayer();
            Alertness = AlertnessRequired;
        }
    }
    void FootstepAudio()
    {
        if (Time.time > lastTimeStep + stepCooldown)
        {
            lastTimeStep = Time.time;
            audioPlayer.Play("footstep");
        }
    }

    void Move()
    {
        switch (State)
        {
            case EnemyState.Chasing:
                ai.destination = target.position;
                lastNoisePosition = target.position;
                stepCooldown = StepCooldownChase;
                break;

            case EnemyState.SearchingForNoise:
                ai.destination = lastNoisePosition;
                if (Vector3.SqrMagnitude(transform.position - lastNoisePosition) <= 1f)
                { State = EnemyState.Patrolling; } //investigated the noise, going back to normal
                break;

            case EnemyState.Patrolling:
            case EnemyState.PatrollingWithGun:
                stepCooldown = StepCooldownWalk;
                if (NavigateToWaypoint(PatrollingRoute[waypointIndex]))
                {
                    waypointIndex++;
                    if (waypointIndex >= PatrollingRoute.Count)
                    { waypointIndex = 0; }
                }
                break;

            case EnemyState.CallingCops:
                if (NavigateToWaypoint(PhoneWaypoint))
                {
                    timeCalledCops = Time.time;
                    State = EnemyState.GrabbingGun;
                }
                break;

            case EnemyState.GrabbingGun:
                if(NavigateToWaypoint(GunWaypoint))
                { state = EnemyState.PatrollingWithGun; }
                break;
        }


        //raycast for doors
        RaycastHit hit;
        Interactable interactable;
        if (Physics.Raycast(eyes.position, eyes.forward, out hit, 2f, InteractableLayerMask, QueryTriggerInteraction.Collide))
        {
            interactable = hit.collider.GetComponent<Interactable>();
            if (interactable != null && interactable is KnobController)
            {
                (interactable as KnobController).Open();
            }
        }

    }

    bool NavigateToWaypoint(Waypoint waypoint)
    {
        ai.destination = waypoint.transform.position;
        if (Vector3.SqrMagnitude(transform.position - waypoint.transform.position) < 0.5f)
        {
            if (!atWaypoint)
            {
                lastTimeAtWaypoint = Time.time;
                atWaypoint = true;
            }
            if(Time.time > lastTimeAtWaypoint + PatrollingRoute[waypointIndex].StopTime)
            {
                atWaypoint = false;
                return true;
            }
        }
        return false;
    }

    bool IsPlayerWithinFieldOfView()
    {
        InFOV = Vector3.Dot(transform.TransformDirection(Vector3.forward), (target.position - transform.position)) >= fov;
        return InFOV;
    }

    bool RaycastToPlayer()
    {
        if (Physics.Raycast(eyes.position, (target.position - eyes.position), out hit, SightDistance, EverythingExceptEnemy, QueryTriggerInteraction.Collide))
        {
            Hit = hit;

            return Hit.collider.CompareTag("Player");

        }
        else
        {
            return false;
        }
    }

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
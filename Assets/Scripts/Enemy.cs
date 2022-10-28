using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
using UnityEditor;
using System;

[RequireComponent(typeof(IAstarAI))]
public class Enemy : MonoBehaviour
{
    public enum EnemyState { Patrolling, Searching, Chasing }

    [Header("References")]
    [HideInInspector] //Hiding this for now, until the patrolling code is functioning
    public List<Waypoint> PatrollingRoute;
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

    [Header("Misc")]
    public LayerMask EverythingExceptEnemy;
    public LayerMask InteractableLayerMask;
    public bool DebugMode = false;
    public TextMesh EnemyDebugObject;

    //events
    public static event Action<Enemy.EnemyState> EnemyChangedState;
    public static event Action<Enemy> EnemySpawned;
    public static event Action CalledCops;

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
    public float stepCooldown = .5f;
    public float sprintCooldown = .25f;
    private float lastTimeStep;
    private bool isPlaying;
    private int waypointIndex;
    private float lastTimeAtWaypoint = -420f;
    private bool atWaypoint;

    void Start()
    {
        ai = GetComponent<IAstarAI>();
        target = FirstPersonController.instance.MainCamera.transform;
        State = EnemyState.Patrolling;

        EnemyDebugObject.gameObject.SetActive(DebugMode);
        fov = Mathf.InverseLerp(180, 0, FieldOfView);
        sqrHearingRadius = HearingRadius * HearingRadius;

        if (PatrollingRoute == null || PatrollingRoute.Count == 0)
        {
            GameObject g = new GameObject();
            g.transform.position = transform.position;
            g.transform.rotation = transform.rotation;
            PatrollingRoute.Add(g.transform);
        }

        EnemySpawned?.Invoke(this);

    }

    void Update()
    {
        if (FirstPersonController.instance.Hiding && IsPlayerWithinFieldOfView() && RaycastToPlayer())
        {
            FirstPersonController.instance.Hiding = false;
            Debug.LogError("deez");
        } //CAUGHT LACKIN

        if (IsPlayerWithinFieldOfView() && RaycastToPlayer() && !FirstPersonController.instance.Hiding)
        {
            IncreaseAlertness();
        }
        else
        {
            DecreaseAlertness();
        }

        Move();
    }

    public void OnMaxNoise()
    {
        State = EnemyState.Searching;
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
        if (State == EnemyState.Patrolling)
            State = EnemyState.Chasing;
    }

    void PlayFootstep()
    {
        AudioManager.instance.Play("EnemyFootsteps");
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
    void CheckAudio()
    {
        if (Time.time > lastTimeStep + stepCooldown)
        {
            lastTimeStep = Time.time;
            PlayFootstep();
        }
    }

    void Move()
    {
        if (State == EnemyState.Chasing)
        {

            ai.destination = target.position;
            lastNoisePosition = target.position;
            stepCooldown = sprintCooldown;

        }
        if (State == EnemyState.Searching)
        {
            ai.destination = lastNoisePosition;

            if (Vector3.SqrMagnitude(transform.position - lastNoisePosition) <= 1f)
            { State = EnemyState.Patrolling; } //investigated the noise, going back to normal
        }
        if (State == EnemyState.Patrolling)
        {
            sprintCooldown = stepCooldown;

            ai.destination = PatrollingRoute[waypointIndex].transform.position;
            if (Vector3.SqrMagnitude(transform.position - ai.destination) < 0.5f)
            {
                if (!atWaypoint)
                {
                    lastTimeAtWaypoint = Time.time;
                    atWaypoint = true;
                }

                if (Time.time > lastTimeAtWaypoint + PatrollingRoute[waypointIndex].StopTime)
                {
                    waypointIndex++;
                    if (waypointIndex >= PatrollingRoute.Count)
                    { waypointIndex = 0; }
                    atWaypoint = false;
                }
            }
        }


        CheckAudio();
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
}
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
using UnityEditor;
using System;

[RequireComponent(typeof(IAstarAI))]
public class Enemy : MonoBehaviour
{
    public enum EnemyState { Idle, Patrolling, LostTarget, Chasing, MaxNoise }

    [Header("References")]
    [HideInInspector] //Hiding this for now, until the patrolling code is functioning
    public List<Transform> PatrollingRoute;
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
    public float CaughtDistance = 1.25f;

    [Header("Misc")]
    public LayerMask EverythingExceptEnemy;
    public LayerMask InteractableLayerMask;
    public bool DebugMode = false;
    public TextMesh EnemyDebugObject;

    //events
    public static event Action<Enemy.EnemyState> EnemyChangedState;
    public static event Action<Enemy> EnemySpawned;
    public static event Action CaughtPlayer;

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
    private bool doPatrol = false;
    private float fov;
    private RaycastHit hit;
    private float sqrCaughtDistance;
    private float sqrHearingRadius;
    private float localNoise;
    private Vector3 lastNoisePosition;

    void Start()
    {
        ai = GetComponent<IAstarAI>();
        target = FirstPersonController.instance.MainCamera.transform;
        State = EnemyState.Idle;

        EnemyDebugObject.gameObject.SetActive(DebugMode);
        fov = Mathf.InverseLerp(180, 0, FieldOfView);
        sqrCaughtDistance = CaughtDistance * CaughtDistance;
        sqrHearingRadius = HearingRadius * HearingRadius;

        if(PatrollingRoute == null || PatrollingRoute.Count == 0)
        {
            GameObject g = new GameObject();
            g.transform.position = transform.position;
            g.transform.rotation = transform.rotation;
            PatrollingRoute.Add(g.transform);
        }
        if (PatrollingRoute != null)
        {
            if (PatrollingRoute.Count > 0)
            { doPatrol = true; }
        }

        EnemySpawned?.Invoke(this);

    }

    void Update()
    {
        if (State != EnemyState.MaxNoise && IsPlayerWithinFieldOfView())
        {
            if (RaycastToPlayer() && !FirstPersonController.instance.Hiding)
            {
                if (State == EnemyState.LostTarget)  //LostTarget -> Chasing is instant
                { SpotPlayer(); }
                else
                {
                    IncreaseAlertness();
                }
            }
            else if (State == EnemyState.Idle || State == EnemyState.Patrolling)
            { DecreaseAlertness(); }
        }
        else
        { DecreaseAlertness(); }

        if (State == EnemyState.Chasing && Time.time > lastTimeSighted + ChaseTime)
        {
            lastTimeLostTarget = Time.time;
            State = EnemyState.LostTarget; //enemy lost the player
        }
        if (State == EnemyState.LostTarget || State == EnemyState.MaxNoise && Time.time > lastTimeLostTarget + LostTargetTime)
        {
            State = EnemyState.Patrolling; //player got away, go back to normal
        }

        Move();

        if (Vector3.SqrMagnitude(transform.position - target.position) <= sqrCaughtDistance)
        {
            CaughtPlayer?.Invoke();
        }
    }

    public void OnMaxNoise()
    {
        State = EnemyState.MaxNoise;
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
        State = EnemyState.Chasing;
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

    void Move()
    {
        if (State == EnemyState.Chasing)
        {
            ai.destination = target.position;
            lastNoisePosition = target.position;
        }
        if (State == EnemyState.LostTarget || State == EnemyState.MaxNoise)
        {
            ai.destination = lastNoisePosition;

            if (Vector3.SqrMagnitude(transform.position - lastNoisePosition) <= 1f)
            { State = EnemyState.Patrolling; } //investigated the noise, going back to normal
        }
        if (State == EnemyState.Idle)
        {
            ai.destination = transform.position;
            //TEMPORARY SOLUTION
            transform.rotation = PatrollingRoute[0].rotation;
        }
        if (State == EnemyState.Patrolling)
        {
            //TEMPORARY SOLUTION
            ai.destination = PatrollingRoute[0].position;
            if (Vector3.SqrMagnitude(transform.position - ai.destination) < 0.5f)
            {
                State = EnemyState.Idle;
            }
        }
        if(State != EnemyState.Idle)
        {
            //raycast for doors
            RaycastHit hit;
            Interactable interactable;
            if (Physics.Raycast(eyes.position, eyes.forward, out hit, 2f, InteractableLayerMask, QueryTriggerInteraction.Collide))
            {
                interactable = hit.collider.GetComponent<Interactable>();
                if(interactable != null && interactable is KnobController)
                {
                    (interactable as KnobController).Open();
                }
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
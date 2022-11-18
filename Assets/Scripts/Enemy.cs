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
    public Animator anim;

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
    public float OpenDoorStopTime = 1f;
    public LayerMask EverythingExceptEnemy;
    public LayerMask InteractableLayerMask;
    public bool DebugMode = false;
    public TextMesh EnemyDebugObject;

    [Header("Sound")]
    AudioPlayer audioPlayer;
    public Sound[] FootstepSounds;
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
    public bool SeesPlayer { get; private set; }
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
    private float lastTimeOpenedDoor = -420f;
    private DoorAnimController lastDoor;
    private float sqrShootDistance;
    private float sqrMoveSpeed;
    private Vector2 pos2D;
    private Vector2 waypointPos2D;

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
        sqrMoveSpeed = Mathf.Pow((ai.maxSpeed / 2f), 2);

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

        if(state == EnemyState.Chasing && SeesPlayer)
        {
            if(Vector3.SqrMagnitude(transform.position - target.position) <= sqrShootDistance)
            {
                GameManager.instance.GameOver(GameManager.LoseState.Shot);
            }
        }

        Move();
        Interact();
        FootstepAudio();
        UpdateAnimationState();
    }

    public void UpdateAnimationState()
    {
        anim.SetBool("isWalking", ai.velocity.sqrMagnitude > sqrMoveSpeed);
        if(State == EnemyState.PatrollingWithGun)
        { anim.SetBool("isRifleRunning", ai.velocity.sqrMagnitude > sqrMoveSpeed); }
        //anim.SetBool("isWalking", true);
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
        else if(State == EnemyState.Patrolling)
        { State = EnemyState.CallingCops; }
    }

    void DecreaseAlertness()
    {
        SeesPlayer = false;
        Alertness = Mathf.Clamp(Alertness - Time.deltaTime, 0f, AlertnessRequired);
        EyeballUI();
    }

    void EyeballUI()
    {
        //WARNING
        //this will only work with one enemy
        float t = Alertness / AlertnessRequired;
        if(t > .66 || (state != EnemyState.Patrolling && state != EnemyState.SearchingForNoise))
        { PlayerUI.instance.SetEyeballUI(PlayerUI.EyeState.Open); }
        else if (t > .33)
        { PlayerUI.instance.SetEyeballUI(PlayerUI.EyeState.Half); }
        else
        { PlayerUI.instance.SetEyeballUI(PlayerUI.EyeState.Closed); }
    }

    void IncreaseAlertness()
    {
        SeesPlayer = true;
        Alertness = Mathf.Clamp(Alertness + (Time.deltaTime * (FirstPersonController.instance.IsCrouching ? 0.5f : 1f)), 0f, AlertnessRequired);

        if (Alertness >= AlertnessRequired)
        {
            SpotPlayer();
            Alertness = AlertnessRequired;
        }

        EyeballUI();
    }
    void FootstepAudio()
    {
        if(ai.velocity.sqrMagnitude < 0.05f)
        { return; }
        if (Time.time > lastTimeStep + stepCooldown)
        {
            lastTimeStep = Time.time;
           // audioPlayer.Play(FootstepSounds[UnityEngine.Random.Range(0, FootstepSounds.Length)]);
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
                    LevelManager.instance.CallCops();
                    State = EnemyState.GrabbingGun;
                }
                break;

            case EnemyState.GrabbingGun:
                if(NavigateToWaypoint(GunWaypoint))
                { state = EnemyState.PatrollingWithGun; }
                break;
        }

        if(Time.time - lastTimeOpenedDoor < OpenDoorStopTime)
        {
            ai.isStopped = true;
        }
        else
        {
            ai.isStopped = false;
            if(lastDoor != null && (Time.time - lastTimeOpenedDoor > OpenDoorStopTime + 1f))
            { lastDoor.OpenDoor(false); }
        }


    }

    private void Interact()
    {
        //raycast for doors
        RaycastHit hit;
        Interactable interactable;
        if (Physics.Raycast(eyes.position, eyes.forward, out hit, 2f, InteractableLayerMask, QueryTriggerInteraction.Collide))
        {
            interactable = hit.collider.GetComponent<Interactable>();
            if (interactable != null && interactable is DoorAnimController)
            {
                if(!(interactable as DoorAnimController).IsOpen())
                {
                    lastTimeOpenedDoor = Time.time;
                    lastDoor = (interactable as DoorAnimController);
                    lastDoor.OpenDoor(true);
                }
            }
        }
    }

    bool NavigateToWaypoint(Waypoint waypoint)
    {
        pos2D.x = transform.position.x;
        pos2D.y = transform.position.z;
        waypointPos2D.x = waypoint.transform.position.x;
        waypointPos2D.y = waypoint.transform.position.z;

        ai.destination = waypoint.transform.position;
        if (Vector2.SqrMagnitude(pos2D - waypointPos2D) < 0.5f)
        {
            if (!atWaypoint)
            {
                lastTimeAtWaypoint = Time.time;
                atWaypoint = true;
            }
            if (Time.time - lastTimeAtWaypoint > waypoint.StopTime)
            {
                atWaypoint = false;
                return true;
            }
        }
        else
        {
            atWaypoint = false;
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
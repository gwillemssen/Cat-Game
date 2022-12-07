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
    public static Enemy instance;

    public enum EnemyState { Patrolling, SearchingForNoise, Chasing, GoingToCallThePolice, GrabbingGun, PatrollingWithGun, Idle, CallingPolice }

    public bool DebugMode = false;

    [Header("References")]
    public List<Waypoint> PatrollingRoute;
    public Waypoint PhoneWaypoint;
    public Waypoint GunWaypoint;
    public Transform eyes;
    public Animator anim;

    [Header("Alertness")]
    [Range(45f, 180f)]
    public float FieldOfView = 90f;
    public float AlertnessRate = .75f;
    public float SightDistance = 12f;
    public float HearingRadius = 20f;

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
    public TextMesh EnemyDebugObject;
    [Tooltip("How long the enemy will be stuck until the game gets them unstuck")]
    public float StuckFixTime = 2f;

    [Header("Sound")]
    public float NoiseDecay = 4f;
    public float Noise { get; private set; }
    public float MaxNoise = 30f;
    AudioPlayer audioPlayer;
    public Sound[] FootstepSounds;
    public float StepCooldownWalk = .5f;
    public float StepCooldownChase = .25f;
    public Sound[] SpotPlayerSounds;
    public Sound[] GoingToCallThePoliceSounds;
    public Sound[] CallingPoliceSounds;
    public Sound[] ChasingSounds;


    public EnemyState State 
    {   
        get 
        { return state_dontUse; } 

        private set 
        {
            state_dontUse = value;
            if (lastState != value)
            { OnChangedState(); }
            lastState = state_dontUse;
        }
    }
    public bool SeesPlayer { get; private set; }
    public float Alertness { get; private set; } //increases as the player is within the sight of the enemy
    public bool InFOV { get; private set; }
    public RaycastHit Hit { get; private set; }

    private IAstarAI ai;
    private Transform target;
    private EnemyState state_dontUse; //use the State property, not this field
    private EnemyState lastState;
    private float lastTimeSighted = -420f;
    private float lastTimeLostTarget = -420f;
    private float fov;
    private RaycastHit hit;
    private float sqrHearingRadius;
    private float localNoise;
    private Vector3 lastLoudNoisePosition;
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
    private Waypoint noiseWaypoint;
    private float timeStuck; //how long has the AI been stuck for?
    private enum VoiceLine {  SpotPlayer, GoingToCallThePolice, CallingPolice, Chasing }
    private NonRepeatingSound chasingRandomSound;
    private NonRepeatingSound callingPoliceRandomSound;
    private NonRepeatingSound goingToCallThePoliceRandomSound;
    private NonRepeatingSound spotPlayerRandomSound;
    private float lastTimePlayedVoiceline = -420f;
    private float voiceLineDuration;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;
    }

    void Start()
    {
        ai = GetComponent<IAstarAI>();
        audioPlayer = GetComponent<AudioPlayer>();
        target = FirstPersonController.instance.MainCamera.transform;
        
        State = EnemyState.Idle;

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

        chasingRandomSound = new NonRepeatingSound(ChasingSounds);
        callingPoliceRandomSound = new NonRepeatingSound(CallingPoliceSounds);
        goingToCallThePoliceRandomSound = new NonRepeatingSound(GoingToCallThePoliceSounds);
        spotPlayerRandomSound = new NonRepeatingSound(SpotPlayerSounds);

        noiseWaypoint = new GameObject().AddComponent<Waypoint>();
    }

    void Update()
    {
        CheckPlayerVisibility();
        Move();
        Interact();
        FootstepAudio();
        UpdateAnimationState();
        DecayNoise();
    }

    void CheckPlayerVisibility()
    {
        if (FirstPersonController.instance.Hiding && IsPlayerWithinFieldOfView() && RaycastToPlayer())
        { FirstPersonController.instance.Hiding = false; } //CAUGHT while going into a hiding spot

        SeesPlayer = IsPlayerWithinFieldOfView() && RaycastToPlayer() && !FirstPersonController.instance.Hiding;

        if (SeesPlayer)
        {
            if (Alertness >= 1f)
            { SpotPlayer(); }
            Alertness = Mathf.Clamp01(Alertness + Time.deltaTime * AlertnessRate);
        }
        else
        {
            Alertness = Mathf.Clamp01(Alertness - Time.deltaTime * AlertnessRate);
        }

        if (State == EnemyState.Chasing && SeesPlayer)
        {
            if (Vector3.SqrMagnitude(transform.position - target.position) <= sqrShootDistance)
            {
                //GLEEK GLACK and SHOOT, dont do this instantly
                //GLEEK GLACK is longer, but then next time you are spotted, she will shoot quickly
                if (Alertness >= 1f) //temp
                { GameManager.instance.GameOver(GameManager.LoseState.Shot); }
            }
        }
    }

    void DecayNoise()
    {
        Noise = Mathf.Clamp(Noise - Time.deltaTime * NoiseDecay, 0f, MaxNoise);
    }

    void PlayVoiceline(VoiceLine voiceLine)
    {
        if(Time.time - lastTimePlayedVoiceline < voiceLineDuration) //make sure we dont overlap voicelines
        {
            //TODO: add voiceline to queue
            return;
        }

        NonRepeatingSound randomSound = null;
        switch(voiceLine)
        {
            case VoiceLine.SpotPlayer:
                randomSound = spotPlayerRandomSound;
                break;
            case VoiceLine.GoingToCallThePolice:
                randomSound = goingToCallThePoliceRandomSound;
                break;
            case VoiceLine.CallingPolice:
                randomSound = callingPoliceRandomSound;
                break;
            case VoiceLine.Chasing:
                randomSound = chasingRandomSound;
                break;
        }

        if(randomSound == null)
        { Debug.LogError("Random Sound null"); return; }

        Sound sound = randomSound.Random();
        audioPlayer.Play(sound);
        voiceLineDuration = sound.Clip.length;

        lastTimePlayedVoiceline = Time.time;
    }

    void UpdateAnimationState()
    {
        anim.SetBool("isWalking", ai.velocity.sqrMagnitude > sqrMoveSpeed);
        if (State == EnemyState.PatrollingWithGun)
        { anim.SetBool("isRifleRunning", ai.velocity.sqrMagnitude > sqrMoveSpeed); }
        //anim.SetBool("isWalking", true);
    }
    
    void OnChangedState()
    {
        Debug.Log("Changed State to " + State);
        switch(State)
        {
            case EnemyState.GoingToCallThePolice:
                PlayVoiceline(VoiceLine.GoingToCallThePolice);
                break;
            case EnemyState.CallingPolice:
                PlayVoiceline(VoiceLine.CallingPolice);
                break;
            case EnemyState.PatrollingWithGun:
                PlayVoiceline(VoiceLine.Chasing);
                break;
        }
    }
    public float OnNoise(Vector3 pos, float amt)
    {
        //returns how much noise it made to the enemy
        localNoise = Vector3.Distance(pos, transform.position) / HearingRadius;
        localNoise = Mathf.Lerp(1, 0, localNoise);
        localNoise = 1f - Mathf.Pow(1f - localNoise, 5f); //easeOutQuint
        localNoise *= amt;

        Noise += localNoise;
        if (Noise >= MaxNoise && (State == EnemyState.Patrolling || State == EnemyState.Idle))  //if we add in an idle State, add it here
        {
            State = EnemyState.SearchingForNoise;
            lastLoudNoisePosition = pos;
        }

        return localNoise;
    }

    void SpotPlayer()
    {
        lastTimeSighted = Time.time;
        if (State == EnemyState.PatrollingWithGun)
        { State = EnemyState.Chasing; }
        else if (State == EnemyState.Patrolling || State == EnemyState.Idle)
        { State = EnemyState.GoingToCallThePolice; }
    }

    void EyeballUI()
    {
        //WARNING
        //this will only work with one enemy
        /*
        float t = Alertness / AlertnessRequired;
        if(t > .66 || (State != EnemyState.Patrolling && State != EnemyState.SearchingForNoise))
        { PlayerUI.instance.SetEyeballUI(PlayerUI.EyeState.Open); }
        else if (t > .33)
        { PlayerUI.instance.SetEyeballUI(PlayerUI.EyeState.Half); }
        else
        { PlayerUI.instance.SetEyeballUI(PlayerUI.EyeState.Closed); }
        */
    }

    void FootstepAudio()
    {
        stepCooldown = StepCooldownChase;
        if (ai.velocity.sqrMagnitude < 0.05f)
        { return; }
        if (Time.time > lastTimeStep + stepCooldown)
        {
            lastTimeStep = Time.time;
            // audioPlayer.Play(FootstepSounds[UnityEngine.Random.Range(0, FootstepSounds.Length)]);
        }
    }

    void Move()
    {
        if (Time.time - lastTimeOpenedDoor < OpenDoorStopTime)
        {
            ai.isStopped = true;
        }
        else
        {
            ai.isStopped = false;
            if (lastDoor != null && (Time.time - lastTimeOpenedDoor > OpenDoorStopTime + 1f))
            { lastDoor.OpenDoor(false); lastDoor = null; } //close the door behind us
        }

        if(State == EnemyState.Idle || State == EnemyState.Patrolling || State == EnemyState.PatrollingWithGun)
        {
            if (SeesPlayer && Alertness < 1f)
            {
                //stop and rotate to face the player
                ai.isStopped = true;
                ai.rotation = Quaternion.Lerp(ai.rotation, Quaternion.LookRotation(new Vector3(target.position.x, 0f, target.position.z) - new Vector3(transform.position.x, 0f, transform.position.z)), Time.deltaTime * 8f);
            }
        }

        switch (State)
        {
            case EnemyState.Idle:
                ai.destination = transform.position;
                break;

            case EnemyState.Chasing:
                ai.destination = target.position;
                lastLoudNoisePosition = target.position;
                break;

            case EnemyState.SearchingForNoise:
                noiseWaypoint.transform.position = lastLoudNoisePosition;
                if (NavigateToWaypoint(noiseWaypoint))
                {
                    State = EnemyState.Patrolling; //investigated the noise, going back to normal
                }
                break;

            case EnemyState.Patrolling:
            case EnemyState.PatrollingWithGun:
                if (NavigateToWaypoint(PatrollingRoute[waypointIndex]))
                {
                    waypointIndex++;
                    if (waypointIndex >= PatrollingRoute.Count)
                    { waypointIndex = 0; }
                }
                break;

            case EnemyState.GoingToCallThePolice:
            case EnemyState.CallingPolice:
                if (NavigateToWaypoint(PhoneWaypoint))
                {
                    LevelManager.instance.CallCops();
                    State = EnemyState.GrabbingGun;
                }
                if (Vector2.SqrMagnitude(pos2D - waypointPos2D) < 0.5f && atWaypoint)
                {
                    State = EnemyState.CallingPolice;
                }
                break;

            case EnemyState.GrabbingGun:
                if (NavigateToWaypoint(GunWaypoint))
                { State = EnemyState.PatrollingWithGun; }
                break;
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
                if (!(interactable as DoorAnimController).IsOpen())
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
        if (Vector2.SqrMagnitude(pos2D - waypointPos2D) < (0.5f + timeStuck)) //add timeStuck onto the threshold to get more generous as the enemy is stuck
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

            if (ai.velocity.sqrMagnitude < 0.25f && ai.isStopped == false) //the ai is stuck somehow
            { timeStuck += Time.deltaTime; }
            else
            { timeStuck = 0f; }

            if (timeStuck >= StuckFixTime)
            {
                LeanTween.cancel(gameObject);
                LeanTween.move(gameObject, AstarPath.active.GetNearest(transform.position).position, 1f);
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

    public void CatPetted()
    {
        if (State == EnemyState.Idle)
        {
            State = EnemyState.Patrolling;
            Debug.Log("First cat petted, starting patrol");
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
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
    public static EnemyState ReloadingState = new ReloadingState();

    public bool ShowScreenSpaceUI { get; protected set; } = true;

    public virtual void Init(Enemy enemy, IAstarAI ai) 
    { enemy.GunObject.SetActive(false); }
    public virtual void Update(Enemy enemy, IAstarAI ai) { }
    public virtual void OnNoise(Enemy enemy, Vector3 noisePos) { }
    public virtual void SetAnimationState(Enemy enemy, Animator anim)
    { anim.SetBool("isWalking", enemy.Moving); }
    //call this method in the update loop to make the enemy stop and rotate to face the player when sighted
    protected void StopAndRotateToFacePlayerIfVisible(Enemy enemy, IAstarAI ai)
    {
        if (enemy.SeesPlayer)
        {
            //stop and rotate to face the player
            ai.isStopped = true;
            ai.rotation = Quaternion.Lerp(ai.rotation, Quaternion.LookRotation(new Vector3(enemy.PlayerTransform.position.x, 0f, enemy.PlayerTransform.position.z) - new Vector3(enemy.transform.position.x, 0f, enemy.transform.position.z)), Time.deltaTime* 3f);
        }
    }
}

public class SittingState : EnemyState
{
    public override void Update(Enemy enemy, IAstarAI ai)
    {
        enemy.RedLightTargetIntensity = enemy.SeesPlayer ? enemy.RedLightIntensityNormal : 0f;

        //Movement
        ai.isStopped = true;
        enemy.NavigateToPosition(enemy.transform.position);

        //Spotting
        if(enemy.SpottedPlayer)
        {
            enemy.PlayVoiceline(Enemy.VoiceLine.SpotPlayer);
            enemy.SetState(CallingCopsGrabbingGunState);
        }

        base.StopAndRotateToFacePlayerIfVisible(enemy, ai);
        PlayerUI.instance.SetSpottedGradient(enemy.SeesPlayer, enemy.transform.position);
    }

    public override void OnNoise(Enemy enemy, Vector3 noisePos)
    { enemy.SetState(SearchingForNoiseState); }

    public override void Init(Enemy enemy, IAstarAI ai)
    {
        enemy.RedLightTargetIntensity = 0f;
        base.ShowScreenSpaceUI = true;
    }
}

public class PatrollingState : EnemyState
{
    private int currentWaypointIndex;

    public override void Update(Enemy enemy, IAstarAI ai)
    {
        enemy.RedLightTargetIntensity = enemy.SeesPlayer ? enemy.RedLightIntensityNormal : 0f;

        //Movement
        ai.isStopped = false;
        enemy.SetWaypoint(enemy.PatrollingRoute[currentWaypointIndex]);

        //Patrolling
        if(enemy.ArrivedAtDestinationOrStuck)
        { currentWaypointIndex++; }
        if(currentWaypointIndex >= enemy.PatrollingRoute.Count)
        { currentWaypointIndex = 0; }

        //Spotting
        if (enemy.SpottedPlayer)
        { OnSpotted(enemy); }

        base.StopAndRotateToFacePlayerIfVisible(enemy, ai);
        PlayerUI.instance.SetSpottedGradient(enemy.SeesPlayer, enemy.transform.position);
    }

    public virtual void OnSpotted(Enemy enemy)
    {
        enemy.PlayVoiceline(Enemy.VoiceLine.SpotPlayer);
        enemy.SetState(CallingCopsGrabbingGunState);
    }

    public override void OnNoise(Enemy enemy, Vector3 noisePos)
    { enemy.SetState(SearchingForNoiseState); }

    public override void Init(Enemy enemy, IAstarAI ai)
    {
        enemy.RedLightTargetIntensity = 0f;
        base.ShowScreenSpaceUI = true;
    }
}

public class ReloadingState : EnemyState
{
    private float timeStartedReloading = -420f;

    public override void Init(Enemy enemy, IAstarAI ai)
    {
        //PLAY I MISSED AUDIO
        timeStartedReloading = Time.time;
        base.ShowScreenSpaceUI = false;
    }

    public override void Update(Enemy enemy, IAstarAI ai)
    {
        ai.isStopped = true;
        if (Time.time - timeStartedReloading > enemy.ReloadTime)
        { enemy.SetState(PatrollingWithGunState); }
    }
}

public class PatrollingWithGunState : PatrollingState
{
    private float lastTimePlayedChasingVoiceline = -420f;
    private float chasingVoicelineDuration = -420f;
    private int timesShot = 0;
    private bool lastTimeSeesPlayer;
    private float lastTimePlayedReloadSound = -420f;
    private const float reloadSoundDelay = 4f;

    public override void Init(Enemy enemy, IAstarAI ai)
    {
        enemy.GunObject.SetActive(true);
        lastTimeSeesPlayer = enemy.SeesPlayer;
        lastTimePlayedReloadSound = -420f;
        base.ShowScreenSpaceUI = false;
    }

    public override void Update(Enemy enemy, IAstarAI ai)
    {
        base.Update(enemy, ai);

        //gun shooty stuff
        if (enemy.Awareness >= .98f)
        {
            UnityEngine.Debug.Log(FirstPersonController.instance.IsMoving);
            if (timesShot >= 1 || !FirstPersonController.instance.IsMoving)
            {
                //GAME OVER
                enemy.enabled = false;
                GameManager.instance.GameOver(GameManager.LoseState.Shot);
            }
            else
            { } //missed

            enemy.AudioPlayer.Play(enemy.ShotgunSound_Fire);
            timesShot++;
            enemy.SetState(ReloadingState);
        }

        bool canPlayReloadSound = Time.time - lastTimePlayedReloadSound > reloadSoundDelay;

        //GLEEK GLACK reload sounds
        if(enemy.SeesPlayer && !lastTimeSeesPlayer && canPlayReloadSound)
        {
            enemy.AudioPlayer.Play(enemy.ShotgunSound_Reload);
            lastTimePlayedReloadSound = Time.time;
        }
        lastTimeSeesPlayer = enemy.SeesPlayer;

        //flashing red
        enemy.RedLightTargetIntensity = Mathf.Lerp(enemy.RedLightIntensityNormal, enemy.RedLightIntensityHigh, Mathf.Abs(Mathf.Sin(Time.time)));

        //Voicelines
        if (Time.time - lastTimePlayedChasingVoiceline > chasingVoicelineDuration)
        {
            Sound soundPlayed = enemy.PlayVoiceline(Enemy.VoiceLine.Chasing);
            if(soundPlayed != null)
            {
                chasingVoicelineDuration = soundPlayed.Clip.length * 1.5f;
                lastTimePlayedChasingVoiceline = Time.time;
            }
        }

        base.StopAndRotateToFacePlayerIfVisible(enemy, ai);
        PlayerUI.instance.SetSpottedGradient(enemy.SeesPlayer, enemy.transform.position);
    }

    public override void SetAnimationState(Enemy enemy, Animator anim)
    {
        anim.SetBool("isWalking", true);
        anim.SetBool("isRifleRunning", true);
        //anim.SetBool("isRifleIdling", !enemy.Moving);
    }

    public override void OnSpotted(Enemy enemy) { }
    public override void OnNoise(Enemy enemy, Vector3 noisePos) { }
}

public class SearchingForNoiseState : EnemyState
{
    public Vector3 NoisePosition;

    public override void Update(Enemy enemy, IAstarAI ai)
    {
        //Movement
        ai.isStopped = false;
        enemy.NavigateToPosition(NoisePosition);

        if(enemy.ArrivedAtDestinationOrStuck)
        { /*enemy.SetState(enemy.LastState);*/ enemy.SetState(PatrollingState); }

        //Spotting
        if(enemy.SpottedPlayer)
        {
            enemy.PlayVoiceline(Enemy.VoiceLine.SpotPlayer);
            enemy.SetState(CallingCopsGrabbingGunState);
        }

        base.StopAndRotateToFacePlayerIfVisible(enemy, ai);
        PlayerUI.instance.SetSpottedGradient(enemy.SeesPlayer, enemy.transform.position);
    }

    public override void OnNoise(Enemy enemy, Vector3 noisePos)
    { NoisePosition = noisePos; }

    public override void Init(Enemy enemy, IAstarAI ai)
    {
        enemy.RedLightTargetIntensity = enemy.RedLightIntensityHigh;
        base.ShowScreenSpaceUI = true;
    }
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
                enemy.SetWaypoint(enemy.PhoneWaypoint);

                if(enemy.ArrivedAtDestinationOrStuck)
                { state = State.GrabbingGun; }

                break;
            case State.GrabbingGun:
                enemy.SetWaypoint(enemy.GunWaypoint);

                if(enemy.ArrivedAtDestinationOrStuck)
                {
                    LevelManager.instance.CallCops();
                    enemy.AudioPlayer.Play(enemy.ShotgunSound_Reload);
                    enemy.SetState(PatrollingWithGunState);
                }

                break;
        }

        PlayerUI.instance.SetSpottedGradient(false, enemy.transform.position);
    }

    public override void Init(Enemy enemy, IAstarAI ai)
    {
        enemy.RedLightTargetIntensity = 0f;
        base.ShowScreenSpaceUI = false;
    }
}

[RequireComponent(typeof(IAstarAI))]
[RequireComponent(typeof(AudioPlayer))]
public class Enemy : MonoBehaviour
{
    public static Enemy instance; //singleton because multiple Enemies are not in the scope of this project

    public bool DebugMode;
    public Transform EyePosition;
    public Light RedLight;
    public List<Waypoint> PatrollingRoute;
    public Waypoint PhoneWaypoint;
    public Waypoint GunWaypoint;
    public GameObject GunObject;
    public EnemyDebug DebugObject;
    public Animator Anim;
    public float SightDistance = 12f;
    public float AwarenessRate = 0.5f;
    public float AwarenessMultiplierBackTurned = 0.5f;
    public float NoiseThreshold = 0.5f;
    [Tooltip("How visible does the player need to be to be spotted [0.0-1.0]")]
    [Range(0f,1f)]
    public float VisibilityThreshold = 0.5f;
    public float ReloadTime = 1.5f;
    public float RedLightIntensityNormal = 4f;
    public float RedLightIntensityHigh = 8f;
    [SerializeField] private LayerMask everythingBesidesEnemy;
    public Sound[] SpotPlayerSounds;
    public Sound[] GoingToCallThePoliceSounds;
    public Sound[] CallingPoliceSounds;
    public Sound[] ChasingSounds;
    public Sound ShotgunSound_Reload;
    public Sound ShotgunSound_Fire;

    [HideInInspector] public AudioPlayer AudioPlayer;
    [HideInInspector] public EnemyState State { get; private set; } = EnemyState.SittingState;
    [HideInInspector] public EnemyState LastState { get; private set; } = EnemyState.SittingState;
    [HideInInspector] public float RedLightTargetIntensity { get; set; }
    public bool SeesPlayer { get; private set; }
    public bool SpottedPlayer { get; private set; }
    public bool ArrivedAtDestinationOrStuck { get; private set; }
    public bool Moving { get; private set; }
    public float Awareness { get; private set; }
    public float PercentVisible { get; private set; }
    public Transform PlayerTransform { get; private set; }

    private IAstarAI ai;
    private bool raycastedToPlayer;
    private Waypoint currentWaypoint;
    private float sightDistanceTarget;
    private float sightDistance;

    private void Awake()
    {
        ai = GetComponent<IAstarAI>();
        LevelManager.OnNoise += OnNoiseCallback;
        Cat.CompletedPetting += CompletedPettingCallback;

        if(instance != null)
        { Destroy(this.gameObject); return; }
        instance = this;
    }

    private void OnDestroy()
    {
        LevelManager.OnNoise -= OnNoiseCallback;
        Cat.CompletedPetting -= CompletedPettingCallback;
    }

    private void Start()
    {
        PlayerTransform = FirstPersonController.instance.transform;
        DebugObject.gameObject.SetActive(DebugMode);
        AudioPlayer = GetComponent<AudioPlayer>();
        chasingRandomSound = new NonRepeatingSound(ChasingSounds);
        callingPoliceRandomSound = new NonRepeatingSound(CallingPoliceSounds);
        goingToCallThePoliceRandomSound = new NonRepeatingSound(GoingToCallThePoliceSounds);
        spotPlayerRandomSound = new NonRepeatingSound(SpotPlayerSounds);

        sightDistance = SightDistance;
        sightDistanceTarget = SightDistance;

        RedLightTargetIntensity = 0f;
        RedLight.intensity = RedLightTargetIntensity;
        State.Init(this, ai);
    }

    private void Update()
    {
        bool pastThreshold = PercentVisible >= VisibilityThreshold;

        if (raycastedToPlayer)
        {
            float awarenessAmt = Time.deltaTime;
            awarenessAmt *= AwarenessRate;
            if (!PlayerUI.instance.EnemyOnScreen)
            { awarenessAmt *= AwarenessMultiplierBackTurned; }
            awarenessAmt *= PercentVisible;
            if (!pastThreshold && Awareness < 0.25f) //0.25 is the threshold for ignoring the threshold lmao
            { awarenessAmt = 0f; }
            Awareness += awarenessAmt;
        }
        else
        { Awareness = 0f; }

        SeesPlayer = Awareness > 0.05f;
        SpottedPlayer = Awareness >= .99f;       
        Moving = !ai.isStopped && ai.velocity.sqrMagnitude > .1f;

        sightDistanceTarget = SightDistance;
        if (raycastedToPlayer)
        { sightDistanceTarget = SightDistance * 2f; } //give her more range once were spotted
        sightDistance = Mathf.Lerp(sightDistance, sightDistanceTarget, Time.deltaTime);

        //TODO: stuck prevention

        CalculateArrivedAtDestination();

        State.Update(this, ai);
        State.SetAnimationState(this, Anim);

        //red light
        RedLight.intensity = Mathf.Lerp(RedLight.intensity, RedLightTargetIntensity, Time.deltaTime * 2f);

        PlayQueuedVoicelines();
    }

    //pain
    private void CalculateArrivedAtDestination()
    {
        enemyPos2D.x = transform.position.x;
        enemyPos2D.y = transform.position.z;
        destinationPos2D.x = ai.destination.x;
        destinationPos2D.y = ai.destination.z;
        ArrivedAtDestinationOrStuck = Vector2.SqrMagnitude(enemyPos2D - destinationPos2D) <= 0.2f;
    }

    private void FixedUpdate()
    {
        raycastedToPlayer = RaycastToPlayer();
    }

    Vector2 enemyPos2D, destinationPos2D;
    public void NavigateToPosition(Vector3 pos)
    {
        ai.destination = pos;
        CalculateArrivedAtDestination();
    }

    public void SetWaypoint(Waypoint waypoint)
    {
        currentWaypoint = waypoint;
        NavigateToWaypoint();
        CalculateArrivedAtDestination();
    }

    bool atWaypoint;
    private float lastTimeAtWaypoint = -420f;
    private void NavigateToWaypoint()
    {
        ai.destination = currentWaypoint.transform.position;

        if (ArrivedAtDestinationOrStuck)
        {
            if (!atWaypoint)
            {
                lastTimeAtWaypoint = Time.time;
                atWaypoint = true;
            }
            if (Time.time - lastTimeAtWaypoint > currentWaypoint.StopTime)
            {
                //arrival
                atWaypoint = false;
                currentWaypoint = null;
            }
        }
    }

    bool IsPlayerWithinFieldOfView()
    {
        return Vector3.Dot(transform.TransformDirection(Vector3.forward), (PlayerTransform.position - transform.position).normalized) >= .15;
    }

    int raycastsHit;
    bool RaycastToPlayer()
    {
        PercentVisible = 0f;
        raycastsHit = 0;

        if(!IsPlayerWithinFieldOfView())
        { return false; }

        bool hit = false;
        for (int i = 0; i < FirstPersonController.instance.VisibilityCheckPoints.Length; i++)
        {
            if (RaycastToPoint(FirstPersonController.instance.VisibilityCheckPoints[i].position))
            {
                hit = true;
                raycastsHit++;
            }
        }

        PercentVisible = (float)raycastsHit / (float)FirstPersonController.instance.VisibilityCheckPoints.Length;

        return hit;
    }

    RaycastHit hit;
    bool RaycastToPoint(Vector3 point)
    {
        bool hitPlayer = false;
        Vector3 direction = (point - EyePosition.position);

        if (Physics.Raycast(EyePosition.position, direction, out hit, sightDistance, everythingBesidesEnemy, QueryTriggerInteraction.Collide))
        {
            hitPlayer = hit.collider.CompareTag("Player");
            if (DebugMode)
            { Debug.DrawLine(EyePosition.position, hit.point, hitPlayer? Color.green : Color.red, 0.25f); }

            if (!hitPlayer)
            { return false; }
        }
        else
        {
            if(!DebugMode)
            { return false; }

            Debug.DrawLine(EyePosition.position, EyePosition.position + direction * SightDistance, Color.red, 0.25f);
        }

        return hitPlayer;
    }

    private void OnNoiseCallback(Vector3 pos, float noisePercentage)
    {
        if (noisePercentage > NoiseThreshold)
        { State.OnNoise(this, pos); }
    }

    private void CompletedPettingCallback()
    {
        if (State == EnemyState.SittingState)
        { SetState(EnemyState.PatrollingState); }
    }

    public void SetState(EnemyState newState)
    {
        if(State == newState)
        { return; }
        LastState = State;
        State = newState;
        State.Init(this, ai);
        Awareness = 0f;
    }

    public enum VoiceLine { SpotPlayer, GoingToCallThePolice, CallingPolice, Chasing }
    private float lastTimePlayedVoiceline = -420f;
    private float voiceLineDuration;
    private float lastTimePlayedChasingVoiceline = -420f;
    private float chasingVoicelineDuration;
    private NonRepeatingSound chasingRandomSound;
    private NonRepeatingSound callingPoliceRandomSound;
    private NonRepeatingSound goingToCallThePoliceRandomSound;
    private NonRepeatingSound spotPlayerRandomSound;
    private Queue<VoiceLine> voicelineQueue = new Queue<VoiceLine>();

    //returns a reference to the sound that was played
    public Sound PlayVoiceline(VoiceLine voiceLine)
    {
        if (Time.time - lastTimePlayedVoiceline < voiceLineDuration) //make sure we dont overlap voicelines
        {
            print("Queueing voiceline " + voiceLine);
            voicelineQueue.Enqueue(voiceLine);
            return null;
        }

        NonRepeatingSound randomSound = null;
        switch (voiceLine)
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

        if (randomSound == null)
        { Debug.LogError("Random Sound null"); return null; }

        Sound sound = randomSound.Random();
        AudioPlayer.Play(sound);
        print("Playing voiceline " + voiceLine);
        voiceLineDuration = sound.Clip.length;


        lastTimePlayedVoiceline = Time.time;
        return sound;
    }

    private void PlayQueuedVoicelines()
    {
        if(voicelineQueue.Count <= 0)
        { return; }

        if (Time.time - lastTimePlayedVoiceline >= voiceLineDuration)
        { PlayVoiceline(voicelineQueue.Dequeue()); }
    }

    //Editor Gizmo stuff
    private void OnDrawGizmosSelected()
    {
        return;
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
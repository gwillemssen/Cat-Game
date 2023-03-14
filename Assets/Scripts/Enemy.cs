using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;
using System;

#region Awareness
public class Awareness
{
    public enum AwarenessEnum { Idle, Warning, Alerted }
    public AwarenessEnum AwarenessValue { get; private set; }

    private Enemy enemy;
    private float value;
    private float maxValue
    {
        get 
        {
            if (AwarenessValue == AwarenessEnum.Idle)
            { return enemy.Awareness_IdleState_Duration; }
            else if (AwarenessValue == AwarenessEnum.Warning)
            { return enemy.Awareness_WarningState_Duration; }
            return 1f;
        }
    }

    public Awareness(Enemy enemy)
    { this.enemy = enemy; }

    public void Update(float delta, float percentVisible, bool enemyOnScreen)
    {
        bool visible = percentVisible >= enemy.VisibilityThreshold;

        float awarenessMultiplier = 1f;
        if(!enemyOnScreen)
        { awarenessMultiplier = enemy.AwarenessMultiplier_BackTurned; }

        value += visible ? enemy.AwarenessRate * delta * awarenessMultiplier : -enemy.AwarenessDecayRate * delta;
        value = Mathf.Clamp(value, 0f, maxValue);      

        if(value >= maxValue && AwarenessValue < AwarenessEnum.Alerted)
        {
            AwarenessValue++;
            value = 0f;
        }
        else if(value <= 0f && AwarenessValue > enemy.State.MinimumAlertness)
        {
            AwarenessValue--;
            value = maxValue;
        }

        AwarenessValue = (AwarenessEnum)Math.Clamp((int)AwarenessValue, 0, (int)AwarenessEnum.Alerted);
    }

    public void SetMinimum(AwarenessEnum min)
    {
        if(AwarenessValue >= min)
        { return; }

        value = 0f;
        AwarenessValue = min;
    }

    public override string ToString()
    {
        return "STATE: " + AwarenessValue.ToString() + " value: " + value + " / " + maxValue;
    }
}
#endregion

#region EnemyState
public class EnemyState
{
    //I'm just now realizing I could have made the classes static instead of using singletons
    public static SittingState SittingState = new SittingState();
    public static PatrollingState PatrollingState = new PatrollingState();
    public static InvestigatingState InvestigatingState = new InvestigatingState();
    public static GrabbingGunState GrabbingGunState = new GrabbingGunState();
    public static PatrollingWithGunState PatrollingWithGunState = new PatrollingWithGunState();
    public static ReloadingState ReloadingState = new ReloadingState();

    public bool ShowScreenSpaceUI { get; protected set; } = true;
    public Awareness.AwarenessEnum MinimumAlertness { get; protected set; } = Awareness.AwarenessEnum.Idle; //the minimum alertness state the enemy can be in

    public virtual void Init(Enemy enemy, NavMeshAgent ai) 
    { enemy.GunObject.SetActive(false); }
    public virtual void Update(Enemy enemy, NavMeshAgent ai) { }
    public virtual void OnDistract(Enemy enemy, Vector3 position) { }
    public virtual void SetAnimationState(Enemy enemy, Animator anim)
    { anim.SetBool("isWalking", enemy.Moving); }
    //call this method in the update loop to make the enemy stop and rotate to face the player when sighted
    protected void StopAndRotateToFacePlayerIfVisible(Enemy enemy, NavMeshAgent ai)
    {
        ai.isStopped = false;
        if (enemy.SeesPlayer)
        {
            //stop and rotate to face the player
            ai.isStopped = true;
            ai.transform.rotation = Quaternion.Lerp(ai.transform.rotation, Quaternion.LookRotation(new Vector3(enemy.PlayerTransform.position.x, 0f, enemy.PlayerTransform.position.z) - new Vector3(enemy.transform.position.x, 0f, enemy.transform.position.z)), Time.deltaTime* 4.5f);
        }
    }
    private bool canPlayAlertedVoiceline = true;
    private float voicelineCooldownTimer;
    protected void PlayAlertedVoicelineIfInWarningState(Enemy enemy)
    {
        if (canPlayAlertedVoiceline && enemy.Awareness.AwarenessValue == Awareness.AwarenessEnum.Warning)
        {
            voicelineCooldownTimer += enemy.PlayVoiceline(Enemy.VoiceLine.Alerted).Clip.length + 2f;
            canPlayAlertedVoiceline = false;
        }
        if(voicelineCooldownTimer <= 0f)
        { canPlayAlertedVoiceline = true; }

        voicelineCooldownTimer = Mathf.Max(0f, voicelineCooldownTimer - Time.deltaTime);
    }
}

public class SittingState : EnemyState
{
    public override void Update(Enemy enemy, NavMeshAgent ai)
    {
        enemy.RedLightTargetIntensity = enemy.Awareness.AwarenessValue == Awareness.AwarenessEnum.Warning ? enemy.RedLightIntensityNormal : 0f;

        //Movement
        ai.isStopped = true;
        enemy.NavigateToPosition(enemy.transform.position);

        //Spotting
        if(enemy.Awareness.AwarenessValue == Awareness.AwarenessEnum.Alerted)
        {
            enemy.PlayVoiceline(Enemy.VoiceLine.SpotPlayer);
            enemy.SetState(GrabbingGunState);
        }

        base.StopAndRotateToFacePlayerIfVisible(enemy, ai);
        base.PlayAlertedVoicelineIfInWarningState(enemy);
        PlayerUI.instance.SetSpottedGradient(enemy.Awareness.AwarenessValue == Awareness.AwarenessEnum.Warning, enemy.transform.position);
    }

    public override void Init(Enemy enemy, NavMeshAgent ai)
    {
        enemy.RedLightTargetIntensity = 0f;
        base.ShowScreenSpaceUI = true;
        base.MinimumAlertness = Awareness.AwarenessEnum.Idle;
    }

    public override void OnDistract(Enemy enemy, Vector3 position)
    {
        if (enemy.SeesPlayer) //ignore if we already see the player - cannot be distracted
        { return; }
        enemy.SetState(InvestigatingState);
        InvestigatingState.Position = position;
    }
}

public class PatrollingState : EnemyState
{
    private int currentWaypointIndex;
    private bool lastSpottedPlayer;

    public override void Update(Enemy enemy, NavMeshAgent ai)
    {
        enemy.RedLightTargetIntensity = enemy.Awareness.AwarenessValue == Awareness.AwarenessEnum.Warning ? enemy.RedLightIntensityNormal : 0f;

        //Movement
        ai.isStopped = false;
        enemy.SetWaypoint(enemy.PatrollingRoute[currentWaypointIndex]);

        //Patrolling
        if(enemy.ArrivedAtDestinationOrStuck)
        {
            int last = currentWaypointIndex;
            currentWaypointIndex = UnityEngine.Random.Range(0, enemy.PatrollingRoute.Count);
            if(currentWaypointIndex == last)
            { currentWaypointIndex++; }
        }
        if(currentWaypointIndex >= enemy.PatrollingRoute.Count)
        { currentWaypointIndex = 0; }

        //Spotting
        if(enemy.Awareness.AwarenessValue == Awareness.AwarenessEnum.Idle && lastSpottedPlayer) //player got out of enemies sights - go investigate
        {
            OnDistract(enemy, FirstPersonController.instance.transform.position);
        }
        lastSpottedPlayer = enemy.Awareness.AwarenessValue != Awareness.AwarenessEnum.Idle;

        if (enemy.Awareness.AwarenessValue == Awareness.AwarenessEnum.Alerted)
        { OnSpotted(enemy); }

        base.StopAndRotateToFacePlayerIfVisible(enemy, ai);
        base.PlayAlertedVoicelineIfInWarningState(enemy);
        PlayerUI.instance.SetSpottedGradient(enemy.Awareness.AwarenessValue == Awareness.AwarenessEnum.Warning, enemy.transform.position);
    }

    public virtual void OnSpotted(Enemy enemy)
    {
        enemy.PlayVoiceline(Enemy.VoiceLine.SpotPlayer);
        enemy.SetState(GrabbingGunState);
    }

    public override void Init(Enemy enemy, NavMeshAgent ai)
    {
        enemy.RedLightTargetIntensity = 0f;
        base.ShowScreenSpaceUI = true;
        base.MinimumAlertness = Awareness.AwarenessEnum.Idle;
    }

    public override void OnDistract(Enemy enemy, Vector3 position)
    {
        if(enemy.SeesPlayer) //ignore if we already see the player - cannot be distracted
        { return; }
        enemy.SetState(InvestigatingState);
        InvestigatingState.Position = position;
    }
}

public class ReloadingState : EnemyState
{
    private float timeStartedReloading = -420f;

    public override void Init(Enemy enemy, NavMeshAgent ai)
    {
        //PLAY I MISSED AUDIO
        timeStartedReloading = Time.time;
        base.ShowScreenSpaceUI = false;
        base.MinimumAlertness = Awareness.AwarenessEnum.Alerted;
    }

    public override void Update(Enemy enemy, NavMeshAgent ai)
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
    private float timeVisible;

    public override void Init(Enemy enemy, NavMeshAgent ai)
    {
        enemy.GunObject.SetActive(true);
        lastTimePlayedReloadSound = -420f;
        base.ShowScreenSpaceUI = false;
        base.MinimumAlertness = Awareness.AwarenessEnum.Alerted;
        enemy.FovDotProduct = -.85f; //make the FOV of the enemy really high
    }

    public override void Update(Enemy enemy, NavMeshAgent ai)
    {
        base.Update(enemy, ai);

        if(enemy.SeesPlayer)
        { timeVisible += Time.deltaTime; }
        else
        { timeVisible -= Time.deltaTime ; }

        timeVisible = Mathf.Clamp(timeVisible, 0f, enemy.TimeUntilShoot);

        //gun shooty stuff
        if (timeVisible >= enemy.TimeUntilShoot)
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
            timeVisible = 0f;
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

        //move to player if spotted
        base.StopAndRotateToFacePlayerIfVisible(enemy, ai);
        if (enemy.SeesPlayer)
        {
            ai.isStopped = false;
            ai.destination = FirstPersonController.instance.transform.position;
            if(Vector3.SqrMagnitude(FirstPersonController.instance.transform.position - enemy.transform.position) <= 1f)
            { ai.isStopped = true; }
        }

        //Voicelines
        if (Time.time - lastTimePlayedChasingVoiceline > chasingVoicelineDuration)
        {
            Sound soundPlayed = enemy.PlayVoiceline(Enemy.VoiceLine.Chasing);
            if(soundPlayed != null)
            {
                chasingVoicelineDuration = soundPlayed.Clip.length * 2.25f;
                lastTimePlayedChasingVoiceline = Time.time;
            }
        }

        PlayerUI.instance.SetSpottedGradient(enemy.Awareness.AwarenessValue == Awareness.AwarenessEnum.Warning, enemy.transform.position);
    }

    public override void SetAnimationState(Enemy enemy, Animator anim)
    {
        anim.SetBool("isWalking", true);
        anim.SetBool("isRifleRunning", true);
        //anim.SetBool("isRifleIdling", !enemy.Moving);
    }

    public override void OnSpotted(Enemy enemy) { }

    public override string ToString()
    {
        return base.ToString() + " time: " + timeVisible;
    }
}

public class InvestigatingState : EnemyState
{
    public Vector3 Position;

    private float investigatingTimer;

    public override void Update(Enemy enemy, NavMeshAgent ai)
    {
        //Movement
        ai.isStopped = false;
        enemy.NavigateToPosition(Position);

        if(enemy.ArrivedAtDestinationOrStuck)
        {
            investigatingTimer += Time.deltaTime;
            if(investigatingTimer > enemy.InvestigateTime)
            {/*enemy.SetState(enemy.LastState);*/ enemy.SetState(PatrollingState); } //for now, the only outcome of this state is Patrolling
        }

        //Spotting
        if(enemy.Awareness.AwarenessValue == Awareness.AwarenessEnum.Alerted)
        {
            enemy.PlayVoiceline(Enemy.VoiceLine.SpotPlayer);
            enemy.SetState(GrabbingGunState);
        }

        base.StopAndRotateToFacePlayerIfVisible(enemy, ai);
        PlayerUI.instance.SetSpottedGradient(enemy.Awareness.AwarenessValue == Awareness.AwarenessEnum.Warning, enemy.transform.position);
    }

    public override void Init(Enemy enemy, NavMeshAgent ai)
    {
        enemy.RedLightTargetIntensity = enemy.RedLightIntensityHigh;
        base.ShowScreenSpaceUI = true;
        base.MinimumAlertness = Awareness.AwarenessEnum.Warning;
        investigatingTimer = 0f;
    }

    public override void OnDistract(Enemy enemy, Vector3 position)
    {
        Position = position;
    }
}

public class GrabbingGunState : EnemyState
{
    public override void Update(Enemy enemy, NavMeshAgent ai)
    {
        //Movement
        ai.isStopped = false;
        enemy.SetWaypoint(enemy.GunWaypoint);

        if (enemy.ArrivedAtDestinationOrStuck)
        {
            enemy.AudioPlayer.Play(enemy.ShotgunSound_Reload);
            enemy.SetState(PatrollingWithGunState);
        }

        PlayerUI.instance.SetSpottedGradient(false, enemy.transform.position);
    }

    public override void Init(Enemy enemy, NavMeshAgent ai)
    {
        enemy.RedLightTargetIntensity = 0f;
        base.ShowScreenSpaceUI = false;
        base.MinimumAlertness = Awareness.AwarenessEnum.Alerted;
    }
}
#endregion

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AudioPlayer))]
public class Enemy : MonoBehaviour
{
    public static Enemy instance; //singleton because multiple Enemies are not in the scope of this project

    public bool DebugMode;
    public Transform EyePosition;
    public Light RedLight;
    public List<Waypoint> PatrollingRoute;
    public Waypoint GunWaypoint;
    public GameObject GunObject;
    public EnemyDebug DebugObject;
    public Animator Anim;
    public float SightDistance = 12f;
    public float AwarenessRate = 0.5f;
    public float AwarenessMultiplier_BackTurned = 0.6f;
    public float AwarenessDecayRate = 0.3f;
    public float Awareness_IdleState_Duration = 0.4f;
    public float Awareness_WarningState_Duration = 1.8f;
    public float InvestigateTime = 3.5f;
    public float CloseDistance = 1.5f;
    public float TimeUntilShoot = 1.2f;
    public float FovDotProduct = 0.15f;
    //public float AwarenessMultiplierBackTurned = 0.5f;
    [Tooltip("How visible does the player need to be to be spotted [0.0-1.0]")]
    [Range(0f,1f)]
    public float VisibilityThreshold = 0.5f;
    public float ReloadTime = 1.5f;
    public float RedLightIntensityNormal = 4f;
    public float RedLightIntensityHigh = 8f;
    [SerializeField] private LayerMask everythingBesidesEnemy;
    public Sound[] SpotPlayerSounds;
    public Sound[] GrabbingGunSounds;
    public Sound[] ChasingSounds;
    public Sound[] AlertedSounds;
    public Sound ShotgunSound_Reload;
    public Sound ShotgunSound_Fire;

    [HideInInspector] public AudioPlayer AudioPlayer;
    [HideInInspector] public EnemyState State { get; private set; } = EnemyState.SittingState;
    [HideInInspector] public EnemyState LastState { get; private set; } = EnemyState.SittingState;
    [HideInInspector] public float RedLightTargetIntensity { get; set; }
    public bool ArrivedAtDestinationOrStuck { get; private set; }
    public bool Moving { get; private set; }
    public Awareness Awareness { get; private set; }
    public float PercentVisible { get; private set; }
    public Transform PlayerTransform { get; private set; }
    public bool SeesPlayer { get; private set; }

    private NavMeshAgent ai;
    private bool raycastedToPlayer;
    private Waypoint currentWaypoint;
    private float sightDistanceTarget;
    private float sightDistance;
    private float sqrCloseDistance;

    private void Awake()
    {
        if (instance != null)
        { Destroy(this.gameObject); return; }
        instance = this;

        ai = GetComponent<NavMeshAgent>();
        Cat.CompletedPetting += CompletedPettingCallback;
        Awareness = new Awareness(this);
        sqrCloseDistance = CloseDistance * CloseDistance;
    }

    private void OnDestroy()
    {
        Cat.CompletedPetting -= CompletedPettingCallback;
    }

    private void Start()
    {
        PlayerTransform = FirstPersonController.instance.transform;
        DebugObject.gameObject.SetActive(DebugMode);
        AudioPlayer = GetComponent<AudioPlayer>();
        chasingRandomSound = new NonRepeatingSound(ChasingSounds);
        spotPlayerRandomSound = new NonRepeatingSound(SpotPlayerSounds);
        alertedRandomSound = new NonRepeatingSound(AlertedSounds);
        grabbingGunRandonSound = new NonRepeatingSound(GrabbingGunSounds);

        sightDistance = SightDistance;
        sightDistanceTarget = SightDistance;

        RedLightTargetIntensity = 0f;
        RedLight.intensity = RedLightTargetIntensity;
        State.Init(this, ai);

        GunObject.SetActive(false);
    }

    private void Update()
    {
        SeesPlayer = PercentVisible >= VisibilityThreshold;

        //Automatically see player if theyre within a distance
        if (Vector3.SqrMagnitude(FirstPersonController.instance.MainCamera.transform.position - EyePosition.transform.position) <= sqrCloseDistance)
        { 
            PercentVisible = 1f;
            SeesPlayer = true;
        }
        Awareness.Update(Time.deltaTime, PercentVisible, PlayerUI.instance.EnemyOnScreen);
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

    public void Distract(Vector3 position)
    {
        State.OnDistract(this, position);
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
        return Vector3.Dot(transform.TransformDirection(Vector3.forward), (PlayerTransform.position - transform.position).normalized) >= FovDotProduct;
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

    private void CompletedPettingCallback()
    {
        if (State == EnemyState.SittingState)
        {
            SetState(EnemyState.PatrollingState);
            PlayVoiceline(VoiceLine.Alerted);
        }
    }

    public void SetState(EnemyState newState)
    {
        if(State == newState)
        { return; }
        LastState = State;
        State = newState;
        State.Init(this, ai);
        Awareness.SetMinimum(newState.MinimumAlertness);
    }

    public enum VoiceLine { SpotPlayer, GrabbingGun, Chasing, Alerted }
    private float lastTimePlayedVoiceline = -420f;
    private float voiceLineDuration;
    private float lastTimePlayedChasingVoiceline = -420f;
    private float chasingVoicelineDuration;
    private NonRepeatingSound chasingRandomSound;
    private NonRepeatingSound spotPlayerRandomSound;
    private NonRepeatingSound alertedRandomSound;
    private NonRepeatingSound grabbingGunRandonSound;
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
            case VoiceLine.Chasing:
                randomSound = chasingRandomSound;
                break;
            case VoiceLine.Alerted:
                randomSound = alertedRandomSound;
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
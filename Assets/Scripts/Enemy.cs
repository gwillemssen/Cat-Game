using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(IAstarAI))]
public class Enemy : MonoBehaviour
{
    public enum EnemyState { Idle, Patrolling, Chasing, LostTarget }
    [Range(0, 180f)]
    public float FieldOfView = 90f;
    public List<Transform> PatrollingRoute;
    public Transform eyes;
    public float SightDistance = 12f;
    public float ChaseTime = 6f;
    public float LostTargetTime = 3f;
    public LayerMask EverythingExceptEnemy;
    public bool DebugMode = false;
    public TextMesh EnemyDebugObject;

    public EnemyState State { get; private set; }
    public bool InFOV { get; private set; }
    public RaycastHit Hit { get; private set; }
    private IAstarAI ai;
    private Transform target;
    private float lastTimeSighted = -420f;
    private float lastTimeLostTarget = -420f;
    private bool doPatrol = false;
    private float fov;
    private RaycastHit hit;

    void Start()
    {
        ai = GetComponent<IAstarAI>();
        target = FirstPersonController.instance.MainCamera.transform;
        State = EnemyState.Idle;

        EnemyDebugObject.gameObject.SetActive(DebugMode);
        fov = Mathf.InverseLerp(180, 0, FieldOfView);

        if (PatrollingRoute != null)
        {
            if (PatrollingRoute.Count > 0)
            { doPatrol = true; }
        }
    }

    void Update()
    {
        if (IsPlayerWithinFieldOfView())
        {
            if (RaycastToPlayer())
            {
                lastTimeSighted = Time.time;
                State = EnemyState.Chasing; //player spotted
            }
        }

        if (State == EnemyState.Chasing && Time.time > lastTimeSighted + ChaseTime)
        {
            lastTimeLostTarget = Time.time;
            State = EnemyState.LostTarget; //enemy lost the player
        }
        if (State == EnemyState.LostTarget && Time.time > lastTimeLostTarget + LostTargetTime)
        {
            State = EnemyState.Idle; //player got away, go back to normal
        }

        Move();
    }

    void Move()
    {
        if(State == EnemyState.Chasing)
        {
            ai.destination = target.position;
        }
        if(State == EnemyState.Idle)
        {
            ai.destination = transform.position;
        }
        //add patrolling / sitting spots here
    }

    bool IsPlayerWithinFieldOfView()
    {
        InFOV = Vector3.Dot(transform.TransformDirection(Vector3.forward), (target.position - transform.position)) >= fov;
        return InFOV;
    }

    bool RaycastToPlayer()
    {
        if(Physics.Raycast(eyes.position, (target.position - eyes.position), out hit, SightDistance, EverythingExceptEnemy, QueryTriggerInteraction.Collide))
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

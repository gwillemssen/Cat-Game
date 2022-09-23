using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

[RequireComponent(typeof(IAstarAI))]
public class Enemy : MonoBehaviour
{
    public enum State { Idle, Chasing, LostTarget }
    public Transform eyes;
    public float SightDistance = 12f;
    public float ChaseTime = 6f;
    public LayerMask EverythingExceptEnemy;

    private State state;
    private IAstarAI ai;
    private Transform target;
    private float lastTimeSighted = -420f;

    void Start()
    {
        ai = GetComponent<IAstarAI>();
        target = FirstPersonController.instance.transform;
        state = State.Idle;
    }

    void Update()
    {
        if(Vector3.Dot(transform.TransformDirection(Vector3.forward), (target.position - transform.position)) >= 0)
        {
            //in front
            RaycastHit hit;
            if (Physics.Raycast(eyes.position, eyes.transform.forward, out hit, SightDistance, EverythingExceptEnemy, QueryTriggerInteraction.Collide))
            {
                if(hit.collider.CompareTag("Player"))
                {
                    lastTimeSighted = Time.time;
                }
            }
        }
        if(Time.time - lastTimeSighted < ChaseTime)
        {
            ai.destination = target.position;
        }
        else
        {
            Debug.Log("test test");
        }
    }
}

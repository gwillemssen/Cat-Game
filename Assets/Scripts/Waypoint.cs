using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    [Tooltip("How long should the AI stop here before moving on to the next Waypoint")]
    public float StopTime = 0f;

    [SerializeField] private AudioClip[] goingToVoicelines;
    [SerializeField] private AudioClip[] arrivedVoicelines;

    [HideInInspector] public NonRepeatingSound RandomGoingToVoicelines { get; private set; }
    [HideInInspector] public NonRepeatingSound RandomArrivedVoicelines { get; private set; }

    private void Start()
    {
        if(goingToVoicelines == null || goingToVoicelines != null && goingToVoicelines.Length == 0)
        { return; }
        if (arrivedVoicelines == null || arrivedVoicelines != null && arrivedVoicelines.Length == 0)
        { return; }

        RandomGoingToVoicelines = new NonRepeatingSound(goingToVoicelines);
        RandomArrivedVoicelines = new NonRepeatingSound(arrivedVoicelines);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 1, 0, 0.75F);
        Gizmos.DrawSphere(transform.position, 0.5f);
    }
}

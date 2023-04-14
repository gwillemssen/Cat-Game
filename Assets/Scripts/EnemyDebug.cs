using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(TextMesh))]
public class EnemyDebug : MonoBehaviour
{
    [HideInInspector]
    public Enemy Enemy;
    private TextMesh outputText;
    private float val;
    private Vector3 lineDir;
    private float alertnessPercentage;

    private void OnEnable()
    {
        Enemy = transform.GetComponentInParent<Enemy>();
        outputText = GetComponent<TextMesh>();
    }

    string GetDebugStatus()
    {
        string output = "";
        output += $"{Enemy.State.ToString()}\n" +
            $"PercentVisible : {Enemy.PercentVisible}\n" +
            $"AtDestination : {Enemy.AtDestination} | isStopped : {Enemy.AI.isStopped}";
        if(Enemy.State == Enemy.PatrollingState)
        {
            output += "Awareness: " + Enemy.PatrollingState.AwarenessValue + "\n";
            output += "GoingToWaypoint: " + Enemy.PatrollingState.GoingToWaypoint + "\n";
        }
        return output;
    }

    void FixedUpdate()
    {
        if (!Enemy.DebugMode)
            return;

        outputText.text = GetDebugStatus();
    }
}

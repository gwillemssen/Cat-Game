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
        alertnessPercentage = Mathf.Lerp(0f, 100f, (Enemy.Awareness / 1f));
        string alertnessBar = "";
        for (int i = 0; i < 20; i++)
        {
            if(alertnessPercentage >= 5f * i)
            { alertnessBar += "|"; }
        }
        return $"{Enemy.State.ToString()}\n" +
            $"Sees Player: {Enemy.SeesPlayer}\n" +
            $"Awareness : {alertnessBar}\n" +
            $"PercentVisible : {Enemy.PercentVisible}\n" +
            $"ArrivedAtDestination : {Enemy.ArrivedAtDestinationOrStuck}";
    }

    void FixedUpdate()
    {
        if (!Enemy.DebugMode)
            return;

        outputText.text = GetDebugStatus();
    }
}

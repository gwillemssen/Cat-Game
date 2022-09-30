using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EnemyDebug : MonoBehaviour
{
    [HideInInspector]
    public Enemy Enemy;
    private TextMesh outputText;
    private float val;
    private Vector3 lineDir;
    private string alertnessBar;
    private float alertnessPercentage;

    private void OnEnable()
    {
        Enemy = transform.GetComponentInParent<Enemy>();
        outputText = Enemy.EnemyDebugObject;
    }

    string GetDebugStatus()
    {
        alertnessPercentage = Mathf.Lerp(0f, 100f, (Enemy.Alertness / Enemy.AlertnessRequired));
        alertnessBar = "";
        for (int i = 0; i < 20; i++)
        {
            if(alertnessPercentage >= 5f * i)
            { alertnessBar += "|"; }
        }
        return $"{Enemy.State.ToString()}\nIn FOV: {Enemy.InFOV}\nALERTNESS: {alertnessBar}";
    }

    void Update()
    {
        if (!Enemy.DebugMode)
            return;

        outputText.text = GetDebugStatus();

        Debug.DrawLine(Enemy.eyes.position, Enemy.eyes.position + Enemy.eyes.transform.forward * 2f, Color.white);
        Debug.DrawLine(Enemy.eyes.position, Enemy.Hit.point, Enemy.State == Enemy.EnemyState.Chasing ? Color.red : Color.green);
    }
}

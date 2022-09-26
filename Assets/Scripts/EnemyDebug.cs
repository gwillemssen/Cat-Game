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

    private void OnEnable()
    {
        Enemy = transform.GetComponentInParent<Enemy>();
        outputText = Enemy.EnemyDebugObject;
    }

    string GetDebugStatus()
    {
        return $"{Enemy.State.ToString()}\nIn FOV: {Enemy.InFOV}";
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

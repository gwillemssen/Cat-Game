using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakePosition : MonoBehaviour
{
    public float Amplitude = 1f;
    public float Frequency = 1f;
    public float Elasticity = 1f;

    private float lastTimeShake = -420f;
    private Vector3 startPos;
    private bool shake = false;

    private void Start()
    {
        Shake();
    }

    public void Shake()
    {
        shake = true;
        startPos = transform.position;
    }

    public void Stop()
    {
        transform.position = startPos;
        shake = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!shake)
            return;
        if(Time.time > lastTimeShake + (Frequency / 1f))
        {
            transform.position += Random.insideUnitSphere * Amplitude;
            lastTimeShake = Time.time;
        }

        transform.position = Vector3.Lerp(transform.position, startPos, Time.deltaTime * Elasticity);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpScript : MonoBehaviour
{
    [HideInInspector]
    public float floatVal;
    public Vector3 vecVal;
    public Color colorVal;
    
    public float floatTarget;
    public Vector3 vecTarget;
    public Color colorTarget;
    
    public float lerpSpeed = 2;

    public enum LerpTiming
    {
        FixedUpdate = 0,
        Update = 1,
        LateUpdate = 2
    }
    public enum LerpType
    {
        Float = 0,
        Vector3 = 1,
        Color = 2
    }

    public LerpType typeOfLerp;
    public LerpTiming whenToLerp;

    void Update()
    {
        if (floatVal != floatTarget && whenToLerp == LerpTiming.Update)
        {
            Count();
        }
    }

    private void LateUpdate()
    {
        if (floatVal != floatTarget && whenToLerp == LerpTiming.LateUpdate)
        {
            Count();
        }
    }

    private void FixedUpdate()
    {
        if (floatVal != floatTarget && whenToLerp == LerpTiming.FixedUpdate)
        {
            Count();
        }
    }

    void Count()
    {
        if (typeOfLerp == LerpType.Float)
        {
            float delta = floatTarget - floatVal;
            delta *= Time.deltaTime * lerpSpeed;
            floatVal += delta;

            if (Mathf.Abs(floatTarget - floatVal) < 0.01f)
            {
                floatVal = floatTarget;
            }
        }

        if (typeOfLerp == LerpType.Vector3)
        {
            float deltaX = vecTarget.x - vecVal.x;
            float deltaY = vecTarget.y - vecVal.y;
            float deltaZ = vecTarget.z - vecVal.z;

            deltaX *= Time.deltaTime * lerpSpeed;
            deltaY *= Time.deltaTime * lerpSpeed;
            deltaZ *= Time.deltaTime * lerpSpeed;

            vecVal.x += deltaX;
            vecVal.y += deltaY;
            vecVal.z += deltaZ;


            if (Mathf.Abs(vecTarget.x - vecVal.x) < 0.01f)
            {
                vecVal.x = vecTarget.x;
            }
            if (Mathf.Abs(vecTarget.y - vecVal.y) < 0.01f)
            {
                vecVal.y = vecTarget.y;
            }
            if (Mathf.Abs(vecTarget.z - vecVal.z) < 0.01f)
            {
                vecVal.z = vecTarget.z;
            }
        }
        
        if (typeOfLerp == LerpType.Color)
        {
            float deltaR = colorTarget.r - colorVal.r;
            float deltaG = colorTarget.g - colorVal.g;
            float deltaB = colorTarget.b - colorVal.b;
            float deltaA = colorTarget.a - colorVal.a;

            deltaR *= Time.deltaTime * lerpSpeed;
            deltaG *= Time.deltaTime * lerpSpeed;
            deltaB *= Time.deltaTime * lerpSpeed;
            deltaA *= Time.deltaTime * lerpSpeed;

            colorVal.r += deltaR;
            colorVal.g += deltaG;
            colorVal.b += deltaB;
            colorVal.a += deltaA;

            if (Mathf.Abs(colorTarget.r - colorVal.r) < 0.01f)
            {
                colorVal.r = colorTarget.r;
            }
            if (Mathf.Abs(colorTarget.g - colorVal.g) < 0.01f)
            {
                colorVal.g = colorTarget.g;
            }
            if (Mathf.Abs(colorTarget.b - colorVal.b) < 0.01f)
            {
                colorVal.b = colorTarget.b;
            }
            if (Mathf.Abs(colorTarget.a - colorVal.a) < 0.01f)
            {
                colorVal.a = colorTarget.a;
            }
        }
    }
}

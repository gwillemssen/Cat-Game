using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LerpScript2 : MonoBehaviour
{

    public List<float> floatValues;
    public List<Vector3> vectorValues;
    public List<Color> colorValues;
    

    public List<float> floatTargets;
    public List<Vector3> vectorTargets;
    public List <Color> colorTargets;
    
    [HideInInspector]
    public float lerpSpeed = 1;

    private bool initialized;

    public enum LerpTiming
    {
        FixedUpdate = 0,
        Update = 1,
        LateUpdate = 2
    }

    private bool doVectors;
    private bool doFloats;
    private bool doColors;
    

    [HideInInspector]
    public LerpTiming whenToLerp;

    void Update()
    {
        if (whenToLerp == LerpTiming.Update)
        {
            Count();
        }
    }

    private void LateUpdate()
    {
        if (whenToLerp == LerpTiming.LateUpdate)
        {
            Count();
        }
    }

    private void FixedUpdate()
    {
        if (whenToLerp == LerpTiming.FixedUpdate)
        {
            Count();
        }
    }

    void Count()
    {
        if (initialized)
        { 
            if (doFloats)
            {
                if (floatValues[0] != null && floatTargets[0] != null)
                {
                    for (int i = 0; i < floatValues.Count; i++)
                    {
                        if (floatValues[i] != floatTargets[i])
                        {
                            float delta = floatTargets[i] - floatValues[i];
                            delta *= Time.deltaTime * lerpSpeed;
                            floatValues[i] += delta;
                        }
                        if (Mathf.Abs(floatTargets[i] - floatValues[i]) < 0.01f)
                        {
                            floatValues[i] = floatTargets[i];
                        }
                    }
                }
            }

            else if (doVectors)
        {
            if (vectorValues[0] != null && vectorTargets[0] != null)
            {
                for (int i = 0; i < vectorValues.Count; i++)
                {
                    Vector3 tempVal = vectorValues[i];
                    if (vectorValues[i] != vectorTargets[i])
                    {
                        Vector3 delta = vectorTargets[i] - vectorValues[i];
                        delta *= Time.deltaTime * lerpSpeed;
                        vectorValues[i] += delta;
                    }
                    if (Mathf.Abs(vectorTargets[i].x - vectorValues[i].x) < 0.01f)
                    {
                        tempVal.x = vectorTargets[i].x;
                    }
                    if (Mathf.Abs(vectorTargets[i].y - vectorValues[i].y) < 0.01f)
                    {
                        tempVal.y = vectorTargets[i].y;
                    }
                    if (Mathf.Abs(vectorTargets[i].z - vectorValues[i].z) < 0.01f)
                    {
                        tempVal.z = vectorTargets[i].z;
                    }
                    if (vectorValues[i] != tempVal)
                    {
                        vectorValues[i] = tempVal;
                    }
                }
            }
            else
            {
                Debug.Log("No vector values found even though that type is set.");
            }
            
        }
        
            else if (doColors)
        {
            if (colorValues[0] != null && colorTargets[0] != null)
            {
                for (int i = 0; i < colorValues.Count; i++)
                {
                    Vector4 delta = colorTargets[i] - colorValues[i];

                    delta *= Time.deltaTime * lerpSpeed;

                    Vector4 tempVal = colorValues[i];
                    tempVal += delta;

                    if (Mathf.Abs(colorTargets[i].r - colorValues[i].r) < 0.01f)
                    {
                        tempVal.w = colorTargets[i].r;
                    }
                    if (Mathf.Abs(colorTargets[i].g - colorValues[i].g) < 0.01f)
                    {
                        tempVal.x = colorTargets[i].g;
                    }
                    if (Mathf.Abs(colorTargets[i].b - colorValues[i].b) < 0.01f)
                    {
                        tempVal.y = colorTargets[i].b;
                    }
                    if (Mathf.Abs(colorTargets[i].a - colorValues[i].a) < 0.01f)
                    {
                        tempVal.z = colorTargets[i].a;
                    }

                    Color tempColor = tempVal;
                    if (tempColor != colorValues[i])
                    {
                        colorValues[i] = tempVal;
                    }
                }
            }
            else
            {
                Debug.Log("No color values found even though that type is set.");
            }
        }
        }
    }
    public void Initialize(Vector3 vector3val, Vector3 vector3tgt)
    {
        vectorTargets.Add(vector3tgt);
        vectorValues.Add(vector3val);

        doVectors = true;

        initialized = true;
    }
    public void Initialize(List<Color> colorVals, List<Color> colortgts)
    {
        colorValues = colorVals;
        colorTargets = colortgts;
        
        doColors = true;

        initialized = true;
    }
    public void Initialize(List<Vector3> vector3vals, List<Vector3> vector3tgts)
    {
        vectorTargets = vector3tgts;
        vectorValues = vector3vals;

        doVectors = true;

        initialized = true;
    }
    public void Initialize(List<float> floatVals, List<float> floattgts)
    {
        floatValues = floatVals;
        floatTargets = floattgts;
        
        doFloats = true;
    }
    public void Initialize(List<float> floatVals, List<float> floattgts, List<Vector3> vector3vals, List<Vector3> vector3tgts, List<Color> color, List<Color> colortgts)
    {
        floatValues = floatVals;
        vectorValues = vector3vals;
        colorValues = color;
        floatTargets = floattgts;
        vectorTargets = vector3tgts;
        colorTargets = colortgts;

        doVectors = true;
        doFloats = true;
        doColors = true;

        initialized = true;
    }






}

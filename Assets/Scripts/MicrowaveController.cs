using System;
using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class MicrowaveController : Interactable
{
    public float currentNoiseEmitted;
    private Light light;
    private Material mat;

    public bool lightOn;
    private float targetLightVal;
    public float timeLeft;

    public bool timerOn = false;

    private TextMeshProUGUI timerText;

    private LerpScript LightLerp;

    public float testValue;

    // Start is called before the first frame update
    void Start()
    {
        LightLerp = this.AddComponent<LerpScript>();
        LightLerp.lerpSpeed = 16;
        mat = Instantiate(transform.GetChild(0).GetComponent<Renderer>().material);
        transform.GetChild(0).GetComponent<Renderer>().material = mat;
        light = transform.GetChild(2).GetComponent<Light>();
        timerText = transform.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>();
        
        timerOn = true;
    }

    // Update is called once per frame
    void Update()
    {
        LightManager();
        TimerManager();
    }

    void LightManager()
    {
        if (lightOn)
        {
            LightLerp.floatTarget = 1;
        }
        else
        {
            LightLerp.floatTarget = 0;
        }
        mat.SetFloat("_LightIntensity",LightLerp.floatVal);
        light.intensity = 0.09f * LightLerp.floatVal;
        testValue = LightLerp.floatVal;
    }

    void TimerManager()
    {
        if (timerOn)
        {
            if (timeLeft >= 0)
            {
                timeLeft -= Time.deltaTime;
                UpdateTimer(timeLeft);
            }
            else
            {
                {
                    Debug.Log("time up");
                    timeLeft = 0;
                    timerOn = false;
                }
            }
        }
    }
    
    void UpdateTimer(float currentTime)
    {
        currentTime += 1;

        float minutes = Mathf.FloorToInt(currentTime / 60);
        float seconds = Mathf.FloorToInt(currentTime % 60);

        timerText.text = string.Format("{0:00},{0:00}", seconds, minutes);
    }

    public override void InteractClick(FirstPersonController controller)
    {
        base.InteractClick(controller);
    }
}


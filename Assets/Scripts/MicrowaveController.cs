using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class MicrowaveController : Interactable
{
    public float currentNoiseEmitted;
    private Light light;
    private Material mat;
    private GameObject plate;
    private GameObject door;

    public bool lightOn;
    public bool plateSpinning;
    public bool doorOpen;

    private float targetLightVal;
    public float timeLeft;

    public bool timerOn = false;

    private TextMeshProUGUI timerText;

    private LerpScript LightLerp;
    private LerpScript plateLerp;
    private LerpScript doorLerp;
    
    // Start is called before the first frame update
    void Start()
    {
        door = transform.GetChild(1).GetChild(0).gameObject;
        doorLerp = this.AddComponent<LerpScript>();
        doorLerp.typeOfLerp = LerpScript.LerpType.Vector3;
        doorLerp.lerpSpeed = 8;
        plate = transform.GetChild(1).GetChild(1).gameObject;
        LightLerp = this.AddComponent<LerpScript>();
        LightLerp.lerpSpeed = 16;
        plateLerp = this.AddComponent<LerpScript>();
        
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
        // TimerManager();
        PlateManager();
        DoorManager();
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

        if (LightLerp.floatTarget != LightLerp.floatVal)
        {
            mat.SetFloat("_LightIntensity",LightLerp.floatVal);
            light.intensity = 0.09f * LightLerp.floatVal;
        }
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

    void PlateManager()
    {

            if (plateSpinning)
            {
                plateLerp.floatTarget = 0.15f;
            }
            else
            {
                plateLerp.floatTarget = 0;
            }

            if (plateLerp.floatTarget != plateLerp.floatVal)
            {
                plate.transform.Rotate(0, 0, plateLerp.floatTarget);
            }
    }

    void DoorManager()
    {
        if (doorOpen)
        {
            doorLerp.vecTarget = new Vector3(0, 0, 100); 
        }
        else
        {
            doorLerp.vecTarget = new Vector3(0, 0, 0);
        }

        if (doorLerp.vecVal != doorLerp.vecTarget)
        {
            door.transform.localRotation = Quaternion.Euler(doorLerp.vecVal);
        }
    }

    public override void InteractClick(FirstPersonController controller)
    {
        base.InteractClick(controller);
    }
}


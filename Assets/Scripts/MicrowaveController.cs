using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using Random = UnityEngine.Random;


public class MicrowaveController : MonoBehaviour
{
    public float currentNoiseEmitted;
    private Light light;
    private Material mat;

    public bool lightOn;
    public float timeLeft;

    public bool timerOn = false;
    public bool doorOpen;
    public bool plateSpinning;

    public TextMeshProUGUI timerText;
    public GameObject ball;
    
    private lerp

    // Start is called before the first frame update
    void Start()
    {
        mat = Instantiate(this.GetComponent<Renderer>().material);
        this.GetComponent<Renderer>().material = mat;
        light = transform.GetChild(2).GetComponent<Light>();

        timerOn = true;
    }

    // Update is called once per frame
    void Update()
    {
        LightManager();
        PlateManager();
        DoorManager();
        TimerManager();
    }

    void LightManager()
    {
        if (lightOn)
        {
            targetLightVal = 1;
        }
        else
        {
            targetLightVal = 0;
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
                    timerText.enabled = false;
                }
            }
        }
    }
    
    void UpdateTimer(float currentTime)
    {
        currentTime += 1;

        float minutes = Mathf.FloorToInt(currentTime / 60);
        float seconds = Mathf.FloorToInt(currentTime % 60);

        timerText.text = string.Format("{0:00} : {1:00}", minutes, seconds);
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
        ActivateMicrowave();
    }

    void ActivateMicrowave()
    {
        lightOn = true;
        plateSpinning = true;
        timeLeft = Random.Range(60, 120);
        timerOn = true;
    }
}


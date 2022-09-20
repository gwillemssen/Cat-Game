using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MicrowaveController : MonoBehaviour
{
    public float currentNoiseEmitted;
    private Light light;
    private Material mat;

    public bool lightOn;
    private float targetLightVal;
    private float currentLightVal;
    public float timeLeft;

    public bool timerOn = false;

    public TextMeshProUGUI timerText;
    public GameObject ball;

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
                    Instantiate(ball, new Vector3(0, 2.53f, 0), Quaternion.Euler(0, 0, 0));
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
}


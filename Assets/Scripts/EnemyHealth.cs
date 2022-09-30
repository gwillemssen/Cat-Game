using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Enemy health is regarded as the stealth meter, my references have it as enemy health 
/// Reference from DapperDino: https://www.youtube.com/watch?v=ZYeXmze5gxg
/// </summary>
public class EnemyHealth : MonoBehaviour
{
    public float meter;
    public float maxMeter;
    public Slider slider;
    public GameObject enemyMeterUI;

    void Start()
    {
        meter = maxMeter;
        slider.value = CalculateMeter();

    }
     void Update()
    {
        slider.value = CalculateMeter();
        if (meter<maxMeter)
        {
            enemyMeterUI.SetActive(true);
        }
        if (meter<=0)
        {
            //Activate the script that lets the enemy follow the player
        }

        if (meter > maxMeter)
        {
            meter = maxMeter;
        }
    }

    float CalculateMeter()
    {
        return meter / maxMeter;
    }

}

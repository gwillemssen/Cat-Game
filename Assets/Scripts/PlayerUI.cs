using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [HideInInspector]
    public Slider PettingMeter;
    private Slider noiseMeter;

    private Canvas canvas;

    public void Init(FirstPersonController controller)
    {
        GameObject g = Resources.Load("Prefab/FpsCam") as GameObject;
        g = Instantiate(g);
        Debug.Log(g.name);
        if (g != null) canvas = g.GetComponentInChildren<Canvas>();
        foreach(Slider s in canvas.GameObject().GetComponentsInChildren<Slider>())
        {
            if(s.gameObject.name == "NoiseMeter")
            { noiseMeter = s; }
            if (s.gameObject.name == "PettingMeter")
            { PettingMeter = s; }
        }
        PettingMeter.gameObject.SetActive(false);
        Debug.Log("petmeter set to false");
        noiseMeter.gameObject.SetActive(false);
    }

    //private void Update()
    //{
    //    if(LevelManager.instance.Noise == 0)
    //    { noiseMeter.gameObject.SetActive(false); }
    //    else
    //    {
    //        noiseMeter.gameObject.SetActive(true);
    //        noiseMeter.value = LevelManager.instance.Noise / LevelManager.instance.MaxNoise;
    //    }
    //}
}

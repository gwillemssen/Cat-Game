using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [Header("References")]
    public GameObject FPSCamPrefab;
    [HideInInspector]
    public Slider PettingMeter;
    private Slider noiseMeter;

    private Canvas canvas;

    public void Init(FirstPersonController controller)
    {
        canvas = Instantiate<GameObject>(FPSCamPrefab, Vector3.down * 100f, Quaternion.identity).GetComponentInChildren<Canvas>();
        foreach(Slider s in canvas.GetComponentsInChildren<Slider>())
        {
            if(s.gameObject.name == "NoiseMeter")
            { noiseMeter = s; }
            if (s.gameObject.name == "PettingMeter")
            { PettingMeter = s; }
        }
        PettingMeter.gameObject.SetActive(false);
        noiseMeter.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (LevelManager.instance.Noise == 0)
        { noiseMeter.gameObject.SetActive(false); }
        else
        {
            noiseMeter.gameObject.SetActive(true);
            noiseMeter.value = LevelManager.instance.Noise / LevelManager.instance.MaxNoise;
        }
    }
}

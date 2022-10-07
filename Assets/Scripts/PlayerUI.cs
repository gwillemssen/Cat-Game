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
    private Text debugText;

    private Canvas canvas;

    public void Init(FirstPersonController controller)
    {
        canvas = Instantiate<GameObject>(FPSCamPrefab, Vector3.down * 100f, Quaternion.identity).GetComponentInChildren<Canvas>();
        foreach(RectTransform r in canvas.GetComponentsInChildren<RectTransform>())
        {
            switch(r.gameObject.name)
            {
                case "NoiseMeter":
                    noiseMeter = r.GetComponent<Slider>();
                    break;
                case "PettingMeter":
                    PettingMeter = r.GetComponent<Slider>(); ;
                    break;
                case "DebugText":
                    debugText = r.GetComponent<Text>();
                    break;
            }
        }
        PettingMeter.gameObject.SetActive(false);
        noiseMeter.gameObject.SetActive(false);
        //debugText.enabled = false;
        debugText.text = "";
    }

    string debugOutput = "";
    private void Update()
    {
        if(LevelManager.instance.Noise == 0)
        { noiseMeter.gameObject.SetActive(false); }
        else
        {
            noiseMeter.gameObject.SetActive(true);
            noiseMeter.value = LevelManager.instance.Noise / LevelManager.instance.MaxNoise;
        }

        debugOutput = "";
        debugOutput += $"Total Noise : {LevelManager.instance.Noise}";
        debugOutput += $"Most Alert Enemy State : {LevelManager.instance.MostAlertEnemyState}";
        debugOutput += $"CATS : {LevelManager.instance.CatsPetted} / {LevelManager.instance.CatsToPet}";
        debugText.text = debugOutput;
    }
}

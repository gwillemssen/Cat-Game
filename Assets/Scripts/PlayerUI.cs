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
    private Text infoText;
    private Animation infoTextFade;
    public GameObject WinScreen { get; private set; }
    public GameObject LoseScreen { get; private set; }

    private Canvas canvas;

    public void Init(FirstPersonController controller)
    {
        FPSCamPrefab = Resources.Load("Prefab/FpsCam") as GameObject;
        
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
                case "WIN":
                    WinScreen = r.gameObject;
                    break;
                case "LOSE":
                    LoseScreen = r.gameObject;
                    break;
                case "INFO":
                    infoText = g.GetComponent<Text>();
                    infoTextFade = g.GetComponent<Animation>();
                    break;
            }
        }
        PettingMeter.gameObject.SetActive(false);
        noiseMeter.gameObject.SetActive(false);
        LoseScreen.SetActive(false);
        WinScreen.SetActive(false);
        //debugText.enabled = false;
        debugText.text = "";
    }

    string debugOutput = "";
    private void Update()
    {
        //If the noise is 0 UI disappears
        if(LevelManager.instance.Noise == 0)
        { noiseMeter.gameObject.SetActive(false); }
        else
        {
            noiseMeter.gameObject.SetActive(true);
            noiseMeter.value = LevelManager.instance.Noise / LevelManager.instance.MaxNoise;
        }

        debugOutput = "";
        //debugOutput += $"Noise : {(int)LevelManager.instance.Noise} / {LevelManager.instance.MaxNoise}\n";
        //debugOutput += $"Most Alert Enemy State : {LevelManager.instance.MostAlertEnemyState}\n";
        debugOutput += $"CATS : {LevelManager.instance.CatsPetted} / {LevelManager.instance.CatsToPet}\n";
        debugText.text = debugOutput;
    }

    public void SetInfoText(string text)
    {
        infoTextFade.Stop();
        infoTextFade.Play();
        infoText.color = Color.white;
        infoText.text = text;
    }
}

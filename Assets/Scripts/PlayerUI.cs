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
    public GameObject WinScreen { get; private set; }
    public GameObject LoseScreen { get; private set; }

    private Canvas canvas;

    public void Init(FirstPersonController controller)
    {
        canvas = Instantiate<GameObject>(FPSCamPrefab, Vector3.down * 100f, Quaternion.identity).GetComponentInChildren<Canvas>();
        Transform g;
        for(int i = 0; i < canvas.transform.childCount; i++)
        {
            g = canvas.transform.GetChild(i);

            switch(g.gameObject.name)
            {
                case "NoiseMeter":
                    noiseMeter = g.GetComponent<Slider>();
                    break;
                case "PettingMeter":
                    PettingMeter = g.GetComponent<Slider>();
                    break;
                case "DebugText":
                    debugText = g.GetComponent<Text>();
                    break;
                case "WIN":
                    WinScreen = g.gameObject;
                    break;
                case "LOSE":
                    LoseScreen = g.gameObject;
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
        debugOutput += $"Noise : {(int)LevelManager.instance.Noise} / {LevelManager.instance.MaxNoise}\n";
        debugOutput += $"Most Alert Enemy State : {LevelManager.instance.MostAlertEnemyState}\n";
        debugOutput += $"CATS : {LevelManager.instance.CatsPetted} / {LevelManager.instance.CatsToPet}\n";
        debugText.text = debugOutput;
    }
}

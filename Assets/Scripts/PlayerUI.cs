using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    //NOTE: 
    //this class SHOULD be responsible for getting values from the player / gamemanager, ect.
    //Right now, the seperation of concern is bad - other classes access this to set UI things, which is wrong
    //can refactor later

    public static PlayerUI instance;

    public enum EyeState { Closed, Half, Open}

    [Header("References")]
    public GameObject FPSCamPrefab;
    [HideInInspector]
    public Slider PettingMeter;
    private Slider noiseMeter;
    private Slider throwStrengthMeter;
    private Image timeUI;
    private GameObject catTimerUI;
    private Image catStandingUI;
    private Image catCrouchUI;
    private Image eyeOpenUI;
    private Image eyeHalfUI;
    private Image eyeClosedUI;
    private Image speakerUI;
    private Text debugText;
    private Text infoText;
    private Animation infoTextFade;
    public GameObject WinScreen { get; private set; }
    public GameObject LoseScreen { get; private set; }
    public GameObject LoseCopsScreen { get; private set; }

    private float lastTimeShownSpeaker = -420f;
    string debugOutput = "";
    Color speakerColor;
    private Canvas canvas;

    private void Awake()
    {
        if(instance != null)
        { 
            Destroy(this);
            return;
        }
        instance = this;
    }

    public void Init(FirstPersonController controller)
    {
        FPSCamPrefab = Resources.Load("Prefab/FpsCam") as GameObject;
        
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
                case "INFO":
                    infoText = g.GetComponent<Text>();
                    infoTextFade = g.GetComponent<Animation>();
                    break;
                case "ThrowStrengthMeter":
                    throwStrengthMeter = g.GetComponent<Slider>();
                    break;
                case "CatTimer":
                    catTimerUI = g.gameObject;
                    timeUI = g.GetChild(3).GetComponent<Image>();
                    break;
                case "LOSECops":
                    LoseCopsScreen = g.gameObject;
                    break;
                case "CrouchUI":
                    catStandingUI = g.GetChild(0).GetComponent<Image>();
                    catCrouchUI = g.GetChild(1).GetComponent<Image>();
                    break;
                case "EyeUI":
                    eyeOpenUI = g.GetChild(0).GetComponent<Image>();
                    eyeHalfUI = g.GetChild(1).GetComponent<Image>();
                    eyeClosedUI = g.GetChild(2).GetComponent<Image>();
                    break;
                case "Speaker":
                    speakerUI = g.GetComponent<Image>();
                    break;
            }
        }
        PettingMeter.gameObject.SetActive(false);
        LoseScreen.SetActive(false);
        WinScreen.SetActive(false);
        LoseCopsScreen.SetActive(false);
        catTimerUI.gameObject.SetActive(false);
        throwStrengthMeter.gameObject.SetActive(false);
        speakerColor = new Color(1, 1, 1, 0);
        speakerUI.color = speakerColor;
        SetCrouchUI(false);
        //debugText.enabled = false;
        debugText.text = "";
    }


    private void Update()
    {
        noiseMeter.value = LevelManager.instance.Noise / LevelManager.instance.MaxNoise;

        debugOutput = "";
        //debugOutput += $"Noise : {(int)LevelManager.instance.Noise} / {LevelManager.instance.MaxNoise}\n";
        //debugOutput += $"Most Alert Enemy State : {LevelManager.instance.MostAlertEnemyState}\n";
        debugOutput += $"CATS : {LevelManager.instance.CatsPetted} / {LevelManager.instance.CatsToPet}\n";
        debugText.text = debugOutput;

        speakerColor.a = Mathf.Lerp(1f, 0f, Mathf.Clamp01((Time.time - lastTimeShownSpeaker)));
        speakerUI.color = speakerColor;
    }

    public void SetInfoText(string text)
    {
        infoTextFade.Stop();
        infoTextFade.Play();
        infoText.color = Color.white;
        infoText.text = text;
    }

    public void SetEyeballUI(EyeState eyeState)
    {
        eyeOpenUI.enabled = eyeClosedUI.enabled = eyeHalfUI.enabled = false;

        eyeOpenUI.enabled = (eyeState == EyeState.Open);
        eyeHalfUI.enabled = (eyeState == EyeState.Half);
        eyeClosedUI.enabled = (eyeState == EyeState.Closed);
    }

    public void SetCrouchUI(bool crouching)
    {
        catCrouchUI.enabled = crouching;
        catStandingUI.enabled = !crouching;
    }

    public void SetThrowStrengthMeter(float p)
    {
        throwStrengthMeter.gameObject.SetActive(p > 0.15f);
        throwStrengthMeter.value = p;
    }

    public void SetTimeUI(float p)
    {
        timeUI.fillAmount = p;
        catTimerUI.gameObject.SetActive(p != 0f);
    }

    public void ShowSpeaker()
    {
        speakerUI.enabled = true;
        lastTimeShownSpeaker = Time.time;
    }

}

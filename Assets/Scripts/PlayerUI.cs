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

    public enum EyeState { Closed, Half, Open }

    public Canvas Canvas;

    [HideInInspector]
    public Slider PettingMeter;
    private Slider throwStrengthMeter;
    private Image timeUI;
    private GameObject catTimerUI;
    private Image eyeOpenUI;
    private Image eyeHalfUI;
    private Image eyeClosedUI;
    private Image speakerUI;
    private Text debugText;
    private Text infoText;
    private Animation catTimerAnim;
    private Animation infoTextFade;
    private Image hamd;
    private GameObject spottedGradient_Left;
    private GameObject spottedGradient_Right;
    public GameObject WinScreen { get; private set; }
    public GameObject LoseScreen { get; private set; }
    public GameObject LoseCopsScreen { get; private set; }
    public Image Hamd { get { return hamd; } private set { hamd = value; } }
    public bool EnemyOnScreen { get; private set; }

    private float lastTimeShownSpeaker = -420f;
    string debugOutput = "";
    Color speakerColor;

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
        Transform t;
        for(int i = 0; i < Canvas.transform.childCount; i++)
        {
            t = Canvas.transform.GetChild(i);

            switch(t.gameObject.name)
            {
                case "PettingMeter":
                    PettingMeter = t.GetComponent<Slider>();
                    break;
                case "DebugText":
                    debugText = t.GetComponent<Text>();
                    break;
                case "WIN":
                    WinScreen = t.gameObject;
                    break;
                case "LOSE":
                    LoseScreen = t.gameObject;
                    break;
                case "INFO":
                    infoText = t.GetComponent<Text>();
                    infoTextFade = t.GetComponent<Animation>();
                    break;
                case "ThrowStrengthMeter":
                    throwStrengthMeter = t.GetComponent<Slider>();
                    break;
                case "CatTimer":
                    catTimerUI = t.gameObject;
                    timeUI = t.GetChild(3).GetComponent<Image>();
                    catTimerAnim = catTimerUI.GetComponent<Animation>();
                    break;
                case "LOSECops":
                    LoseCopsScreen = t.gameObject;
                    break;
                case "EyeUI":
                    eyeOpenUI = t.GetChild(0).GetComponent<Image>();
                    eyeHalfUI = t.GetChild(1).GetComponent<Image>();
                    eyeClosedUI = t.GetChild(2).GetComponent<Image>();
                    break;
                case "Speaker":
                    speakerUI = t.GetComponent<Image>();
                    break;
                case "Hamd":
                    hamd = t.GetComponent<Image>();
                    break;
                case "SpottedGradient_Left":
                    spottedGradient_Left = t.gameObject;
                    break;
                case "SpottedGradient_Right":
                    spottedGradient_Right = t.gameObject;
                    break;
            }
        }
        PettingMeter.gameObject.SetActive(false);
        LoseScreen.SetActive(false);
        WinScreen.SetActive(false);
        LoseCopsScreen.SetActive(false);
        catTimerUI.gameObject.SetActive(false);
        throwStrengthMeter.gameObject.SetActive(false);
        hamd.enabled = false;
        speakerColor = new Color(1, 1, 1, 0);
        speakerUI.color = speakerColor;
        //debugText.enabled = false;
        debugText.text = "";
        spottedGradient_Left.SetActive(false);
        spottedGradient_Right.SetActive(false);
    }


    private void Update()
    {
        debugOutput = "";
        //debugOutput += $"Noise : {(int)LevelManager.instance.Noise} / {LevelManager.instance.MaxNoise}\n";
        //debugOutput += $"Most Alert Enemy State : {LevelManager.instance.MostAlertEnemyState}\n";
        debugOutput += $"CATS : {LevelManager.instance.CatsPetted} / {LevelManager.instance.CatsToPet}\n";
        debugText.text = debugOutput;

        speakerColor.a = Mathf.Lerp(1f, 0f, Mathf.Clamp01((Time.time - lastTimeShownSpeaker)));
        speakerUI.color = speakerColor;
    }

    public void SetSpottedGradient(bool enable, Vector3 pos)
    {
        if(!enable)
        { return; }
        EnemyOnScreen = Vector3.Dot(transform.TransformDirection(Vector3.forward), (pos - transform.position)) >= .65;
        //enable both if enemy is on the screen
        bool onRightSide = Vector3.Cross(transform.TransformDirection(Vector3.forward), (pos - transform.position)).y > 0;

        spottedGradient_Left.SetActive(enable && EnemyOnScreen || enable && !onRightSide);
        spottedGradient_Right.SetActive(enable && EnemyOnScreen || enable && onRightSide);
    }

    public void SetInfoText(string text) { SetInfoText(text, 64, true); }
    public void SetInfoText(string text, int size) { SetInfoText(text, size, true); }
    public void SetInfoText(string text, int size, bool italics)
    {
        infoTextFade.Stop();
        infoTextFade.Play();
        infoText.color = Color.white;
        infoText.text = text;
        infoText.fontSize = size;
        infoText.fontStyle = italics ? FontStyle.Italic : FontStyle.Bold;
    }

    public void SetEyeballUI(EyeState eyeState)
    {
        eyeOpenUI.enabled = eyeClosedUI.enabled = eyeHalfUI.enabled = false;

        eyeOpenUI.enabled = (eyeState == EyeState.Open);
        eyeHalfUI.enabled = (eyeState == EyeState.Half);
        eyeClosedUI.enabled = (eyeState == EyeState.Closed);
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
        if(!catTimerAnim.isPlaying)
        { catTimerAnim.Play(); }
    }

    public void ShowSpeaker()
    {
        speakerUI.enabled = true;
        lastTimeShownSpeaker = Time.time;
    }

}

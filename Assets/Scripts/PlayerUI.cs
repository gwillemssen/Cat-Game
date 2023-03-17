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

    [HideInInspector] public Slider PettingMeter;
    private Slider throwStrengthMeter;
    private Image timeUI;
    private GameObject catTimerUI;
    private Image eyeOpenUI;
    private Image eyeHalfUI;
    private Image eyeClosedUI;
    private Text debugText;
    private Text infoText;
    private Animation catTimerAnim;
    private Animation infoTextFade;
    private Image hamd;
    private GameObject spottedGradient_Left;
    private GameObject spottedGradient_Right;
    private Image grannyScreenSpaceUI;
    private Image grannyScreenSpaceUI_Fill;
    public GameObject WinScreen { get; private set; }
    public GameObject LoseScreen { get; private set; }
    public GameObject LoseCopsScreen { get; private set; }
    public Image Hamd { get { return hamd; } private set { hamd = value; } }
    public bool EnemyOnScreen { get; private set; }

    string debugOutput = "";

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
                case "Hamd":
                    hamd = t.GetComponent<Image>();
                    break;
                case "SpottedGradient_Left":
                    spottedGradient_Left = t.gameObject;
                    break;
                case "SpottedGradient_Right":
                    spottedGradient_Right = t.gameObject;
                    break;
                case "GrannyScreenSpaceUI":
                    grannyScreenSpaceUI = t.GetComponent<Image>();
                    grannyScreenSpaceUI_Fill = t.GetChild(0).GetComponent<Image>();
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
        //debugText.enabled = false;
        debugText.text = "";
        spottedGradient_Left.SetActive(false);
        spottedGradient_Right.SetActive(false);
    }

    private void Update()
    {
        debugOutput = "";
        debugOutput += $"CATS : {LevelManager.instance.CatsPetted} / {LevelManager.instance.CatsToPet}\n";
        debugText.text = debugOutput;

        Color targetColor = Color.white;
        targetColor.a = 0f;

        if (true || Enemy.instance.State.ShowScreenSpaceUI)
        {
            switch (Enemy.instance.Awareness.AwarenessValue)
            {
                case Awareness.AwarenessEnum.Idle:
                    targetColor = Color.white;
                    targetColor.a = 0.25f;
                    break;
                case Awareness.AwarenessEnum.Warning:
                    targetColor = Color.yellow;
                    targetColor.a = 0.25f;
                    break;
                case Awareness.AwarenessEnum.Alerted:
                    targetColor = Color.red;
                    targetColor.a = 1f;
                    break;
            }
        }

        if(Enemy.instance != null)
        {
            grannyScreenSpaceUI.color = Color.Lerp(grannyScreenSpaceUI.color, targetColor, Time.deltaTime * 3f);
            bool enabled = Vector3.Dot(FirstPersonController.instance.transform.forward, (FirstPersonController.instance.transform.position - Enemy.instance.transform.position)) < 0f;
            grannyScreenSpaceUI.enabled = enabled; //disable if it is behind
            grannyScreenSpaceUI_Fill.enabled = enabled; //disable if it is behind
            grannyScreenSpaceUI.rectTransform.position = FirstPersonController.instance.MainCamera.WorldToScreenPoint(Enemy.instance.transform.position + Vector3.up * 2f);
        }

        grannyScreenSpaceUI_Fill.fillAmount = Enemy.instance.Awareness.AwarenessPercentage;
        targetColor.a = 1f;
        grannyScreenSpaceUI_Fill.color = targetColor;

    }

    public void SetSpottedGradient(bool enable, Vector3 pos)
    {
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

    //the way these 3 methods below are is bad
    //ideally, the UI should observe other properties and act independently
    //ill change this later if i have time
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
}

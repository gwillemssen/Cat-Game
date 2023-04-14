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


    public Canvas Canvas;

    [HideInInspector] public Slider PettingMeter;
    private Slider throwStrengthMeter;
    private Image timeUI;
    private GameObject catTimerUI;
    private Text debugText;
    private Text infoText;
    private Animation catTimerAnim;
    private Animation infoTextFade;
    private Image hamd;
    private GameObject spottedGradient_Left;
    private GameObject spottedGradient_Right;
    private Image grannyScreenSpaceUI;
    private Sprite grannyScreenSpaceUI_NormalTexture;
    [SerializeField] private Sprite grannyScreenSpaceUI_ExclaimationPoint;
    [SerializeField] private Sprite grannyScreenSpaceUI_Stunned;
    private Image grannyScreenSpaceUI_Fill;
    private Image bloodOverlay;
    public GameObject WinScreen { get; private set; }
    public GameObject LoseScreen { get; private set; }
    public GameObject LoseCopsScreen { get; private set; }
    public Image Hamd { get { return hamd; } private set { hamd = value; } }
    public bool EnemyOnScreen { get; private set; }

    string debugOutput = "";
    private float exclaimationPointTimer = 0f;
    private float blinkTimer = 0f;
    private bool lastTimeSeenPlayer;
    private const float exclaimationPointDuration = 0.8f;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;
    }

    public void Init(FirstPersonController controller)
    {
        Transform t;
        for (int i = 0; i < Canvas.transform.childCount; i++)
        {
            t = Canvas.transform.GetChild(i);

            switch (t.gameObject.name)
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
                    grannyScreenSpaceUI_NormalTexture = grannyScreenSpaceUI.sprite;
                    break;
                case "BloodOverlay":
                    bloodOverlay = t.GetComponent<Image>();
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
        bloodOverlay.enabled = false;
        Enemy.instance.AggroState.OnShoot += OnShootCallback;
    }

    private void Update()
    {
        debugOutput = "";
        debugOutput += $"Cats Remaining: {GameManager.instance.CatsToPet.Count}\n";
        debugText.text = debugOutput;

        Color targetColor = Color.white;
        targetColor.a = 0f;

        targetColor = Color.white;
        targetColor.a = 0.25f;

        if (Enemy.instance.State == Enemy.instance.PatrollingState)
        {
            if(Enemy.instance.PatrollingState.AwarenessValue > Enemy.instance.Awareness_IdleState_Duration)
            {
                targetColor = Color.yellow;
                targetColor.a = 0.25f;
            }
        }
        else
        {
            targetColor = Color.red;
            targetColor.a = 0.3f;
        }

        bool uiEnabled = true;

        if (Enemy.instance != null)
        {
            grannyScreenSpaceUI.color = Color.Lerp(grannyScreenSpaceUI.color, targetColor, Time.deltaTime * 3f);
            uiEnabled = Vector3.Dot(FirstPersonController.instance.transform.forward, (FirstPersonController.instance.transform.position - Enemy.instance.transform.position)) < 0f;
            grannyScreenSpaceUI.enabled = uiEnabled; //disable if it is behind
            grannyScreenSpaceUI_Fill.enabled = uiEnabled; //disable if it is behind
            grannyScreenSpaceUI.rectTransform.position = FirstPersonController.instance.MainCamera.WorldToScreenPoint(Enemy.instance.transform.position + Vector3.up * 2f);
        }

        float fillAmount = 1f;

        if(Enemy.instance.State == Enemy.instance.PatrollingState)
        {
            fillAmount = Enemy.instance.PatrollingState.AwarenessValue / (Enemy.instance.Awareness_IdleState_Duration + Enemy.instance.Awareness_WarningState_Duration);
        }
        if(Enemy.instance.State == Enemy.instance.AggroState)
        {
            fillAmount = Enemy.instance.AggroState.AggroPercent;
        }

        grannyScreenSpaceUI_Fill.fillAmount = fillAmount;
        targetColor.a = 1f;
        grannyScreenSpaceUI_Fill.color = targetColor;

        if(Enemy.instance.SeesPlayer && !lastTimeSeenPlayer)
        { exclaimationPointTimer = exclaimationPointDuration; }

        exclaimationPointTimer -= Time.deltaTime;
        if(exclaimationPointTimer < 0)
        { exclaimationPointTimer = 0f; }

        grannyScreenSpaceUI.sprite = exclaimationPointTimer == 0 ? grannyScreenSpaceUI_NormalTexture : grannyScreenSpaceUI_ExclaimationPoint;
        grannyScreenSpaceUI_Fill.sprite = exclaimationPointTimer == 0 ? grannyScreenSpaceUI_NormalTexture : grannyScreenSpaceUI_ExclaimationPoint;
        if(Enemy.instance.Stunned)
        {
            grannyScreenSpaceUI.sprite = grannyScreenSpaceUI_Stunned;
            grannyScreenSpaceUI_Fill.sprite = grannyScreenSpaceUI_Stunned;
        }
        grannyScreenSpaceUI.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 1.65f, Mathf.InverseLerp(0f, exclaimationPointDuration, exclaimationPointTimer));

        if(Enemy.instance.GunObject.activeSelf && Enemy.instance.SeesPlayer)
        {
            blinkTimer += Time.deltaTime;
            if(blinkTimer > 0.25f)
            {
                blinkTimer = 0f;
                exclaimationPointTimer = exclaimationPointDuration;
            }
        }
        else if(uiEnabled)
        {
            grannyScreenSpaceUI.enabled = true;
        }

        lastTimeSeenPlayer = Enemy.instance.SeesPlayer;


        EnemyOnScreen = Vector3.Dot(transform.TransformDirection(Vector3.forward), (Enemy.instance.transform.position - transform.position)) >= .65;
        //enable both if enemy is on the screen
        bool onRightSide = Vector3.Cross(transform.TransformDirection(Vector3.forward), (Enemy.instance.transform.position - transform.position)).y > 0;

        spottedGradient_Left.SetActive(Enemy.instance.SeesPlayer && EnemyOnScreen || Enemy.instance.SeesPlayer && !onRightSide);
        spottedGradient_Right.SetActive(Enemy.instance.SeesPlayer && EnemyOnScreen || Enemy.instance.SeesPlayer && onRightSide);

    }

    private void OnShootCallback()
    {
        bloodOverlay.enabled = true;
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
}

using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using Random = UnityEngine.Random;

public class MicrowaveController : Interactable
{
    private Light microwaveLight;
    private Material mat;
    private GameObject plate;
    private GameObject door;

    public bool lightOn;
    public bool plateSpinning;
    public bool doorOpen;
    public bool allowDeactivate = false;
    public float timeLeft;
    public bool timerOn = false;

    private AudioClip[] sounds = new AudioClip[6];
    // SOUND LIST:
    // 0: beep
    // 1: start
    // 2: loop
    // 3: finish
    // 4: open
    // 5: close
    private AudioSource microwavePlayer;
    [HideInInspector]
    public int clipID;
    private TextMeshProUGUI timerText;
    private LerpScript LightLerp;
    private LerpScript plateLerp;
    private LerpScript doorLerp;
    private bool active;
    private Animator microwaveAnimator;
    
    void Start()
    {
        MicrowaveLoad();
    }

    // Update is called once per frame
    void Update()
    {
        LightManager();
        TimerManager();
        PlateManager();
        DoorManager();

        microwavePlayer.clip = sounds[clipID];
    }

    void LightManager()
    {
        if (lightOn)
        {
            LightLerp.floatTarget = 1;
        }
        else
        {
            LightLerp.floatTarget = 0;
        }

        if (LightLerp.floatTarget != LightLerp.floatVal)
        {
            mat.SetFloat("_LightIntensity",LightLerp.floatVal);
            microwaveLight.intensity = 0.09f * LightLerp.floatVal;
        }
    }

    void TimerManager()
    {
        if (timerOn)
        {
            if (timeLeft > 0)
            {
                timeLeft -= Time.deltaTime;
                UpdateTimer(timeLeft);
            }
            else
            {
                {
                    timeLeft = 0;
                    Debug.Log("time up");
                    timerOn = false;
                    microwaveAnimator.SetTrigger("FinishedTrigger");
                    active = false;
                }
            }
        }
    }
    
    void UpdateTimer(float currentTime)
    {
        currentTime += 1;

        float minutes = Mathf.FloorToInt(currentTime / 60);
        float seconds = Mathf.FloorToInt(currentTime % 60);

        timerText.text = string.Format("{0:00} : {1:00}", minutes, seconds);
    }

    void PlateManager()
    {

            if (plateSpinning)
            {
                plateLerp.floatTarget = 0.15f;
            }
            else
            {
                plateLerp.floatTarget = 0;
            }

            if (plateLerp.floatTarget != plateLerp.floatVal)
            {
                plate.transform.Rotate(0, 0, plateLerp.floatTarget);
            }
    }

    void DoorManager()
    {
        if (doorOpen)
        {
            doorLerp.vecTarget = new Vector3(0, 0, 100); 
        }
        else
        {
            doorLerp.vecTarget = new Vector3(0, 0, 0);
        }

        if (doorLerp.vecVal != doorLerp.vecTarget)
        {
            door.transform.localRotation = Quaternion.Euler(doorLerp.vecVal);
        }
    }

    public override void InteractClick(FirstPersonController controller)
    {
        ActivateMicrowave();
    }

    void ActivateMicrowave()
    {
        if (!active)
        {
            if (doorOpen)
            {
                //door open when off stuff
                microwaveAnimator.SetTrigger("CloseTrigger");
            }
            else
            {
                //turn on microwave
                active = true;
                timerOn = true;
                timeLeft = Random.Range(5,10);
                microwaveAnimator.SetTrigger("CookTrigger");
            }
        }
        else if (active && allowDeactivate)
        {
            //cancel cook of microwave if allowed
            microwaveAnimator.SetTrigger("CancelTrigger");
            active = false;
            CancelCook();
        }
    }

    private void MicrowaveLoad()
    {
        //sound loading
        sounds[0] = Resources.Load("Sound/So_MicrowaveBeep") as AudioClip;
        sounds[1] = Resources.Load("Sound/So_MicrowaveStart") as AudioClip;
        sounds[2] = Resources.Load("Sound/So_MicrowaveLoop") as AudioClip;
        sounds[3] = Resources.Load("Sound/So_MicrowaveFinish") as AudioClip;
        sounds[4] = Resources.Load("Sound/So_MicrowaveOpen") as AudioClip;
        sounds[5] = Resources.Load("Sound/So_MicrowaveClose") as AudioClip;
        Debug.Log($"{gameObject.name} sounds loaded");

        microwaveAnimator = GetComponent<Animator>();
        microwavePlayer = GetComponent<AudioSource>();
        door = transform.GetChild(1).GetChild(0).gameObject;
        doorLerp = this.AddComponent<LerpScript>();
        doorLerp.typeOfLerp = LerpScript.LerpType.Vector3;
        doorLerp.lerpSpeed = 8;
        plate = transform.GetChild(1).GetChild(1).gameObject;
        LightLerp = this.AddComponent<LerpScript>();
        LightLerp.lerpSpeed = 16;
        plateLerp = this.AddComponent<LerpScript>();
        
        mat = Instantiate(transform.GetChild(0).GetComponent<Renderer>().material);
        transform.GetChild(0).GetComponent<Renderer>().material = mat;
        microwaveLight = transform.GetChild(2).GetComponent<Light>();
        timerText = transform.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>();
        
        Debug.Log($"{gameObject.name} initialized");
    }

    void CancelCook()
    {
        timerOn = false;
        timeLeft = 0;
    }
}


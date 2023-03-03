using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using Random = UnityEngine.Random;

public class MicrowaveController : Interactable
{
    [SerializeField] private Transform investigatePos;
    public float currentNoiseEmitted;
    private Light light;
    private Material mat;
    private GameObject plate;
    private GameObject door;

    public bool lightOn;
    public bool plateSpinning;
    public bool doorOpen;

    private AudioClip[] sounds = new AudioClip[6];
    // SOUND LIST:
    // 0: beep
    // 1: start
    // 2: loop
    // 3: finish
    // 4: open
    // 5: close

    private AudioSource microwavePlayer;
    public int clipID;


    public float timeLeft;
    public bool timerOn = false;


    private TextMeshProUGUI timerText;

    private LerpScript LightLerp;
    private LerpScript plateLerp;
    private LerpScript doorLerp;
    public bool allowDeactivate = false;
    
    private bool active;

    private Animator microwaveAnimator;
    
    // Start is called before the first frame update
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
            light.intensity = 0.09f * LightLerp.floatVal;
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

    public override void Interact(FirstPersonController controller)
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

    private void MicrowaveFinish()
    {
        microwaveAnimator.SetTrigger("FinishedTrigger");
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
        light = transform.GetChild(2).GetComponent<Light>();
        timerText = transform.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    void PlaySound(AudioClip sound, bool interrupt)
    {
        if (interrupt)
        {
            microwavePlayer.Stop();
        }
        microwavePlayer.clip = sound;
        microwavePlayer.Play();
    }

    IEnumerator waiter(float seconds)
    {
        bool done = false;
        yield return new WaitForSeconds(seconds);
    }

    void CancelCook()
    {
        timerOn = false;
        timeLeft = 0;
    }
}


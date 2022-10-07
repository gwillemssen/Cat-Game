using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SwitchController : Interactable
{
    private LerpScript switchLerp;
    private GameObject switchBone;
    public bool on;
    private bool storedValue;
    private Vector3 offPos = new Vector3(40, 0, 0);
    private Vector3 onPos = new Vector3(-40, 0, 0);
    [Tooltip("List of lights that this switch effects, add as many as needed.")]
    public List<Light> lights;
    private AudioClip[] sounds = new AudioClip[2];
    // SOUND LIST:
    // 0: on
    // 1: off

    // Start is called before the first frame update
    private AudioSource lightSwitchPlayer;
    [HideInInspector]
    public int clipID;
    void Start()
    {
        SwitchLoad();
    }
    
    void Update()
    {
        if (storedValue != on)//runs the frame that the boolean for "on" changes
        {
            if (on)//when it's switched to on, the rotation target is moved to the on position and the on sound is played, then all lights in the array are toggled and the stored value is made equal so it doesn't loop the function
            {
                switchLerp.vecTarget = onPos;
                PlaySound(0);
                ToggleLight();
                storedValue = on;
            }
            else
            {
                switchLerp.vecTarget = offPos;
                PlaySound(1);
                ToggleLight();
                storedValue = on;
            }
        }

        if (switchBone.transform.localRotation != Quaternion.Euler(switchLerp.vecVal))//updates rotation of the switch to match the desired value while it's not the same
        {
            switchBone.transform.localRotation = Quaternion.Euler(switchLerp.vecVal);
        }
    }

    void ToggleLight()
    {
        foreach (Light light1 in lights)
        {
            light1.enabled = !light1.enabled;
        }
    }
    public override void InteractClick(FirstPersonController controller)
    {
        ToggleSwitch();
    }

    private void SwitchLoad()
    {
        //sound loading
        sounds[0] = Resources.Load("Sound/LightSwitch/So_SwitchOn") as AudioClip;
        sounds[1] = Resources.Load("Sound/LightSwitch/So_SwitchOff") as AudioClip;
        Debug.Log($"{gameObject.name} sounds loaded");
        
        //initialization
        lightSwitchPlayer = GetComponent<AudioSource>();
        switchBone = transform.GetChild(2).GetChild(0).gameObject;
        switchLerp = this.AddComponent<LerpScript>();
        switchLerp.typeOfLerp = LerpScript.LerpType.Vector3;
        switchLerp.lerpSpeed = 8;
        Debug.Log($"{gameObject.name} Initialized");
    }

    private void PlaySound(int clip)
    {
        clipID = clip;
        lightSwitchPlayer.clip = sounds[clipID];
        lightSwitchPlayer.Play();
    }

    private void ToggleSwitch()
    {
        on = !on;
    }

    
}

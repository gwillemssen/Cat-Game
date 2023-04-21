using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;

public class SwitchController : Interactable
{
    public MeshRenderer meshRenderer;
    public AudioClip SoundOn, SoundOff;
    private LerpScript switchLerp;
    private GameObject switchBone;
    public bool on;
    private Vector3 offPos = new Vector3(40, 0, 0);
    private Vector3 onPos = new Vector3(-40, 0, 0);
    [Tooltip("List of lights that this switch effects, add as many as needed.")]
    public List<LightInfo> lights;
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
        ToggleLight();

        switchLerp.vecTarget = on ? onPos : offPos;
    }

    void Update()
    {
        Animate();
    }

    void ToggleLight()
    {
        foreach (var light in lights.Where(light=> light != null))
        {
            light.light.enabled = on;
            light.meshRenderer.material.SetColor("_EmissionColor", on ? Color.yellow : Color.black);
        }
        switchLerp.vecTarget = on ? onPos : offPos;
    }


    public override void Interact(FirstPersonController controller)
    {
        on = !on;
        ToggleLight();
        PlaySound(on ? 0 : 1);

        //if (on == onBefore)
        //{
        //    //I can't believe this works
        //    on = !on; 
        //    ToggleLight();
        //    on = !on;
        //    //This allows multiple switches to control the same light, like a real home
        //} 
    }

    private void SwitchLoad()
    {
        //sound loading
        sounds[0] = SoundOn; 
        sounds[1] = SoundOff;
        //Debug.Log($"{gameObject.name} sounds loaded");

        //initialization
        lightSwitchPlayer = GetComponent<AudioSource>();
        switchBone = transform.GetChild(2).GetChild(0).gameObject;
        switchLerp = this.AddComponent<LerpScript>();
        switchLerp.typeOfLerp = LerpScript.LerpType.Vector3;
        switchLerp.lerpSpeed = 8;
        //Debug.Log($"{gameObject.name} Initialized");
    }

    private void PlaySound(int clip)
    {
        clipID = clip;
        lightSwitchPlayer.clip = sounds[clipID];
        lightSwitchPlayer.Play();
    }

    private void Animate()
    {

        if (switchBone.transform.localRotation != Quaternion.Euler(switchLerp.vecVal))//updates rotation of the switch to match the desired value while it's not the same
        {
            switchBone.transform.localRotation = Quaternion.Euler(switchLerp.vecVal);
        }
    }

    [Serializable]
    public class LightInfo
    {
        public Light light;
        public MeshRenderer meshRenderer;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;

public class SwitchController : Interactable
{
    public GameObject switchBone;
    public bool on;
    private Vector3 offPos = new Vector3(40, 0, 0);
    private Vector3 onPos = new Vector3(-40, 0, 0);
    [Tooltip("List of lights that this switch effects, add as many as needed.")]
    public List<LightInfo> lights;
    public AudioClip[] sounds = new AudioClip[2];
    // SOUND LIST:
    // 0: on
    // 1: off
    public AudioSource lightSwitchPlayer;
    void Start()
    {
        SwitchLoad();
    }

    void ToggleLight()
    {
        if(LeanTween.isTweening(switchBone.gameObject)) LeanTween.cancel(switchBone.gameObject);
        foreach (var light in lights.Where(light=> light != null))
            light.light.enabled = on;

        foreach (var lightMesh in lights.Where(lightMesh => lightMesh != null))
            lightMesh.meshRenderer.material.SetColor("_EmissionColor", on ? Color.yellow : Color.black);

        LeanTween.value(switchBone.gameObject, 0, 1, 0.5f).setOnUpdate((float val) =>
        {
            switchBone.transform.localRotation = Quaternion.Lerp(switchBone.transform.localRotation, Quaternion.Euler(on?onPos:offPos), val);
        });
        lightSwitchPlayer.PlayOneShot(on ? sounds[0] : sounds[1]);
    }


    public override void Interact(FirstPersonController controller)
    {
        on = !on;
        ToggleLight();
    }

    private void SwitchLoad()
    {
        //initialization
        lightSwitchPlayer = GetComponent<AudioSource>();
        // switchBone = transform.GetChild(2).GetChild(0).gameObject;
        //Debug.Log($"{gameObject.name} Initialized");
    }

    [Serializable]
    public class LightInfo
    {
        public Light light;
        public MeshRenderer meshRenderer;
    }
}

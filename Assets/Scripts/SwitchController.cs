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
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

    private void Start()
    {
        //initialize the emission colors of each light.
        foreach (LightInfo light in lights) if(light.meshRenderer)light.meshColor = light.meshRenderer.material.GetColor(EmissionColor);
    }
    void ToggleLight()
    {
        foreach (var light in lights)
        {
            //toggle light if it exists
            if (light.light) light.light.enabled = !light.light.enabled;

            //toggle emission if the entry exists
            if (!light.meshRenderer) continue;
            Color lightColor;
            lightColor = light.meshRenderer.material.GetColor(EmissionColor);
                
            if(LeanTween.isTweening(light.meshRenderer.gameObject)) LeanTween.cancel(light.meshRenderer.gameObject);
            LeanTween.value(light.meshRenderer.gameObject, 0, 1, 0.05f).setOnUpdate(val =>
                light.meshRenderer.material.SetColor(EmissionColor,
                    Color.Lerp(lightColor,
                        light.light.enabled ? light.meshColor : Color.black,val)));
        }

        //Animate Lightswitch
        if(LeanTween.isTweening(switchBone.gameObject)) LeanTween.cancel(switchBone.gameObject);
        LeanTween.value(switchBone.gameObject, 0, 1, 0.5f).setOnUpdate(val =>
            switchBone.transform.localRotation = Quaternion.Lerp(switchBone.transform.localRotation,
                Quaternion.Euler(on ? onPos : offPos), val));
        //Play Lightswitch Sound
        lightSwitchPlayer.PlayOneShot(on ? sounds[0] : sounds[1]);
    }


    public override void Interact(FirstPersonController controller)
    {
        on = !on;
        ToggleLight();
    }

    [Serializable]
    public class LightInfo
    {
        public Light light;
        public MeshRenderer meshRenderer;
        [HideInInspector]public Color meshColor;
    }
}

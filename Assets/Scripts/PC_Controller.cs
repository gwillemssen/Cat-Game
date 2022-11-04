using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using Random = UnityEngine.Random;

public class PC_Controller : Interactable
{
    private AudioPlayer audioPlayer;
    public Sound StartupSound;
    public GameObject objPC;
    private Renderer pcRenderer;
    public Texture pcTex;
    private Material mat;
    public bool on;

    private void Start()
    {
        audioPlayer = GetComponent<AudioPlayer>();
        pcRenderer = objPC.GetComponent<Renderer>();
        mat = Instantiate(pcRenderer.material);
        pcRenderer.material = mat;
    }
    public override void InteractClick(FirstPersonController controller)
    {
        if (on)
        {
            mat.SetFloat(Shader.PropertyToID("_PowerBool"),0);
            on = !on;
            audioPlayer.Stop();
        }
        else
        {
            mat.SetFloat(Shader.PropertyToID("_PowerBool"),1);
            on = !on;
            audioPlayer.Play(StartupSound);
        }


    }
}

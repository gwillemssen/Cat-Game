using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using Random = UnityEngine.Random;

public class PC_Controller : Interactable
{
    public GameObject objPC;
    private Renderer pcRenderer;
    public Texture pcTex;
    private Material mat;
    public bool on;

    private void Start()
    {

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
            AudioManager.instance.Stop("PC Startup");
            AudioManager.instance.Stop("PC Distraction");
        }
        else
        {
            mat.SetFloat(Shader.PropertyToID("_PowerBool"),1);
            on = !on;
            AudioManager.instance.Play("PC Startup");
            AudioManager.instance.Play("PC Distraction");
        }


    }
}

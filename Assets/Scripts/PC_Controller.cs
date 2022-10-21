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

    private void Start()
    {
        pcRenderer = objPC.GetComponent<Renderer>();
    }
    public override void InteractClick(FirstPersonController controller)
    {
        AudioManager.instance.Play("PC Startup");
        AudioManager.instance.Play("PC Distraction");
        pcRenderer.material.SetTexture("PC_ON_BaseColor", pcTex);
        
    }
}

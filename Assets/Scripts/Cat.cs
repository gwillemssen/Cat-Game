using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cat : Interactable
{
    //inspector
    public enum CatStartState { Pettable, Unpettable } 
    public CatStartState StartState;
    public float PettingSpeed = .2f;
    public float PettingDecayRate = 0.1f;
    public float PettingDecayDelay = 1f;
    public float PettingSpeedRequired = 1000f;

    //general
    private enum CatState { Pettable, Unpettable, PettingMinigame, DonePetting };
    private CatState state;
    private Animator anim;
    private AudioSource audio;
    
    //Minigame
    private Vector3 catOriginalPos;
    private Quaternion catOriginalRot;
    private Transform catHoldingPosition;
    private FirstPersonController playerController;
    private float timeStartedMovingCat = -420f;
    private float pettingAmount = 0f;
    private Vector2 lastMousePosition = Vector2.zero;
    private Vector2 mouseDelta = Vector2.zero;
    private float lastTimePet = -420f;

    private void Start()
    {
        state = (CatState)StartState;
        anim = GetComponentInChildren<Animator>();
        audio = GetComponent<AudioSource>();
    }

    private void Update()
    {
        anim.SetBool("Excited", LookingAt && state == CatState.Pettable);

        if (state == CatState.PettingMinigame || state == CatState.DonePetting)
        {
            float t = Time.time - timeStartedMovingCat;
            t = Mathf.Clamp01(t);
            t = t * t * t * (t * (6f * t - 15f) + 10f); //smootherstep

            //move da cat
            if (state == CatState.PettingMinigame)
            {
                transform.position = Vector3.Lerp(catOriginalPos, catHoldingPosition.position, t);
                transform.rotation = Quaternion.Lerp(catOriginalRot, catHoldingPosition.rotation, t);
            }
            else
            {
                transform.position = Vector3.Lerp(catHoldingPosition.position, catOriginalPos, t);
                transform.rotation = Quaternion.Lerp(catHoldingPosition.rotation, catOriginalRot, t);

                if(Vector3.SqrMagnitude(transform.position - catOriginalPos) <= 0.005)
                {
                    transform.position = catOriginalPos;
                    transform.rotation = catOriginalRot;
                    state = CatState.Pettable;
                    //maybe this should this change back to the startState?
                }
            }

            if (state == CatState.PettingMinigame) //PET THE CAT
            {
                bool decay = Time.time - lastTimePet > PettingDecayDelay;

                if (playerController.Input.interacting)
                {
                    mouseDelta = lastMousePosition - playerController.Input.mousePosition;
                    float mouseSpeed = mouseDelta.magnitude / Time.deltaTime; //speed = distance / time

                    if (mouseSpeed >= PettingSpeedRequired)
                    {
                        pettingAmount += mouseSpeed * PettingSpeed * .000001f;
                        lastTimePet = Time.time;
                        audio.volume += Time.deltaTime;
                    }
                }
                else if(!decay)
                {
                    audio.volume -= Time.deltaTime;
                }

                if (decay) //cat got bored
                {
                    pettingAmount -= Time.deltaTime * PettingDecayRate;
                    audio.volume -= Time.deltaTime;
                }

                lastMousePosition = playerController.Input.mousePosition;

                playerController.PettingMeter.value = pettingAmount;

                pettingAmount = Mathf.Clamp01(pettingAmount);
                if(pettingAmount == 1f)                         //win
                { EndMinigame(); }
            }
        }
        if(audio.isPlaying && state != CatState.PettingMinigame)
        {
            audio.volume -= Time.deltaTime;
            if (audio.volume <= 0f)
                audio.Stop();
        }
    }

    public override void InteractClick(FirstPersonController controller)
    {
        if(state == CatState.Pettable)
        {
            catHoldingPosition = controller.CatHoldingPosition;
            playerController = controller;
            StartMinigame();
        }
    }

    private void StartMinigame()
    {
        catOriginalPos = transform.position;
        catOriginalRot = transform.rotation;
        timeStartedMovingCat = Time.time;
        state = CatState.PettingMinigame;
        playerController.enabled = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        playerController.PettingMeter.gameObject.SetActive(true);
        pettingAmount = 0f;
        audio.volume = 0f;
        audio.Play();
    }

    private void EndMinigame()
    {
        state = CatState.DonePetting;
        timeStartedMovingCat = Time.time;
        playerController.enabled = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        playerController.PettingMeter.gameObject.SetActive(false);
    }
}

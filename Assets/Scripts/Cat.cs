using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * I am aware this code needs refactoring
 * its a bit of a mess right now
 * 
 * It also needs a way to cancel the minigame.  Maybe right click?
 * Right click could also be the throw button for props.
 */
[RequireComponent(typeof(AudioSource))]
public class Cat : Interactable
{
    //inspector
    public enum CatStartState { Pettable, Unpettable }
    public CatStartState StartState;
    public int PetsRequired = 12;
    public float PettingDecayRate = 0.75f;
    public float PettingDecayDelay = 1f;

    //general
    private enum CatState { Pettable, Unpettable, PettingMinigame, DonePetting };
    private CatState state;
    private Animator anim;
    private AudioSource audio;
    //private ShakePosition shake;

    //events
    public static event Action CompletedPetting;

    //Minigame
    private Vector3 catOriginalPos;
    private Quaternion catOriginalRot;
    private Vector3 catOriginalScale;
    private Vector3 catHoldingPos;
    private Quaternion catHoldingRot;
    private FirstPersonController playerController;
    private float timeStartedMovingCat = -420f;
    private float pettingAmount = 0f;
    private Vector2 lastPetMousePos; //need to travel a certain distance from this point to trigger a pet
    private const float petDistance = 128f;
    private const float sqrPetDistance = petDistance * petDistance;
    private const float pettingSpeedMax = 1000f;
    //private Vector2 lastMousePosition = Vector2.zero;// Intellisense reamarked that it was never used
    private Vector2 lastDirectionPet; //so we need to go back and forth while petting
    private bool firstPet = true;
    private Vector2 mouseDelta = Vector2.zero;
    private Vector2 mouseSpeed;
    private float lastTimePet = -420f;
    private static float petPushCatAmt = 0.03f;
    private static float petStretchCatAmt = 0.007f;
    private static float petPushLerpSmoothing = .5f;
    private static float petStretchLerpSmoothing = 8f;
    private static float petCameraFOVNormal = 90f;
    private static float petCameraFOVZoomed = 80f;

    private void Start()
    {
        state = (CatState)StartState;
        anim = GetComponentInChildren<Animator>();
        audio = GetComponent<AudioSource>();
        lastPetMousePos = new Vector2(-420f, -420f);
    }

    private void Update()
    {
        anim.SetBool("Excited", LookingAt && state == CatState.Pettable);

        if (state == CatState.PettingMinigame || state == CatState.DonePetting)
        {
            MoveCat();

            if (state == CatState.PettingMinigame) //PET THE CAT
            {
                UpdateMinigame();
                if (playerController.Input.throwing)
                {
                    EndMinigame();
                    base.CanInteract = true;
                }
            }
        }
        if (audio.isPlaying && state != CatState.PettingMinigame)
        {
            audio.volume -= Time.deltaTime;
            if (audio.volume <= 0f)
                audio.Stop();
        }
    }

    public override void InteractClick(FirstPersonController controller)
    {
        if (state == CatState.Pettable)
        {
            catHoldingPos = controller.CatHoldingPosition.position;
            catHoldingRot = controller.CatHoldingPosition.rotation;
            playerController = controller;
            StartMinigame();
        }
    }

    Vector3 startPos, targetPos, offsetPos, offsetScale;
    Quaternion startRot, targetRot;
    private void MoveCat()
    {
        float t = Time.time - timeStartedMovingCat;
        t = Mathf.Clamp01(t);
        t = t * t * t * (t * (6f * t - 15f) + 10f); //smootherstep

        offsetPos = Vector3.Lerp(offsetPos, Vector3.zero, Time.deltaTime * petPushLerpSmoothing);
        offsetScale = Vector3.Lerp(offsetScale, Vector3.zero, Time.deltaTime * petStretchLerpSmoothing);

        //move da cat
        if (state == CatState.PettingMinigame)
        {
            if (playerController.Input.interacting)
            {
                //cat gets pushed around by petting
                offsetPos -= transform.right * mouseDelta.x;
                offsetPos += transform.up * mouseDelta.y;
                offsetPos *= petPushCatAmt;

                //cat gets stretched by petting
                //choose greater one
                if (Mathf.Abs(mouseDelta.x) > Mathf.Abs(mouseDelta.y))
                { offsetScale.x += Mathf.Abs(mouseDelta.x * petStretchCatAmt); }
                else
                { offsetScale.y += Mathf.Abs(mouseDelta.y * petStretchCatAmt); }

            }

            //FOV from petting intensity
            playerController.TargetFOV = Mathf.Lerp(petCameraFOVNormal, petCameraFOVZoomed, (mouseSpeed.magnitude / pettingSpeedMax));

            startPos = catOriginalPos;
            startRot = catOriginalRot;
            targetPos = catHoldingPos - offsetPos;
            targetRot = catHoldingRot;
        }
        else
        {
            targetPos = catOriginalPos;
            targetRot = catOriginalRot;
            startPos = catHoldingPos;
            startRot = catHoldingRot;
        }

        if (transform.position != targetPos)
        {
            if (state == CatState.DonePetting && Vector3.SqrMagnitude(transform.position - targetPos) <= 0.005)
            {
                transform.SetPositionAndRotation(targetPos, targetRot);

                state = CatState.Pettable;
                //maybe this should this change back to the startState?
            }
            else
            {
                transform.SetPositionAndRotation(Vector3.Lerp(startPos, targetPos, t), Quaternion.Lerp(startRot, targetRot, t));
                
            }
        }

        transform.localScale = catOriginalScale + offsetScale;
    }

    private void StartMinigame()
    {
        base.CanInteract = false;
        catOriginalPos = transform.position;
        catOriginalRot = transform.rotation;
        catOriginalScale = transform.localScale;
        timeStartedMovingCat = Time.time;
        state = CatState.PettingMinigame;
        playerController.DisableMovement = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        playerController.UI.PettingMeter.gameObject.SetActive(true);
        playerController.UI.PettingMeter.value = 0f;
        playerController.Interaction.HideCrosshair = true;
        playerController.UI.SetInfoText("Click and Drag to pet the Cat!\nRight click to Cancel");
        pettingAmount = 0f;
        audio.volume = 0f;
        audio.Play();
        
    }

    private void UpdateMinigame()
    {
        bool decay = Time.time - lastTimePet > PettingDecayDelay;

        if (playerController.Input.interactedOnce)
        { 
            firstPet = true;
            lastPetMousePos = playerController.Input.mousePosition;
            FindObjectOfType<AudioManager>().Play("DeathMetal");
        }

        if (playerController.Input.interacting)
        {
            mouseDelta = playerController.Input.look;
            mouseSpeed = mouseDelta / Time.deltaTime; //speed = distance / time

            if (Vector2.SqrMagnitude(playerController.Input.mousePosition - lastPetMousePos) >= sqrPetDistance)
            {
                if (firstPet || Vector2.Dot(lastDirectionPet, (playerController.Input.mousePosition - lastPetMousePos)) > 0)
                {
                    //petted the cat
                    firstPet = false;
                    lastDirectionPet = lastPetMousePos - playerController.Input.mousePosition;
                    lastPetMousePos = playerController.Input.mousePosition;
                    pettingAmount += (1f / (float)PetsRequired);
                    lastTimePet = Time.time;
                }
            }
        }
        else if (!decay)
        {
            audio.volume -= Time.deltaTime;
        }

        if (decay) //cat got bored
        {
            pettingAmount -= Time.deltaTime * PettingDecayRate;
            audio.volume -= Time.deltaTime;
        }

        if (!playerController.Input.interacting)
        {
            mouseSpeed = Vector2.zero;
            mouseDelta = Vector2.zero;
        }

        playerController.UI.PettingMeter.value = pettingAmount;

        pettingAmount = Mathf.Clamp01(pettingAmount);
        if (pettingAmount == 1f)                         //win
        {
            EndMinigame();
            base.CanInteract = false;
            CompletedPetting?.Invoke();
            //maybe play an audio clip when the minigame is done?
            // I'm thinking that the player desn't know if a cat has been finished petting or if they've already pet one.
        }
    }

    private void EndMinigame()
    {
        playerController.TargetFOV = petCameraFOVNormal;
        transform.localScale = catOriginalScale;
        state = CatState.DonePetting;
        timeStartedMovingCat = Time.time;
        playerController.DisableMovement = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        playerController.UI.PettingMeter.gameObject.SetActive(false);
        playerController.Interaction.HideCrosshair = false;
        FindObjectOfType<AudioManager>().Stop("DeathMetal");
    }
}

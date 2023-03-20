using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;
using Random = UnityEngine.Random;

public class CatMeow : MonoBehaviour
{
    //the reason this happens in a seperate object is because the cat sounds were overlapping too much
    private float catTimer;
    float minTimeForMeow = 5f;
    float maxTimeForMeow = 25f;

    public void Awake()
    {
        if(Resources.FindObjectsOfTypeAll(typeof(CatMeow)).Length > 1) //not super performant, but it should only be called a few times at the beginning of the game
        { Destroy(this.gameObject); return; } //there can only be one

        catTimer += Random.Range(minTimeForMeow, maxTimeForMeow);
    }

    public void Update()
    {
        if (GameManager.instance.CatsToPet.Count > 0) //there are still unpetted cats
        {
            if (catTimer <= 0f)
            {
                Cat randomCat = GameManager.instance.CatsToPet[Random.Range(0, GameManager.instance.CatsToPet.Count)]; //meow a random cat
                randomCat.MeowAudioSource.Play();
                catTimer += Random.Range(minTimeForMeow, maxTimeForMeow);
            }
            if (catTimer > 0f)
            {
                catTimer -= Time.deltaTime;
            }
        }
    }
}

/*
 * I am aware this code needs refactoring
 * its a bit of a mess right now
 * 
 */
[RequireComponent(typeof(AudioSource))]
public class Cat : Interactable
{
    //inspector
    public enum CatStartState { Pettable, Unpettable }
    public CatStartState StartState;
    public int PetsRequired = 12;
    public float PettingDecayRate = 0.75f;
    public float PettingDecayDelay = 0.5f;
    public float MaxVolume = 0.2f;
    public float LightningAmount = 10f;
    public AudioSource MusicAudioSource;
    [SerializeField] private AudioClip music1;
    [SerializeField] private AudioClip music2;
    public AudioSource MeowAudioSource;
    [SerializeField] private float[] musicStartPositions;
    [SerializeField] private float[] music2StartPositions;

    //general
    private enum CatState { Pettable, Unpettable, PettingMinigame, DonePetting };
    private CatState state;
    private Animator anim;
    public ParticleSystem lightningParticles;


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
    private const float petDistance = 48f;
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
    public float colorDampener;
    public float colorAmplifier;

    private void Start()
    {
        state = (CatState)StartState;
        anim = GetComponentInChildren<Animator>();
        lastPetMousePos = new Vector2(-420f, -420f);
        catOriginalPos = transform.position;
        catOriginalRot = transform.rotation;
        catOriginalScale = transform.localScale;
        GameObject g = new GameObject("CatMeowObject", typeof(CatMeow)); //create an external object to do the meowing sounds.

        GameManager.instance.RegisterCat(this);

    }

    private void Update()
    {
        anim.SetBool("Excited", LookingAt && state == CatState.Pettable);
        anim.SetBool("DonePetting", state == CatState.DonePetting && !base.CanInteract);

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
        if (MusicAudioSource.isPlaying && state != CatState.PettingMinigame)
        {
            MusicAudioSource.volume -= Time.deltaTime;
            if (MusicAudioSource.volume <= 0f)
            { MusicAudioSource.Stop(); }
        }

        if (state == CatState.PettingMinigame)
        {
            playerController.UI.Hamd.enabled = true;
            playerController.UI.Hamd.transform.position = Input.mousePosition;
        }
    }

    public override void Interact(FirstPersonController controller)
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
        timeStartedMovingCat = Time.time;
        state = CatState.PettingMinigame;
        playerController.DisableMovement = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.None;
        PlayerUI.instance.PettingMeter.gameObject.SetActive(true);
        PlayerUI.instance.PettingMeter.value = 0f;
        playerController.Interaction.HideCrosshair = true;
        PlayerUI.instance.SetInfoText("Click and Drag to pet the Cat!\nRight click to Cancel");
        pettingAmount = 0f;
        MusicAudioSource.volume = 0f;
        MusicAudioSource.Play();
    }

    private void UpdateMinigame()
    {
        bool decay = Time.time - lastTimePet > PettingDecayDelay;

        //start
        if (playerController.Input.interactedOnce)
        {
            firstPet = true;
            lastPetMousePos = playerController.Input.mousePosition;
        }

        //petting
        if (playerController.Input.interacting)
        {
            mouseDelta = playerController.Input.look;
            mouseSpeed = mouseDelta / Time.deltaTime; //speed = distance / time

            if (Vector2.SqrMagnitude(playerController.Input.mousePosition - lastPetMousePos) >= sqrPetDistance)
            {
                if (firstPet || Vector2.Dot(lastDirectionPet, (playerController.Input.mousePosition - lastPetMousePos)) > 0)
                {
                    //petted the cat
                    if (!firstPet)
                    {
                        lastTimePet = Time.time;
                        int particleAmt = (int)(LightningAmount * pettingAmount);
                        lightningParticles.Emit(particleAmt);
                    }
                    firstPet = false;
                    lastDirectionPet = lastPetMousePos - playerController.Input.mousePosition;
                    pettingAmount += (1f / (float)PetsRequired);
                }

                lastPetMousePos = playerController.Input.mousePosition; //we move this either way to make it feel consistent
            }
        }
        else //not interacting
        {
            mouseSpeed = Vector2.zero;
            mouseDelta = Vector2.zero;
        }

        if(MusicAudioSource.volume == 0f)
        {
            MusicAudioSource.Stop();
            int music = Random.Range(0, 2);
            MusicAudioSource.clip = music == 0 ? music1 : music2;
            MusicAudioSource.time = music == 0 ? musicStartPositions[Random.Range(0, musicStartPositions.Length)] : music2StartPositions[Random.Range(0, music2StartPositions.Length)];
            MusicAudioSource.Play();
        }

        if (decay) //cat got bored
        {
            pettingAmount -= Time.deltaTime * PettingDecayRate;
            //audio.volume -= Time.deltaTime;
            MusicAudioSource.volume = 0f;
        }
        else //cat getting pet mmm yes good :)
        {
            MusicAudioSource.volume += Time.deltaTime * MaxVolume;
            MusicAudioSource.volume = Mathf.Clamp(MusicAudioSource.volume, 0f, MaxVolume);
        }

        PlayerUI.instance.PettingMeter.value = pettingAmount;
        pettingAmount = Mathf.Clamp01(pettingAmount);

        if (pettingAmount == 1f) //win
        {
            EndMinigame();
            base.CanInteract = false;
            GameManager.instance.CatPetted(this);
        }
    }

    public void EndMinigame()
    {
        transform.localScale = catOriginalScale;
        state = CatState.DonePetting;
        timeStartedMovingCat = Time.time;
        playerController.DisableMovement = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        PlayerUI.instance.PettingMeter.gameObject.SetActive(false);
        playerController.Interaction.HideCrosshair = false;
        playerController.UI.Hamd.enabled = false;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

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
    public float PettingDecayDelay = 0.5f;
    public float MaxVolume = 0.2f;
    // public AudioClip[] Catsounds;
    public AudioClip firewhoosh;
    private Meow Catsound;


    //general
    private enum CatState { Pettable, Unpettable, PettingMinigame, DonePetting };
    private CatState state;
    private Animator anim;
    private AudioSource audioSource;
    private AudioSource audioSource2;
    //public ParticleSystem effects;
    public ParticleSystem[] effects;



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
    private static float petCameraFOVNormal = 70f;
    private static float petCameraFOVZoomed = 50f;
    public float colorDampener;
    public float colorAmplifier;

    // audioPlayer.Play(Catsounds[Random.Range(0, 2)]); No idea where to put this. It either plays the cat sounds or doesn't
    private void Start()
    {
        //audioPlayer = GetComponent<AudioPlayer>();
        //effects = GetComponentInChildren<ParticleSystem>();
        effects = GetComponentsInChildren<ParticleSystem>();
        state = (CatState)StartState;
        anim = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        lastPetMousePos = new Vector2(-420f, -420f);
        //audioSource2 = GetComponentInChildren<AudioSource>();

    }

    private void Update()
    {
        // audioSource2.volume = 1f;

        anim.SetBool("Excited", LookingAt && state == CatState.Pettable);
        anim.SetBool("DonePetting", state == CatState.DonePetting);

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
        if (audioSource.isPlaying && state != CatState.PettingMinigame)
        {
            audioSource.volume -= Time.deltaTime;
            if (audioSource.volume <= 0f)
            { audioSource.Stop(); }
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
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.None;

        PlayerUI.instance.PettingMeter.gameObject.SetActive(true);
        PlayerUI.instance.PettingMeter.value = 0f;
        playerController.Interaction.HideCrosshair = true;
        PlayerUI.instance.SetInfoText("Click and Drag to pet the Cat!\nRight click to Cancel");
        pettingAmount = 0f;
        audioSource.volume = 0f;
        audioSource.Play();
    }

    private void UpdateMinigame()
    {
        bool decay = Time.time - lastTimePet > PettingDecayDelay;

        if (playerController.Input.interactedOnce)
        {
            firstPet = true;
            lastPetMousePos = playerController.Input.mousePosition;
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
                    if (!firstPet)
                    { lastTimePet = Time.time; }
                    firstPet = false;
                    lastDirectionPet = lastPetMousePos - playerController.Input.mousePosition;
                    pettingAmount += (1f / (float)PetsRequired);
                }

                lastPetMousePos = playerController.Input.mousePosition; //we move this either way to make it feel consistent
            }
        }

        if (decay) //cat got bored
        {
            pettingAmount -= Time.deltaTime * PettingDecayRate;
            //audio.volume -= Time.deltaTime;
            audioSource.volume = 0f;
        }
        else
        {
            audioSource.volume += Time.deltaTime * MaxVolume;
            audioSource.volume = Mathf.Clamp(audioSource.volume, 0f, MaxVolume);
        }

        if (!decay && !effects[0].isPlaying || !decay && !effects[1].isPlaying)
        {
            audioSource.PlayOneShot(firewhoosh);
            effects[0].Play();
            effects[1].Play();
            var efmain = effects[0].emission.rateOverDistance.constant;
            //  var efsubMain = effects[0].colorOverLifetime.color.gradient.alphaKeys[Random.Range(2,4)];
            FadeinParticles();
            efmain = 7f;

            efmain += Time.deltaTime;
        }
        if (decay && effects[0].isPlaying || decay && effects[1].isPlaying)

        { //effects.Stop();
            var efmain = effects[1].emission.rateOverDistance.constant;
            efmain -= Time.deltaTime;
            FadeoutParticles();
        }

        if (!playerController.Input.interacting)
        {
            mouseSpeed = Vector2.zero;
            mouseDelta = Vector2.zero;

        }

        PlayerUI.instance.PettingMeter.value = pettingAmount;

        pettingAmount = Mathf.Clamp01(pettingAmount);
        if (pettingAmount == 1f)                         //win
        {
            EndMinigame();
            Meow.instance.PlayMeow();

            base.CanInteract = false;
            CompletedPetting?.Invoke();
        }
    }

    public void EndMinigame()
    {

        // audioSource2.PlayOneShot(Catsounds[Random.Range(0, Catsounds.Length)]);
        playerController.TargetFOV = petCameraFOVNormal;
        transform.localScale = catOriginalScale;
        state = CatState.DonePetting;
        timeStartedMovingCat = Time.time;
        playerController.DisableMovement = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        PlayerUI.instance.PettingMeter.gameObject.SetActive(false);
        playerController.Interaction.HideCrosshair = false;
        effects[0].Stop();
        effects[1].Stop();
        playerController.UI.Hamd.enabled = false;

    }

    private void FadeinParticles()
    {
        //FIRE
        var colorChange = effects[0].main.startColor.color;
        Color col = colorChange;
        Debug.Log("Initial Color: " + col.a);
        col.a += col.a * colorAmplifier * Time.deltaTime;
        Debug.Log("Color: " + col.a);
        colorChange = col;

        //LIGHTNING

        //LIGHTNING
        var colorChange2 = effects[1].main.startColor.color;
        Color col2 = colorChange2;
        Debug.Log("Initial Color: " + col.a);
        col2.a += col2.a * colorAmplifier * Time.deltaTime;
        Debug.Log("Color: " + col.a);
        colorChange2 = col;

    }
    private void FadeoutParticles()
    {
        //FIRE
        var colorChange = effects[0].main.startColor.color;
        Color col = colorChange;
        Debug.Log("Initial Color: " + col.a);
        col.a -= col.a * colorDampener * Time.deltaTime;
        Debug.Log("Color: " + col.a);

        //LIGHTNING
        var colorChange2 = effects[1].main.startColor.color;
        Color col2 = colorChange2;
        Debug.Log("Initial Color: " + col2.a);
        col2.a -= col2.a * colorDampener * Time.deltaTime;
        Debug.Log("Color: " + col2.a);


    }
}

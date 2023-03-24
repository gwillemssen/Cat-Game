using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(FirstPersonInput))]
[RequireComponent(typeof(FirstPersonInteraction))]
[RequireComponent(typeof(AudioPlayer))]

public class FirstPersonController : MonoBehaviour
{
    public static FirstPersonController instance; //SINGLETON TIME

    [Header("Player")]
    [Tooltip("Move speed of the character in m/s")]
    public float MoveSpeed = 4.0f;
    [Tooltip("Sprint speed of the character in m/s")]
    public float SprintSpeed = 6.0f;
    [Tooltip("Rotation speed of the character")]
    public float RotationSpeed = 1.0f;
    [Tooltip("higher value = more smooth, lower value = more snappy")]
    public float Smoothing = .1f;
    [Tooltip("Move speed of the character when crouching")]
    public float CrouchSpeed = 2f;
    [Tooltip("How fast do we transition from crouching / standing")]
    public float CrouchTransitionSpeed = 8f;


    [Space(10)]
    [Tooltip("The height the player can jump")]
    public float JumpHeight = 1.2f;
    [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
    public float Gravity = -15.0f;

    [Space(10)]
    [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
    public float JumpTimeout = 0.1f;
    [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
    public float FallTimeout = 0.15f;


    [Header("Player Grounded")]
    [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
    public bool Grounded = true;
    [Tooltip("Useful for rough ground")]
    public float GroundedOffset = -0.14f;
    [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
    public float GroundedRadius = 0.5f;
    [Tooltip("What layers the character uses as ground")]
    public LayerMask GroundLayers;

    [Tooltip("How far in degrees can you move the camera up")]
    [HideInInspector]
    public float TopClamp = 90.0f;
    [Tooltip("How far in degrees can you move the camera down")]
    [HideInInspector]
    public float BottomClamp = -90.0f;
    private float _cinemachineTargetPitch;
    public GameObject CinemachineCameraTarget;

    [Header("Sound")]
    private AudioPlayer audioPlayer;
    public Sound[] FootstepSounds;
    public float StepCooldownWalk = .5f;
    public float StepCooldownChase = .25f;

    //Public Properties
    [HideInInspector]
    public Camera MainCamera { get; private set; }
    [HideInInspector]
    public FirstPersonInput Input { get; private set; }
    [HideInInspector]
    public FirstPersonInteraction Interaction { get; private set; }
    public PlayerUI UI;
    [HideInInspector]
    public bool IsCrouching { get; private set; }
    [HideInInspector]
    public bool IsMoving { get; private set; }


    // player
    private Vector2 wishMove;
    private Vector3 wishMoveDir;
    private Vector3 moveDir;
    private Vector3 moveDamp;
    private float targetSpeed;
    private float rotationVelocity;
    private float verticalVelocity;
    private float terminalVelocity = 53.0f;
    private float jumpTimeoutDelta;
    private float fallTimeoutDelta;
    private Vector3 crouchScale;
    private Vector3 resetScale;
    private float stepCooldown;
    private float lastTimeStep = -420f;

    [Header("Other")]
    public Transform[] VisibilityCheckPoints;
    public Transform CatHoldingPosition;
    public Transform PickupPosition;
    public Transform PickupPositionWindup;
    [HideInInspector]
    public bool DisableMovement = false;
    [HideInInspector]
    public bool DisableCamera = false;
    [HideInInspector]
    public bool Hiding = false;

    //Private References
    private CharacterController controller;
    //Constants
    private static float CameraFOVLerpSpeed = 2f;
    private const float _threshold = 0.01f;
    private const bool isCurrentDeviceMouse = true;
    private const bool useAnalogMovement = false; //enable this if we want to use a controller
    private float crouchAmount;
    private bool crouching = false;
    private int lastRng;

    private void Awake()
    {
        if(!GameObject.Find("GameManager"))
        { Debug.LogError("Add the GameManager to the scene from Prefabs"); }

        if (instance != null)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }

        MainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        Input = GetComponent<FirstPersonInput>();
        audioPlayer = GetComponent<AudioPlayer>();
        Interaction = GetComponent<FirstPersonInteraction>();
        Interaction.Init(this);
        UI.Init(this);
    }

    private void Start()
    {
        crouchScale = new(transform.localScale.x, transform.localScale.y / 2, transform.localScale.z); //makes it half constantly. Only need it to half once.
        resetScale = transform.localScale;

        controller = GetComponent<CharacterController>();
        // reset our timeouts on start
        jumpTimeoutDelta = JumpTimeout;
        fallTimeoutDelta = FallTimeout;
    }

    private void LateUpdate()
    {
        controller.enabled = !DisableMovement;
        if (!DisableMovement)
        {
            JumpAndGravity();
            GroundedCheck();
            Move();
            Audio();
        }
        if (!DisableCamera)
        { CameraRotation(); }
        Interaction.UpdateInteraction();
    }

    private void Audio()
    {
        if(Input.move.sqrMagnitude < 0.05f)
        { return; }

        stepCooldown = Input.sprint ? StepCooldownChase : StepCooldownWalk;

        if (Time.time > lastTimeStep + stepCooldown)
        {
            int rng = Random.Range(0, FootstepSounds.Length);

            
            lastTimeStep = Time.time;
            audioPlayer.Play(FootstepSounds[rng]);



            if (lastRng == rng && rng! >= FootstepSounds.Length)
            {
                // rng ++;

                audioPlayer.Play(FootstepSounds[rng + 1]);

            }
            if (lastRng > FootstepSounds.Length)
            {
                lastRng = 0;
                audioPlayer.Play(FootstepSounds[rng - 1]);
            }
            lastRng = rng;
        }

    }

    private void GroundedCheck()
    {
        // set sphere position, with offset
        Vector3 spherePosition = new(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
        Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
    }

    private void CameraRotation()
    {
        //Don't multiply mouse input by Time.deltaTime
        float deltaTimeMultiplier = isCurrentDeviceMouse ? 1.0f : Time.deltaTime;
        _cinemachineTargetPitch += Input.look.y * RotationSpeed * deltaTimeMultiplier;
        rotationVelocity = Input.look.x * RotationSpeed * deltaTimeMultiplier;
        // clamp our pitch rotation
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);
        // Update Cinemachine camera target pitch
        CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);
        // rotate the player left and right
        transform.Rotate(Vector3.up * rotationVelocity);
    }

    private void Move()
    {
        wishMove = Input.move.normalized;
        wishMoveDir.x = wishMove.x;
        wishMoveDir.y = 0f;
        wishMoveDir.z = wishMove.y;

        wishMoveDir = transform.TransformDirection(wishMoveDir);

        IsCrouching = Input.crouch;
        IsMoving = Input.move.SqrMagnitude() > 0.2f;

        // targetSpeed = Input.sprint ? SprintSpeed : MoveSpeed;
        if (Input.crouch)
        { targetSpeed = CrouchSpeed; }
        else if(Input.sprint)
        { targetSpeed = SprintSpeed; }
        else
        { targetSpeed = MoveSpeed; }

        moveDir = Vector3.SmoothDamp(moveDir, targetSpeed * wishMoveDir, ref moveDamp, Smoothing);

        moveDir.y = verticalVelocity;
        controller.Move(moveDir * Time.deltaTime);
    }
  
    private void JumpAndGravity()
    {
        if (Grounded)
        {
            // reset the fall timeout timer
            fallTimeoutDelta = FallTimeout;

            // stop our velocity dropping infinitely when grounded
            if (verticalVelocity < 0.0f)
            {
                verticalVelocity = -2f;
            }

            // Jump
            if (Input.jump && jumpTimeoutDelta <= 0.0f)
            {
                // the square root of H * -2 * G = how much velocity needed to reach desired height
                verticalVelocity = Mathf.Sqrt(JumpHeight * -1.15f * Gravity);
            }

            // jump timeout
            if (jumpTimeoutDelta >= 0.0f)
            {
                jumpTimeoutDelta -= Time.deltaTime;
            }

            //Crouch
            
            if(Input.crouchOnce)
            { crouching = !crouching; }

            if (crouching) // If the crouch button is held down
            { crouchAmount += Time.deltaTime * CrouchTransitionSpeed; }
            else
            { crouchAmount -= Time.deltaTime* CrouchTransitionSpeed; }

            crouchAmount = Mathf.Clamp01(crouchAmount);
            transform.localScale = Vector3.Lerp(resetScale, crouchScale, crouchAmount);
        } 
        else
        {
            // reset the jump timeout timer
            jumpTimeoutDelta = JumpTimeout;

            // fall timeout
            if (fallTimeoutDelta >= 0.0f)
            {
                fallTimeoutDelta -= Time.deltaTime;
            }

            // if we are not grounded, do not jump
            Input.jump = false;
        }

        // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
        if (verticalVelocity < terminalVelocity)
        {
            verticalVelocity += Gravity * Time.deltaTime;
        }
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    private void OnDrawGizmosSelected()
    {
        Color transparentGreen = new(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new(1.0f, 0.0f, 0.0f, 0.35f);

        if (Grounded) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;

        // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
        Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
    }

}
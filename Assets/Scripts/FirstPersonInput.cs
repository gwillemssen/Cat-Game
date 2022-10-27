using UnityEngine;

public class FirstPersonInput : MonoBehaviour
{
    [Header("Character Input Values")]
    public Vector2 move;
    public Vector2 look;
    private Vector2 stop = new(0,0);
    public Vector2 mousePosition;
    public bool jump;
    public bool sprint;
    public bool crouch;
    public bool interacting;
    public bool interactedOnce;
    public bool throwing;
    private Sound playerSound;
    public bool isMoving;
    private bool isPlaying;
    private float lastTimeStep;

    [Space(10)]
    [Tooltip("Cooldown for sounds to play")]
    public float stepCooldown = .5f;
    public float sprintCooldown =.25f;


    [Header("Mouse Cursor Settings")]
    public bool cursorLocked = true;
    public bool cursorInputForLook = true;

    private void OnApplicationFocus(bool hasFocus)
    {
        SetCursorState(cursorLocked);
    }

    
    private void Update()
    {
       
        
        move.x = Input.GetAxisRaw("Horizontal");
        move.y = Input.GetAxisRaw("Vertical");
        look.x = Input.GetAxis("Mouse X");
        look.y = -Input.GetAxis("Mouse Y");
        jump = Input.GetButtonDown("Jump");
        sprint = Input.GetButton("Sprint");
        throwing = Input.GetButtonDown("Throw");
        interactedOnce = Input.GetButtonDown("Interact");
        interacting = Input.GetButton("Interact");
        mousePosition = Input.mousePosition;
        crouch = Input.GetButtonDown("Crouch");

        if (move.x != stop.x || move.y != stop.y)
        {
            CheckAudio();
        }
        if (sprint)
        {
            stepCooldown = sprintCooldown;
        }
        else
        {
            stepCooldown = .5f;
        }
    }

    void PlayFootstep()
    {
        FindObjectOfType<AudioManager>().Play("Footsteps");
        isPlaying = false;

    }
    void CheckAudio()
    {
        if (Time.time > lastTimeStep + stepCooldown)
        {
            lastTimeStep = Time.time;
            isPlaying = true;
        }


        if (isPlaying == true)
        {
            PlayFootstep();

        }
    }


    private void SetCursorState(bool newState)
    {
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }
          
}
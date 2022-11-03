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
        crouch = Input.GetButton("Crouch");
    }

    private void SetCursorState(bool newState)
    {
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }
          
}
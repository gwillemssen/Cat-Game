using UnityEngine;

public class FirstPersonInput : MonoBehaviour
{
    [Header("Character Input Values")]
    public Vector2 move;
    public Vector2 look;
    public Vector2 mousePosition;
    public bool jump;
    public bool sprint;
    public bool interacting;
    public bool interactedOnce;
    public bool throwing;

    [Header("Movement Settings")]
    public bool analogMovement;

    [Header("Mouse Cursor Settings")]
    public bool cursorLocked = true;
    public bool cursorInputForLook = true;

    private void OnApplicationFocus(bool hasFocus)
    {
        SetCursorState(cursorLocked);
    }

    private void Update()
    {
        move.x = Input.GetAxis("Horizontal");
        move.y = Input.GetAxis("Vertical");
        look.x = Input.GetAxis("Mouse X");
        look.y = -Input.GetAxis("Mouse Y");
        jump = Input.GetButtonDown("Jump");
        sprint = Input.GetButtonDown("Sprint");
        throwing = Input.GetButtonDown("Throw");
        interactedOnce = Input.GetButtonDown("Interact");
        interacting = Input.GetButton("Interact");
        mousePosition = Input.mousePosition;
    }

    private void SetCursorState(bool newState)
    {
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }
}
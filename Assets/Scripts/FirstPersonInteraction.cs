using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonInteraction : MonoBehaviour
{
    [Tooltip("How far can the player reach?")]
    public float InteractRange = 8.0f;
    public float ThrowForceMin = 0.5f;
    public float ThrowForceMax = 8.0f;
    public float ThrowWindupSpeed = 0.75f;
    [Tooltip("What layers can we interact with?")]
    public LayerMask InteractableLayerMask;
    public Texture2D CrosshairSprite_Normal;
    public Texture2D CrosshairSprite_Interactable;
    public Texture2D CrosshairSprite_Noise;
    [HideInInspector]
    public bool HideCrosshair = false;

    private FirstPersonController controller;
    private Interactable interactable = null;
    public InteractablePickup Pickup { get; private set; } = null;
    private InteractablePickup interactablePickup;
    private Interactable lastInteractable = null;

    //crosshair
    private Texture2D crosshairImage;
    private Vector2 crosshairPos;
    private Vector2 crosshairPosInteract;
    private Vector2 crosshairPosInteractBig;
    private Vector2 crosshairSizeInteractBig;
    private const float crosshairInteractBigMult = 1.5f;
    private float lastTimeNewInteractable = -420f;
    private float throwForce;

    public void Init(FirstPersonController con)
    {
        controller = con;
        CalculateCrosshair();
    }

    public void UpdateInteraction()
    {
        RaycastForInteractable();

        HandleInteraction();

        UpdateCrosshair();
    }

    private void PickupInteractable()
    {
        Pickup = interactablePickup;
        interactablePickup.InteractClick(controller);
        Pickup.Rigidbody.isKinematic = true;
    }

    private void ThrowInteractable()
    {
        Pickup.transform.position = controller.MainCamera.transform.position;
        Pickup.Rigidbody.isKinematic = false;
        /*RaycastHit hit;
        if(!Physics.Raycast(controller.MainCamera.transform.position, Pickup.transform.position - controller.MainCamera.transform.position, out hit, InteractRange, InteractableLayerMask, QueryTriggerInteraction.Collide))
        {
            Pickup.transform.position = controller.MainCamera.transform.position;
        }*/
        //throw from same position if its not through a wall
        Pickup.Rigidbody.AddForce(transform.forward * Mathf.Lerp(ThrowForceMin, ThrowForceMax, throwForce), ForceMode.VelocityChange);
        throwForce = 0f;
        Pickup = null;
    }

    private void Interact()
    {
        //regular interaction
        interactable.Interact(controller);
        if (controller.Input.interactedOnce)
        { interactable.InteractClick(controller); }
    }

    private void HandleInteraction()
    {

        if (controller.Input.interacting && interactable != null)
        {
            interactablePickup = interactable as InteractablePickup;

            if(interactablePickup != null)
            {
                if (Pickup == null)
                { PickupInteractable(); }
            }
            else
            {
                Interact();
            }
        }
        if (Pickup != null)
        {
            float t = Mathf.Sin(throwForce * Mathf.PI * 0.5f);
            Pickup.transform.position = Vector3.Lerp(controller.PickupPosition.position, controller.PickupPositionWindup.position, t);
            Pickup.transform.rotation = Quaternion.Lerp(controller.PickupPosition.rotation, controller.PickupPositionWindup.rotation, t);

            if (controller.Input.throwing)
            {
                throwForce += Time.deltaTime * ThrowWindupSpeed;
                throwForce = Mathf.Clamp01(throwForce);

                PlayerUI.instance.SetThrowStrengthMeter(throwForce);
            }
            else if (controller.Input.throwRelease)
            { ThrowInteractable(); }
        }

        if (lastInteractable != interactable && lastInteractable != null)
        { lastInteractable.LookingAt = false; }

        if (interactable != lastInteractable && interactable != null)
        { lastTimeNewInteractable = Time.time; }

        lastInteractable = interactable;
    }

    private void RaycastForInteractable()
    {
        RaycastHit hit;

        if (Physics.Raycast(controller.MainCamera.transform.position, controller.MainCamera.transform.forward, out hit, InteractRange, InteractableLayerMask, QueryTriggerInteraction.Collide))
        {
            interactable = hit.transform.GetComponent<Interactable>();
            if (interactable != null)
            {
                if (!interactable.CanInteract)
                { interactable = null; }
                else
                { interactable.LookingAt = true; }
            }
        }
        else
        { interactable = null; }
    }

    private void UpdateCrosshair()
    {
        if(interactable == null)
            { crosshairImage = CrosshairSprite_Normal; }
        else if(interactable.NoiseCrosshair == false)
            { crosshairImage = CrosshairSprite_Interactable; }
        else
            { crosshairImage = CrosshairSprite_Noise; }
    }

    public void CalculateCrosshair()
    {
        crosshairImage = CrosshairSprite_Normal;
        float xMin = (Screen.width / 2) - (crosshairImage.width / 2);
        float yMin = (Screen.height / 2) - (crosshairImage.height / 2);
        crosshairPos = new Vector2(xMin, yMin);
        crosshairPosInteract = new Vector2((Screen.width / 2) - (CrosshairSprite_Interactable.width / 2), (Screen.height / 2) - (CrosshairSprite_Interactable.height / 2));

        crosshairSizeInteractBig = new Vector2(CrosshairSprite_Interactable.width * crosshairInteractBigMult, CrosshairSprite_Interactable.height * crosshairInteractBigMult);
        crosshairPosInteractBig = new Vector2((Screen.width / 2) - (crosshairSizeInteractBig.x / 2), (Screen.height / 2) - (crosshairSizeInteractBig.y / 2));
    }

    void OnGUI()
    {
        if (HideCrosshair)
            return;
        GUI.color = Color.white; //change the transparency here if needed

        Vector2 pos = crosshairPos;
        float t = 1f;
        Vector2 size;
        size.x = crosshairImage.width;
        size.y = crosshairImage.height;

        if (crosshairImage == CrosshairSprite_Interactable || crosshairImage == CrosshairSprite_Noise)
        {
            t = (Time.time - lastTimeNewInteractable) / .5f;
            t = Mathf.Clamp01(t);
            t = Mathf.Sin(t * Mathf.PI * 0.5f);

            pos = Vector2.Lerp(crosshairPosInteractBig, crosshairPosInteract, t);
            size = Vector2.Lerp(crosshairSizeInteractBig, new Vector2(CrosshairSprite_Interactable.width, CrosshairSprite_Interactable.height), t);
        }

        GUI.DrawTexture(new Rect(pos.x, pos.y, size.x, size.y), crosshairImage);

        GUI.color = Color.white;
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FirstPersonInteraction : MonoBehaviour
{
    [Tooltip("How far can the player reach?")]
    public float InteractRange = 10.0f;
    public float ThrowForceMin = 0.5f;
    public float ThrowForceMax = 10.0f;
    public float ThrowWindupSpeed = 1f;
    [Tooltip("What layers can we interact with?")]
    public LayerMask InteractableLayerMask;
    public Texture2D CrosshairSprite_Normal;
    public Texture2D CrosshairSprite_Interactable;
    public Texture2D CrosshairSprite_Noise;
    public Texture2D CrosshairSprite_Locked;
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
        interactablePickup.Interact(controller);
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
        Pickup.Rigidbody.AddForce(controller.MainCamera.transform.forward * ThrowForceMax, ForceMode.VelocityChange);
        if(throwForce > 0.5f)
        { Pickup.AlwaysMakeImpact = true; }
        throwForce = 0f;
        Pickup = null;
    }

    public void DestroyPickup()
    {
        if(Pickup == null)
        { return;  }
        Destroy(Pickup.gameObject);
        Pickup = null;
    }

    private void Interact()
    {
        if (Pickup != null)
        { interactable.InteractWith(controller, Pickup); }
        interactable.InteractHold(controller);
        if (controller.Input.interactedOnce)
        { interactable.Interact(controller); }
        interactable.InteractBase();
        
    }

    private void HandleInteraction()
    {
        if (controller.Input.interacting && interactable != null)
        {
            interactablePickup = interactable as InteractablePickup; 

            if (interactablePickup != null)
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
                bool requiredItemMatches = true;
                if(interactable.RequiredItemToViewInteraction != "" && Pickup == null)
                { requiredItemMatches = false; }
                else if (interactable.RequiredItemToViewInteraction != "" && Pickup != null && interactable.RequiredItemToViewInteraction != Pickup.name)
                { requiredItemMatches = false; }


                if (!interactable.CanInteract || interactable.Disabled || !requiredItemMatches)
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
        
        if (interactable == null) SetCrosshair("Normal");
        else if (interactable != null && !interactable.VisiblyLockedOnView ) SetCrosshair("Interactable");
        else if(interactable != null && interactable.VisiblyLockedOnView) SetCrosshair("Locked");
    }
    public void SetCrosshair(string crosshairInput)
    {
        Texture2D newCrosshair;
        switch (crosshairInput)
        {
            case "Normal":
                newCrosshair = CrosshairSprite_Normal;
                break;

            case "Interactable":
                newCrosshair = CrosshairSprite_Interactable;
                break;

            case "Noise":
                newCrosshair = CrosshairSprite_Noise;
                break;

            case "Locked":
                newCrosshair = CrosshairSprite_Locked;
                break;

            default:
                newCrosshair = CrosshairSprite_Normal;
                break;
        }
        crosshairImage = newCrosshair;
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

        if (crosshairImage != CrosshairSprite_Normal)
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

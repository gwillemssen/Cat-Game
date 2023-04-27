using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Interactable
{
    [SerializeField] private Transform forward;
    [SerializeField] private Transform pivot;
    //[SerializeField] private float time = 1f;
    [SerializeField] private Collider doorcollider;

    private Transform targetRot;
    private bool open;
    private float lastTimeOpenedDoor = -420f;
    public AudioSource doorOpen;
    public AudioSource doorClose;

    private void Start()
    {
        targetRot = new GameObject("doorTargetRot").transform;
        targetRot.SetParent(pivot.parent);
        targetRot.localRotation = pivot.localRotation;
        
    }

    public override void Interact(FirstPersonController controller)
    {
        InteractDoor(controller.transform.position);
    }

    private void InteractDoor(Vector3 pos)
    {
        lastTimeOpenedDoor = Time.time;
        open = !open;
        if(open) doorOpen.Play();
        float targetY = Vector3.Dot(forward.forward, (forward.position - pos)) < 0f ? -90f : 90f;
        if (!open)
        { targetY = 0f; doorClose.Play(); }

        targetRot.localRotation = Quaternion.Euler(0f, targetY, 0f);

        
    }

    public void DoorTriggerEnter(Vector3 pos)
    {
        if(open)
        { return; }

        InteractDoor(pos);
    }

    private void Update()
    {
        pivot.localRotation = Quaternion.Lerp(pivot.localRotation, targetRot.localRotation, Time.deltaTime * 4f);
        doorcollider.isTrigger = open || (!open && Mathf.Abs(pivot.localRotation.y) > .1f);
        //pivot.rotation = Quaternion.Euler(pivot.rotation.x, Mathf.Lerp(pivot.rotation.y, targetRot, Time.deltaTime * 4f), pivot.rotation.z);
    }



}

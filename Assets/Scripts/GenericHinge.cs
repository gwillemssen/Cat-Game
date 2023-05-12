using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericHinge : Interactable

{
    public GameObject Hinge;
    public Vector3 DefaultPos, TransformPos;
    public List<AudioClip> OpenSounds, ClosedSounds;
    public float OpeningDuration, ClosingDuration;
    public bool ApplyOnStart;
    // Start is called before the first frame update
    void Start()
    {
        if (ApplyOnStart)
        {
            OpenHinge(Hinge, DefaultPos, TransformPos, OpeningDuration, ClosingDuration, OpenSounds, ClosedSounds);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Interact(FirstPersonController controller)
    {
        OpenHinge(Hinge, DefaultPos, TransformPos, OpeningDuration, ClosingDuration, OpenSounds, ClosedSounds);
        
    }
}

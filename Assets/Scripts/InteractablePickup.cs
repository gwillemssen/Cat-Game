using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class InteractablePickup : Interactable
{
    public Rigidbody Rigidbody { get; private set; }
    public Transform OriginalParent { get; private set; }

    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
        OriginalParent = transform.parent;
    }

    public override void InteractClick(FirstPersonController controller)
    {
        controller.UI.SetInfoText("Left click to use\nRight click to drop");
    }
}

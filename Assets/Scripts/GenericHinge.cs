using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericHinge : Interactable

{
    [SerializeField] private GameObject Hinge;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Interact(FirstPersonController controller)
    {
        OpenHinge(Hinge, new Vector3(0,0,0), new Vector3 (0,0,90), 1f, .5f);
    }
}

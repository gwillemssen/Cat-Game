using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SwitchController : Interactable
{
    private LerpScript switchLerp;
    private GameObject switchBone;
    public bool on;
    private bool storedValue;
    private Vector3 offPos = new Vector3(40, 0, 0);
    private Vector3 onPos = new Vector3(-40, 0, 0);
    public List<Light> lights;
    // Start is called before the first frame update
    void Start()
    {
        switchBone = transform.GetChild(2).GetChild(0).gameObject;
        switchLerp = this.AddComponent<LerpScript>();
        switchLerp.typeOfLerp = LerpScript.LerpType.Vector3;
        switchLerp.lerpSpeed = 8;
    }

    // Update is called once per frame
    void Update()
    {
        if (on)
        {
            switchLerp.vecTarget = onPos;
        }
        else
        {
            switchLerp.vecTarget = offPos;

        }
        switchBone.transform.localRotation = Quaternion.Euler(switchLerp.vecVal);
        if (storedValue != on)
        {
            ToggleLight();
            storedValue = on;
        }

    }

    void ToggleLight()
    {
        foreach (Light light in lights)
        {
            light.enabled = !light.enabled;
        }
    }
    
    public override void InteractClick(FirstPersonController controller)
    {
        on = !on;
    }

    
}

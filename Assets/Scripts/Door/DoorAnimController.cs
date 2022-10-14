using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorAnimController : MonoBehaviour
{
    private LerpScript doorLerp;

    public bool open;

    private GameObject doorHinge;

    private Collider doorCollider;
    
    // Start is called before the first frame update
    void Start()
    {
        doorLerp = gameObject.AddComponent<LerpScript>();
        doorLerp.typeOfLerp = LerpScript.LerpType.Vector3;
        doorLerp.lerpSpeed = 8;
        doorHinge = transform.GetChild(3).GetChild(0).gameObject;
        doorCollider = doorHinge.GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (open)
        {
            if (doorLerp.vecTarget != new Vector3(0, 0, 103))
            {
                doorLerp.vecTarget = new Vector3(0, 0, 103);
            }
        }
        else
        {
            if (doorLerp.vecTarget != Vector3.zero)
            {
                doorLerp.vecTarget = new Vector3(0, 0, 0);
            }
        }

        if (doorHinge.transform.localRotation.eulerAngles != doorLerp.vecTarget)
        {
            if (doorCollider.enabled)
            {
                doorCollider.enabled = false;
            }
            doorHinge.transform.localRotation = Quaternion.Euler(doorLerp.vecVal);
        }
        else
        {
            if (!doorCollider.enabled)
            {
                doorCollider.enabled = true;
            }
        }

    }
}

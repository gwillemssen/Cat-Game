using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorAnimController : MonoBehaviour
{
    private LerpScript doorLerp;

    public bool open;

    private GameObject doorHinge;
    
    // Start is called before the first frame update
    void Start()
    {
        doorLerp = gameObject.AddComponent<LerpScript>();
        doorLerp.typeOfLerp = LerpScript.LerpType.Vector3;
        doorLerp.lerpSpeed = 4;
        doorHinge = transform.GetChild(3).GetChild(0).gameObject;

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
            doorHinge.transform.localRotation = Quaternion.Euler(doorLerp.vecVal);
        }

    }
}

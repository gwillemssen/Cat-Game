using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Desk_Controller : MonoBehaviour
{
    private LerpScript deskLerp;

    public bool open;

    public GameObject drawerBone;


    private float lastTime;

    public float Cooldown;

    // Start is called before the first frame update
    void Start()
    {
        deskLerp = gameObject.AddComponent<LerpScript>();
        deskLerp.typeOfLerp = LerpScript.LerpType.Vector3;
        deskLerp.lerpSpeed = 8;
        drawerBone = transform.GetChild(3).gameObject;
    }

    // Update is called once per frame
    void Update()
    {

        if (open)
        {

            if (deskLerp.vecTarget != new Vector3(0, 0, 103))
            {
                deskLerp.vecTarget = new Vector3(0, 0, 103);
            }
        }
        else
        {

            if (deskLerp.vecTarget != Vector3.zero)
            {
                deskLerp.vecTarget = new Vector3(0, 0, 0);
            }
        }



    }

}

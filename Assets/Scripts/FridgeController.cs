using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class FridgeController : MonoBehaviour
{
    // Start is called before the first frame update


    private LerpScript fridgelerp;
    private LerpScript fridgelerp2;

    public bool doorOpen;
    public bool bottomDoorOpen;

    public GameObject[] doors;

    private Vector3 position = new Vector3(0,0,-146);

    void Start()
    {
        doors[0] = transform.GetChild(2).GetChild(0).gameObject;
        doors[1] = transform.GetChild(2).GetChild(1).gameObject;
        
        fridgelerp = this.AddComponent<LerpScript>();
        fridgelerp.typeOfLerp = LerpScript.LerpType.Vector3;

        fridgelerp2 = this.AddComponent<LerpScript>();
        fridgelerp2.typeOfLerp = LerpScript.LerpType.Vector3;
    }

    // Update is called once per frame
    void Update()
    {
        if (doorOpen)
        {
            fridgelerp.vecTarget = position; 
        }
        else
        {
            fridgelerp.vecTarget = Vector3.zero;
        }
        doors[0].transform.localRotation = Quaternion.Euler(fridgelerp.vecVal);
        
        if (bottomDoorOpen)
        {
            fridgelerp2.vecTarget = position; 
        }
        else
        {
            fridgelerp2.vecTarget = Vector3.zero;
        }
        doors[1].transform.localRotation = Quaternion.Euler(fridgelerp2.vecVal);
    }

}

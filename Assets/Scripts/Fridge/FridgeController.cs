using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class FridgeController : Interactable
{
    // Start is called before the first frame update
    private bool initialized = false;

    private LerpScript topDoorLerp;
    private LerpScript bottomDoorLerp;

    public bool topDoorOpen;
    public bool bottomDoorOpen;

    private GameObject[] doors = new GameObject[] {null,null};

    private Vector3 position = new Vector3(0,0,-146);

    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        DoorManager();
    }

    void Initialize()
    {
        doors[0] = transform.GetChild(2).GetChild(0).gameObject;
        doors[1] = transform.GetChild(2).GetChild(1).gameObject;
        
        topDoorLerp = this.AddComponent<LerpScript>();
        topDoorLerp.typeOfLerp = LerpScript.LerpType.Vector3;

        bottomDoorLerp = this.AddComponent<LerpScript>();
        bottomDoorLerp.typeOfLerp = LerpScript.LerpType.Vector3;

        initialized = true;
    }

    void DoorManager()
    {
        if (initialized)
        {
            if (topDoorOpen)
            {
                topDoorLerp.vecTarget = position; 
            }
            else
            {
                topDoorLerp.vecTarget = Vector3.zero;
            }
            doors[0].transform.localRotation = Quaternion.Euler(topDoorLerp.vecVal);
        
            if (bottomDoorOpen)
            {
                bottomDoorLerp.vecTarget = position; 
            }
            else
            {
                bottomDoorLerp.vecTarget = Vector3.zero;
            }
            doors[1].transform.localRotation = Quaternion.Euler(bottomDoorLerp.vecVal);
        }
    }
    
    public override void InteractClick(FirstPersonController controller)
    {
        
    }

}

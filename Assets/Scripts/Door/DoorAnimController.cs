using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorAnimController : MonoBehaviour
{
    public bool open;

    private GameObject doorHinge;

    private Collider doorCollider;

    private float lastTime;
   
    public float Cooldown;

    // Start is called before the first frame update
    void Start()
    {
        doorHinge = transform.GetChild(3).GetChild(0).gameObject;
        doorCollider = doorHinge.GetComponent<Collider>();
    }

    public void Open(bool open)
    {
        if (open)
        {
            doorHinge.LeanRotateY(-90,0.2f);
        }
        else
        {
            doorHinge.LeanRotateY(0,0.2f);
        }

    }
    
    


}

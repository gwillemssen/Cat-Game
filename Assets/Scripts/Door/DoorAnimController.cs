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

    private Quaternion startRot;
    private Quaternion endRot;
    private Quaternion target;

    // Start is called before the first frame update
    void Start()
    {
        doorHinge = transform.GetChild(3).GetChild(0).gameObject;
        doorCollider = doorHinge.GetComponent<Collider>();
        startRot = doorHinge.transform.rotation;
        endRot = Quaternion.Euler(new Vector3(doorHinge.transform.rotation.eulerAngles.x, doorHinge.transform.rotation.eulerAngles.y, doorHinge.transform.rotation.eulerAngles.z + 100f));
        target = startRot;
    }

    public void Open(bool open)
    {
        if (open)
        {
            //doorHinge.LeanRotateY(-90,0.2f);
            target = endRot;
            doorCollider.enabled = false;
        }
        else
        {
            //doorHinge.LeanRotateY(0,0.2f);
            target = startRot;
            doorCollider.enabled = true;
        }
    }

    private void Update()
    {
        doorHinge.transform.rotation = Quaternion.Lerp(doorHinge.transform.rotation, target, Time.deltaTime * 5f);
    }




}

using UnityEngine;
using System.Collections;

public class Billboard : MonoBehaviour
{
    public static Transform cam;
    
    private Vector3 freeRotation = Vector3.one;
    private Vector3 eangles = Vector3.zero;

    void Start()
    {
        if (cam == null)
        {
            cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Transform>();
        }
    }

    void LateUpdate()
    {
        this.transform.LookAt(cam);
        transform.Rotate(0, 180, 0);
        eangles = transform.eulerAngles;
        eangles.x *= freeRotation.x;
        eangles.y *= freeRotation.y;
        eangles.z *= freeRotation.z;
        transform.eulerAngles = eangles;
    }
}

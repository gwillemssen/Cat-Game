using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SinMover : MonoBehaviour
{
    float moveAmt;
    public float timeToLoop;
    float timer;
    Rigidbody rb;
    Vector3 departurePosition;
    Vector3 destinationPosition;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    private void Update()
    {
        departurePosition = transform.parent.Find("Start").transform.position;
        destinationPosition = transform.parent.Find("Destination").transform.position;
        timer += Time.deltaTime;
        moveAmt = (Mathf.Sin(timer / (timeToLoop / 4)) + 1) / 2;
    }
    void FixedUpdate()
    {
        rb.position = (Vector3.Lerp(departurePosition, destinationPosition, moveAmt));
    }
}
